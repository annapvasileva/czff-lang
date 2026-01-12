#pragma once

#include "jit_compiler.hpp"

namespace czffvm_jit {

class X86JitCompiler : public JitCompiler {
public:
    X86JitCompiler();
    ~X86JitCompiler() override;
    
    bool CanCompile(czffvm::OperationCode opcode) override;
    bool CanCompile(czffvm::Operation op) override;
    std::unique_ptr<CompiledRuntimeFunction> CompileFunction(
        const RuntimeFunction& function) override;

    static constexpr std::array<OperationCode, 1> kAllowedOperationsToCompile = { OperationCode::LDC };

private:
    asmjit::JitRuntime runtime;

    enum class VMReg {
        STACK_PTR,
        STACK_BASE,
        FRAME_PTR,
        PC,
        MEM_PTR,
        TEMP1,
        TEMP2,
    };
};

class X86CompiledRuntimeFunction : public CompiledRuntimeFunction {
private:
    asmjit::JitRuntime* runtime;
    void* compiledCode;
    size_t codeSize;

    CompileOperation(asmjit::x86::Assembler& a, asmjit::x86::Gp& stackPtr, asmjit::x86::Gp stackBase, const Operation& op);
    
public:
    CompiledRuntimeFunction(void* code, size_t size) 
        : compiledCode(code), codeSize(size) {}
    
    template<typename FuncType>
    FuncType getFunction() const {
        return reinterpret_cast<FuncType>(compiledCode);
    }
    
    ~CompiledRuntimeFunction() override {
        if (compiledCode) {
            runtime->release(compiledCode);
        }
    }
};

} // namespace czffvm_jit