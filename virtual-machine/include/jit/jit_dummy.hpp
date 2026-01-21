#pragma once

#include "jit_compiler.hpp"

namespace czffvm_jit {

class DummyJitCompiler : public JitCompiler {
public:
    DummyJitCompiler();
    ~DummyJitCompiler() override;
    
    bool CanCompile(czffvm::OperationCode opcode) override;
    bool CanCompile(czffvm::Operation op) override;
    std::unique_ptr<CompiledRuntimeFunction> CompileFunction(
        const czffvm::RuntimeFunction& function,
        czffvm::RuntimeDataArea& rda) override;
};

} // namespace czffvm_jit