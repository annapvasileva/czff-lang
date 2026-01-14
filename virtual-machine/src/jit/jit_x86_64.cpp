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

std::unique_ptr<CompiledRuntimeFunction> X86JitCompiler::CompileFunction(const RuntimeFunction& function) {
    
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
    
#ifdef DEBUG_BUILD
    std::cout << "[JIT] Compiling operations..." << std::endl;
#endif

    for (const auto& op : function.code) {
        std::cout << "[JIT-OP] Code: 0x" << std::hex << (uint16_t)op.code << std::dec 
                  << ", args: " << op.arguments.size() << std::endl;
        
        CompileOperation(a, stackPtr, stackBase, heapPtr, op);
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

void X86JitCompiler::CompileOperation(asmjit::x86::Assembler& a, asmjit::x86::Gp& stackPtr, asmjit::x86::Gp stackBase, asmjit::x86::Gp heapPtr, const Operation& op) {
    using namespace asmjit::x86;

    switch (op.code) {
        case OperationCode::LDC: {
            if (!op.arguments.empty()) {
                uint32_t constant = 0;
                for (size_t i = 0; i < 4 && i < op.arguments.size(); ++i) {
                    constant |= (op.arguments[i] << (i * 8));
                }
                
                a.mov(eax, constant);
                a.mov(dword_ptr(stackPtr), eax);
                a.add(stackPtr, 4);
            }
            break;
        }
        case OperationCode::LDV: {
            if (!op.arguments.empty()) {
                uint32_t varIndex = op.arguments[0];
                a.mov(eax, dword_ptr(stackBase, varIndex * 4));
                a.mov(dword_ptr(stackPtr), eax);
                a.add(stackPtr, 4);
            }
            break;
        }
        case OperationCode::STORE: {
            if (!op.arguments.empty()) {
                uint32_t varIndex = op.arguments[0];
                a.sub(stackPtr, 4);
                a.mov(eax, dword_ptr(stackPtr));
                a.mov(dword_ptr(stackBase, varIndex * 4), eax);
            }
            break;
        }
        
        case OperationCode::ADD: {
            // pop rhs
            a.sub(stackPtr, 4);
            a.mov(ecx, dword_ptr(stackPtr));

            // pop lhs
            a.sub(stackPtr, 4);
            a.mov(eax, dword_ptr(stackPtr));

            // eax = lhs + rhs
            a.add(eax, ecx);

            // push result
            a.mov(dword_ptr(stackPtr), eax);
            a.add(stackPtr, 4);
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
            a.sub(stackPtr, 4);
            a.mov(ecx, dword_ptr(stackPtr));

            a.sub(stackPtr, 4);
            a.mov(eax, dword_ptr(stackPtr));

            a.imul(eax, ecx);

            a.mov(dword_ptr(stackPtr), eax);
            a.add(stackPtr, 4);
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
            a.mov(dword_ptr(stackPtr), eax);     // push copy
            a.add(stackPtr, 4);
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
            // pop result
            a.sub(stackPtr, 4);
            a.mov(eax, dword_ptr(stackPtr));

            // reset stackPtr to stackBase
            a.mov(stackPtr, stackBase);

            // store result at stack[0]
            a.mov(dword_ptr(stackBase), eax);
            break;
        }
        case OperationCode::NEWARR: {
            // ─── pop size (uint32) ─────────────
            a.sub(stackPtr, 4);
            a.mov(edx, dword_ptr(stackPtr));   // EDX = size

            // ─── type_idx (uint16 -> uint32) ──
            uint16_t type_idx = (op.arguments[0] << 8) | op.arguments[1];
            a.mov(r8d, type_idx);              // R8D = type

            // ─── call helper ──────────────────
            a.mov(rcx, heapPtr);               // RCX = heap
            a.mov(rax, (uint64_t)&JIT_NewArray);
            a.call(rax);                       // EAX = heapRef.id

            // ─── push heapRef.id onto VM stack ─
            a.mov(dword_ptr(stackPtr), eax);   // store id
            a.add(stackPtr, 4);                // push uint32

            break;
        }
        case OperationCode::STELEM: {

            // ─── pop VALUE (int32) ─────────────────────
            a.sub(stackPtr, 4);
            a.mov(r9d, dword_ptr(stackPtr));   // value

            // ─── pop INDEX (int32) ─────────────────────
            a.sub(stackPtr, 4);
            a.mov(r8d, dword_ptr(stackPtr));   // index

            // ─── pop HeapRef.id (int32) ─────────────────
            a.sub(stackPtr, 4);
            a.mov(edx, dword_ptr(stackPtr));   // arrId

            // RCX = heapPtr
            a.mov(rcx, heapPtr);

            // ─── call helper ───────────────────────────
            a.mov(rax, (uint64_t)&JIT_StoreElem_I4);
            a.call(rax);
            break;
        }
        case OperationCode::LDELEM: {
            using namespace asmjit::x86;

            // резервируем место для Value
            a.sub(stackPtr, 8);
            a.lea(r9, ptr(stackPtr));           // Value* out

            // pop index
            a.sub(stackPtr, 4);
            a.mov(r8d, dword_ptr(stackPtr));

            // pop array ref
            a.sub(stackPtr, 8);
            a.lea(rdx, ptr(stackPtr));          // HeapRef*

            // RCX = heapPtr
            a.mov(rax, (uint64_t)JIT_LoadElem);
            a.mov(rcx, heapPtr);
            a.call(rax);

            // результат уже записан в *r9
            a.add(stackPtr, 8); // push Value* на стек
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
            return true;
    }
    return false;
}

bool X86JitCompiler::CanCompile(czffvm::Operation op) {
    return CanCompile(op.code);
}


void JIT_NewArray(X86JitHeapHelper* heap, uint32_t size, uint16_t type, HeapRef* out_ref) {
    std::cout << "[NewArray] this=" << heap
            << " heap=" << &heap->rda_
            << " heap=" << &heap->rda_.GetHeap()
            << " index=" << size
            << " type=" << type
            << std::endl;
    *out_ref = heap->NewArray(size, type);
    std::cout << out_ref
            << std::endl;
}

void JIT_StoreElem(X86JitHeapHelper* heap, HeapRef* arr, uint32_t index, Value* value) {
    std::cout << "[StoreElem] this=" << heap
            << " heap=" << &heap->rda_
            << " heap=" << &heap->rda_.GetHeap()
            << " ref=" << arr->id
            << " ref=" << arr
            << " index=" << index
            << " value.index=" << value->index()
            << std::endl;
    heap->StoreElem(*arr, index, value);
}

void JIT_LoadElem(X86JitHeapHelper* heap, HeapRef* arr, uint32_t index, Value* out_value) {
    assert(out_value != nullptr);
    *out_value = heap->LoadElem(*arr, index);
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
