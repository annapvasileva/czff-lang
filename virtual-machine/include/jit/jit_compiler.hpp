#pragma once

#include <memory>
#include "common.hpp"

namespace czffvm {
    enum class OperationCode : uint16_t;
    struct Operation;
    struct RuntimeFunction;
    
    struct HeapRef;
    
    using StringRef = std::shared_ptr<std::string>;

    using Value = std::variant<
        int8_t,                     // I1
        uint8_t,                    // U1
        int16_t,                    // I2
        uint16_t,                   // U2
        uint32_t,                   // U4
        int32_t,                    // I4
        StringRef,                  // STRING
        uint64_t,                   // U8
        int64_t,                    // I8
        stdint128::int128_t,        // I16
        stdint128::uint128_t,       // U16
        bool,                       // BOOL
        HeapRef
    >;

    class RuntimeDataArea;

    size_t CountParams(const std::string& s);
}

namespace czffvm_jit {

class CompiledRuntimeFunction {
public:
    template<typename FuncType>
    FuncType getFunction() const {
        return reinterpret_cast<FuncType>(GetCode());
    }

    
    ~CompiledRuntimeFunction() = default;
    virtual void* GetCode() const = 0;
    virtual size_t GetSize() const = 0;
    virtual size_t GetArgumentCount() const = 0;
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
        const czffvm::RuntimeFunction& function,
        czffvm::RuntimeDataArea& rda
    ) = 0;
    
    static std::unique_ptr<JitCompiler> create();
};

}  // namespace czffvm_jit
