#pragma once

#include <memory>
#include <asmjit/x86.h>
#include "jit_compiler.hpp"
#include "common.hpp"
#include "runtime_data_area/runtime_data_area.hpp"

namespace czffvm_jit {

class X86CompiledRuntimeFunction : public CompiledRuntimeFunction {
private:
    std::weak_ptr<asmjit::JitRuntime> runtime;
    void* compiled_code;
    size_t code_size;
    uint16_t name_index;
    uint16_t return_type_index;
    uint16_t max_stack;
public:
    X86CompiledRuntimeFunction(void* code, size_t size, std::weak_ptr<asmjit::JitRuntime> runtime_) 
        : compiled_code(code), code_size(size), runtime(runtime_) {}
    
    ~X86CompiledRuntimeFunction() {
#ifdef DEBUG_BUILD
        std::cout << "[JIT-FUNC] Destroying, runtime use_count=" 
                  << runtime.use_count() << std::endl;
#endif
        if (compiled_code) {
            if (auto rt = runtime.lock()) {
                rt->release(compiled_code);
            }
        }
    };
    
    void* GetCode() const override { return compiled_code; }
    size_t GetSize() const override { return code_size; }
    
    uint16_t GetNameIndex() const override { return name_index; }
    uint16_t GetReturnTypeIndex() const override { return return_type_index; }
};

class X86JitHeapHelper {
private:
public:
    czffvm::RuntimeDataArea& rda_;
    X86JitHeapHelper(czffvm::RuntimeDataArea& rda) : rda_(rda) {}

    czffvm::HeapRef NewArray(
        uint32_t size,
        uint16_t type_idx
    );

    void StoreElem(
        czffvm::HeapRef ref,
        uint32_t index,
        czffvm::Value* value
    );

    czffvm::Value LoadElem(
        czffvm::HeapRef ref,
        uint32_t index
    );
};

class X86JitCompiler : public JitCompiler {
public:
    X86JitCompiler();
    ~X86JitCompiler() override;
    
    bool CanCompile(czffvm::OperationCode opcode) override;
    bool CanCompile(czffvm::Operation op) override;
    std::unique_ptr<CompiledRuntimeFunction> CompileFunction(
        const czffvm::RuntimeFunction& function,
        czffvm::RuntimeDataArea& rda) override;
private:
    std::shared_ptr<asmjit::JitRuntime> runtime;

    enum class VMReg {
        STACK_PTR,
        STACK_BASE,
        FRAME_PTR,
        PC,
        MEM_PTR,
        TEMP1,
        TEMP2,
    };

    void CompileOperation(
        asmjit::x86::Assembler& a, 
        asmjit::x86::Gp& stackPtr, 
        asmjit::x86::Gp stackBase, 
        asmjit::x86::Gp heapPtr,
        const czffvm::Operation& op,
        const std::vector<asmjit::v1_21::Label>& labels,
        czffvm::RuntimeDataArea& rda
    );
};

extern "C" uint32_t
JIT_NewArray(
    X86JitHeapHelper* heap,
    uint32_t size,
    uint16_t type
);

extern "C" void
JIT_StoreElem(X86JitHeapHelper* heap,
              czffvm::HeapRef* arr,
              uint32_t index,
              czffvm::Value* value);

extern "C" void
JIT_StoreElem_I4(
    X86JitHeapHelper* heap,
    uint32_t arrId,
    uint32_t index,
    int32_t value
);

extern "C" int32_t
JIT_LoadElem(
    X86JitHeapHelper* heap,
    uint32_t refId,
    uint32_t index
);

} // namespace czffvm_jit