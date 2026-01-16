#include <array>
#include <vector>
#include <cassert>
#include <iostream>
#include <asmjit/x86.h>

#include "jit/jit_x86_64.hpp"
#include "common.hpp"

namespace czffvm_jit {

using namespace czffvm;

X86JitCompiler::X86JitCompiler()
    : runtime(std::make_shared<asmjit::JitRuntime>()) {

#ifdef DEBUG_BUILD
    std::cout << "[JIT] Using shared_ptr runtime, use_count=" 
              << runtime.use_count() << std::endl;

    std::cout << "[JIT] X86JitCompiler constructor called" << std::endl;
    std::cout << "[JIT] Runtime created at address: " << runtime << std::endl;
#endif

    if (!runtime) {
        std::cerr << "[JIT] FATAL: Failed to create JitRuntime!" << std::endl;
        throw std::runtime_error("Failed to create JitRuntime");
    }

    auto env = runtime->environment();
}

std::unique_ptr<CompiledRuntimeFunction> X86JitCompiler::CompileFunction(const czffvm::RuntimeFunction& function, czffvm::RuntimeDataArea& rda) {
    
#ifdef DEBUG_BUILD
    std::cout << "[JIT] Starting compilation..." << std::endl;
    std::cout << "[JIT] Function has " << function.code.size() << " operations" << std::endl;
#endif

    try {
    asmjit::CodeHolder code;
    auto err = code.init(runtime->environment());
    if (err != asmjit::kErrorOk) {
        std::cerr << "[JIT] ERROR: Failed to initialize CodeHolder: " 
                  << asmjit::DebugUtils::error_as_string(err) << std::endl;
        return nullptr;
    }
    
    asmjit::x86::Assembler a(&code);
    
#ifdef DEBUG_BUILD
    std::cout << "[JIT] Generating Windows x64 prologue..." << std::endl;
#endif

    // prologue
    asmjit::x86::Gp stackBase = asmjit::x86::r13;
    asmjit::x86::Gp stackPtr  = asmjit::x86::r12;
    asmjit::x86::Gp heapPtr   = asmjit::x86::r14;

    // save non-volatiles
    a.push(asmjit::x86::rbp);
    a.mov(asmjit::x86::rbp, asmjit::x86::rsp);
    a.push(asmjit::x86::r12);
    a.push(asmjit::x86::r13);
    a.push(asmjit::x86::r14);

    a.sub(asmjit::x86::rsp, 32); // shadow space for windows x64

    a.mov(stackBase, asmjit::x86::rcx);
    a.lea(stackPtr, ptr(stackBase, function.locals_count * 4));
    a.mov(heapPtr, asmjit::x86::rdx);

    std::vector<asmjit::v1_21::Label> labels(function.code.size());
    for (auto& l : labels)
        l = a.new_label();
    
#ifdef DEBUG_BUILD
    std::cout << "[JIT] Compiling operations..." << std::endl;
#endif

    size_t ip = 0;
    for (const auto& op : function.code) {
#ifdef DEBUG_BUILD
        std::cout << "[JIT-OP] Code: 0x" << std::hex << (uint16_t)op.code << std::dec 
                  << ", args: " << op.arguments.size() << std::endl;
#endif

        a.bind(labels[ip]);
        CompileOperation(a, stackPtr, stackBase, heapPtr, op, labels);
        ip += 1;
    }

    a.mov(asmjit::x86::eax, 0);
    
    // epilogue
    a.add(asmjit::x86::rsp, 32);
    a.pop(asmjit::x86::r14);
    a.pop(asmjit::x86::r13);
    a.pop(asmjit::x86::r12);
    a.pop(asmjit::x86::rbp);
    a.ret();
    
    void* funcPtr = nullptr;
    
    err = runtime->add(&funcPtr, &code);
    
    if (err != asmjit::kErrorOk) {
        std::cerr << "[JIT] ERROR: Failed to add code to runtime: " 
                  << asmjit::DebugUtils::error_as_string(err) << std::endl;
        throw std::runtime_error("Failed to compile function");
    }

#ifdef DEBUG_BUILD
    std::cout << "[JIT] Success! Compiled function at address: " << funcPtr << std::endl;
    std::cout << "[JIT] Code size: " << code.code_size() << " bytes" << std::endl;
#endif

    return std::make_unique<X86CompiledRuntimeFunction>(
        funcPtr,
        code.code_size(),
        runtime
    );
    } catch (const std::exception& e) {
        std::cerr << "[JIT] EXCEPTION in compileFunction: " << e.what() << std::endl;
        return nullptr;
    } catch (...) {
        std::cerr << "[JIT] UNKNOWN EXCEPTION in compileFunction" << std::endl;
        return nullptr;
    }
}

void X86JitCompiler::CompileOperation(
    asmjit::x86::Assembler& a, 
    asmjit::x86::Gp& stackPtr, 
    asmjit::x86::Gp stackBase, 
    asmjit::x86::Gp heapPtr, 
    const Operation& op,
    const std::vector<asmjit::v1_21::Label>& labels
) {
    using namespace asmjit::x86;

    auto pop32 = [&](Gp dst) {
        a.sub(stackPtr, 4);
        a.mov(dst, dword_ptr(stackPtr));
    };

    auto push32 = [&](Gp src) {
        a.mov(dword_ptr(stackPtr), src);
        a.add(stackPtr, 4);
    };

    switch (op.code) {
        case OperationCode::LDC: {
            if (!op.arguments.empty()) {
                uint32_t constant = 0;
                for (size_t i = 0; i < 4 && i < op.arguments.size(); ++i) {
                    constant |= (op.arguments[i] << (i * 8));
                }
                
                a.mov(eax, constant);
                push32(eax);
            }
            break;
        }
        case OperationCode::LDV: {
            if (!op.arguments.empty()) {
                uint32_t varIndex = op.arguments[0];
                a.mov(eax, dword_ptr(stackBase, varIndex * 4));
                push32(eax);
            }
            break;
        }
        case OperationCode::STORE: {
            if (!op.arguments.empty()) {
                uint32_t varIndex = op.arguments[0];
                pop32(eax);
                a.mov(dword_ptr(stackBase, varIndex * 4), eax);
            }
            break;
        }
        
        case OperationCode::ADD: {
            pop32(ecx);       // pop rhs
            pop32(eax);       // pop lhs
            a.add(eax, ecx);  // eax = lhs + rhs
            push32(eax);      // push result
            break;
        }
        
        case OperationCode::SUB: {
            a.sub(stackPtr, 4);
            a.mov(eax, dword_ptr(stackPtr, -4));
            a.sub(eax, dword_ptr(stackPtr));
            a.mov(dword_ptr(stackPtr, -4), eax);
            a.sub(stackPtr, 4);
            break;
        }
        
        case OperationCode::MUL: {
            pop32(ecx);       // pop rhs
            pop32(eax);       // pop lhs
            a.imul(eax, ecx); // eax = lhs * rhs
            push32(eax);      // push result
            break;
        }
        
        case OperationCode::DIV: {
            a.mov(r10d, edx);
            a.sub(stackPtr, 4);
            a.mov(eax, dword_ptr(stackPtr, -4));
            a.cdq();
            a.idiv(dword_ptr(stackPtr));
            a.mov(dword_ptr(stackPtr, -4), eax);
            a.mov(edx, r10d);
            a.sub(stackPtr, 4);
            break;
        }
        
        case OperationCode::DUP: {
            a.mov(eax, dword_ptr(stackPtr, -4)); // EAX = top
            push32(eax);                         // push copy
            break;
        }
        case OperationCode::SWAP: {
            a.mov(eax, dword_ptr(stackPtr, -4)); // B
            a.mov(edx, dword_ptr(stackPtr, -8)); // A

            a.mov(dword_ptr(stackPtr, -8), eax); // A <- B
            a.mov(dword_ptr(stackPtr, -4), edx); // B <- A
            break;
        }
        case OperationCode::RET: {
            pop32(eax);                       // pop result
            a.mov(stackPtr, stackBase);       // reset stackPtr to stackBase
            a.mov(dword_ptr(stackBase), eax); // store result at stack[0]
            break;
        }
        case OperationCode::NEWARR: {
            uint16_t type_idx = (op.arguments[0] << 8) | op.arguments[1];

            // ─── pop size (uint32) ─────────────
            pop32(edx);                        // EDX = size
            a.mov(r8d, type_idx);              // R8D = type_idx

            // ─── call helper ──────────────────
            a.mov(rcx, heapPtr);               // RCX = heap
            a.mov(rax, (uint64_t)&JIT_NewArray);
            a.call(rax);                       // EAX = heapRef.id

            // ─── push heapRef.id onto VM stack ─
            push32(eax);                       // push uint32

            break;
        }
        case OperationCode::STELEM: {
            pop32(r9d);                        // value
            pop32(r8d);                        // index
            pop32(edx);                        // arrId

            // RCX = heapPtr
            a.mov(rcx, heapPtr);

            // ─── call helper ───────────────────────────
            a.mov(rax, (uint64_t)&JIT_StoreElem_I4);
            a.call(rax);
            break;
        }
        case OperationCode::LDELEM: {
            pop32(r8d);                        // index
            pop32(edx);                        // arrId

            // ─── call helper ───────────────────
            a.mov(rcx, heapPtr);                // RCX = heap
            a.mov(rax, (uint64_t)&JIT_LoadElem);
            a.call(rax);                        // EAX = int32 value

            // ─── push value back to VM stack ───
            push32(eax);

            break;
        }
        case OperationCode::EQ: {
            pop32(eax);   // b
            pop32(edx);   // a
            a.cmp(edx, eax);
            a.sete(al);
            a.movzx(eax, al);
            push32(eax);
            break;
        }
        case OperationCode::LT: {
            pop32(eax);   // b
            pop32(edx);   // a
            a.cmp(edx, eax);
            a.setl(al);
            a.movzx(eax, al);
            push32(eax);
            break;
        }
        case OperationCode::LEQ: {
            pop32(eax);
            pop32(edx);
            a.cmp(edx, eax);
            a.setle(al);
            a.movzx(eax, al);
            push32(eax);
            break;
        }
        case OperationCode::NEG: {
            a.mov(eax, dword_ptr(stackPtr, -4));
            a.neg(eax);
            a.mov(dword_ptr(stackPtr, -4), eax);
            break;
        }
        case OperationCode::MOD: {
            pop32(ecx);     // b
            pop32(eax);     // a
            a.cdq();        // sign extend eax -> edx
            a.idiv(ecx);    // eax = a/b, edx = a%b
            push32(edx);
            break;
        }
        case OperationCode::LOR: {
            pop32(eax);
            pop32(edx);
            a.or_(eax, edx);
            a.setne(al);
            a.movzx(eax, al);
            push32(eax);
            break;
        }
        case OperationCode::LAND: {
            pop32(eax);
            pop32(edx);
            a.and_(eax, edx);
            a.setne(al);
            a.movzx(eax, al);
            push32(eax);
            break;
        }
        case OperationCode::JMP: {
            uint16_t target =
                (op.arguments[0] << 8) | op.arguments[1];
            a.jmp(labels[target]);
            break;
        }

        case OperationCode::JZ: {
            uint16_t target =
                (op.arguments[0] << 8) | op.arguments[1];

            pop32(eax);
            a.test(eax, eax);
            a.je(labels[target]);
            break;
        }

        case OperationCode::JNZ: {
            uint16_t target =
                (op.arguments[0] << 8) | op.arguments[1];

            pop32(eax);
            a.test(eax, eax);
            a.jne(labels[target]);
            break;
        }

        default: {
            std::cerr << "Some of this operations are unable to compile" << std::endl;
            throw std::runtime_error("Some of this operations are unable to compile");
            break;
        }
    }
}

X86JitCompiler::~X86JitCompiler() {
#ifdef DEBUG_BUILD
    std::cout << "[JIT] Destructor: destroying runtime at " << runtime.get() << std::endl;
#endif
}

bool X86JitCompiler::CanCompile(czffvm::OperationCode opcode) {
    switch (opcode) {
        case OperationCode::LDC:
        case OperationCode::LDV:
        case OperationCode::STORE:
        case OperationCode::ADD:
        case OperationCode::SUB:
        case OperationCode::MUL:
        case OperationCode::DIV:
        case OperationCode::DUP:
        case OperationCode::SWAP:
        case OperationCode::RET:
        case OperationCode::LDELEM:
        case OperationCode::STELEM:
        case OperationCode::NEWARR:
        case OperationCode::EQ:
        case OperationCode::LT:
        case OperationCode::LEQ:
        case OperationCode::NEG:
        case OperationCode::MOD:
        case OperationCode::LOR:
        case OperationCode::LAND:
        case OperationCode::JMP:
        case OperationCode::JZ:
        case OperationCode::JNZ:
            return true;
    }
    return false;
}

bool X86JitCompiler::CanCompile(czffvm::Operation op) {
    return CanCompile(op.code);
}


extern "C" uint32_t JIT_NewArray(X86JitHeapHelper* heap, uint32_t size, uint16_t type) {
    HeapRef out_ref = heap->NewArray(size, type);

    return out_ref.id;
}

extern "C" void JIT_StoreElem(X86JitHeapHelper* heap, HeapRef* arr, uint32_t index, Value* value) {
    heap->StoreElem(*arr, index, value);
}

extern "C" void
JIT_StoreElem_I4(
    X86JitHeapHelper* heap,
    uint32_t arrId,
    uint32_t index,
    int32_t value
) {
    HeapRef ref{arrId};
    Value v(value);
    JIT_StoreElem(heap, &ref, index, &v);
}

extern "C" int32_t JIT_LoadElem(
    X86JitHeapHelper* heap,
    uint32_t refId,
    uint32_t index
) {
    HeapRef ref{refId};
    Value v = heap->LoadElem(ref, index);
    return std::get<int32_t>(v);
}




czffvm::HeapRef X86JitHeapHelper::NewArray(uint32_t arr_size, uint16_t type_idx) {
    const Constant& type_c =
        rda_.GetMethodArea().GetConstant(type_idx);

    std::string elem_type(type_c.data.begin(), type_c.data.end());
    std::string array_type = "[" + elem_type;

    std::vector<Value> elements(arr_size);
    return rda_.GetHeap().Allocate(array_type, std::move(elements));
}

void X86JitHeapHelper::StoreElem(czffvm::HeapRef ref, uint32_t index, czffvm::Value* value) {
    HeapObject& obj = rda_.GetHeap().Get(ref);

    if (obj.type.empty() || obj.type[0] != '[')
        throw std::runtime_error("STELEM: not array");

    if (index >= obj.fields.size())
        throw std::runtime_error("STELEM: OOB");

    obj.fields[index] = *value;
}

czffvm::Value X86JitHeapHelper::LoadElem(czffvm::HeapRef ref, uint32_t index) {
    HeapObject& obj = rda_.GetHeap().Get(ref);

    if (obj.type.empty() || obj.type[0] != '[')
        throw std::runtime_error("LDELEM: not array");

    if (index >= obj.fields.size())
        throw std::runtime_error("LDELEM: OOB");

    return obj.fields[index];
}


}  // namespace czffvm_jit
