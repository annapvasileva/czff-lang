#pragma once

#include <memory>
#include "common.hpp"

namespace czffvm_jit {

using namespace czffvm;

class CompiledRuntimeFunction {
public:
    template<typename FuncType>
    FuncType getFunction() const {
        return reinterpret_cast<FuncType>(GetCode());
    }

    
    ~CompiledRuntimeFunction() = default;
    virtual void* GetCode() const = 0;
    virtual size_t GetSize() const = 0;
    virtual uint16_t GetNameIndex() const = 0;
    virtual uint16_t GetReturnTypeIndex() const = 0;
    
    explicit operator bool() const { return GetCode() != nullptr; }
};

class JitCompiler {
public:
    virtual ~JitCompiler() = default;
    
    virtual bool CanCompile(czffvm::OperationCode opcode) = 0;
    virtual bool CanCompile(czffvm::Operation op) = 0;
    virtual std::unique_ptr<CompiledRuntimeFunction> CompileFunction(
        const RuntimeFunction& function) = 0;
    
    static std::unique_ptr<JitCompiler> create();
};

}  // namespace czffvm_jit
