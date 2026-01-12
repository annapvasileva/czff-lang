#pragma once

#include <memory>
#include "common.hpp"

namespace czffvm_jit {

using namespace czffvm;

class CompiledRuntimeFunction {
    template<typename FuncType>
    FuncType getFunction() const;
    
    virtual ~CompiledRuntimeFunction() = default;
};

class JitCompiler {
    virtual ~JitCompiler() = default;
    
    virtual bool CanCompile(czffvm::OperationCode opcode) = 0;
    virtual bool CanCompile(czffvm::Operation op) = 0;
    virtual std::unique_ptr<CompiledRuntimeFunction> CompileFunction(
        const RuntimeFunction& function) = 0;
    
    static std::unique_ptr<JitCompiler> create();
};

}  // namespace czffvm_jit
