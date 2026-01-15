#pragma once

#include "runtime_data_area.hpp"
#include "jit/jit_x86_64.hpp"

namespace czffvm {

class Interpreter {
public:
    explicit Interpreter(RuntimeDataArea& rda);

    void Execute(RuntimeFunction* entry);

    void SetJitCompiler(std::unique_ptr<czffvm_jit::JitCompiler> jit);

    void JitCompile(RuntimeFunction* function);
    void ExecuteJitFunction(RuntimeFunction* function, CallFrame& caller_frame, std::vector<Value>& args);
    bool CanCompile(const RuntimeFunction* function);

private:
    RuntimeDataArea& rda_;
    std::unique_ptr<czffvm_jit::JitCompiler> jit_compiler_;
    std::unique_ptr<czffvm_jit::X86JitHeapHelper> heapHelper_;
};

} // namespace czffvm
