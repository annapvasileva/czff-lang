#include <array>
#include <vector>
#include <asmjit/x86.h>

#include "jit/jit_x86_64.hpp"
#include "common.hpp"

namespace czffvm_jit {

using namespace czffvm;

CompiledRuntimeFunction X86JitCompiler::CompileFunction(RuntimeFunction& function) {
    asmjit::CodeHolder code;
    code.init(runtime.environment());
    
    asmjit::x86::Assembler a(&code);
    
    // prologue
    a.push(asmjit::x86::rbp);
    a.mov(asmjit::x86::rbp, asmjit::x86::rsp);
    
    a.push(asmjit::x86::rbx);
    a.push(asmjit::x86::r12);
    a.push(asmjit::x86::r13);
    a.push(asmjit::x86::r14);
    a.push(asmjit::x86::r15);
    
    asmjit::x86::Gp stackBase = asmjit::x86::r13;
    a.mov(stackBase, asmjit::x86::rdi);
    
    asmjit::x86::Gp stackPtr = asmjit::x86::r12;
    a.mov(stackPtr, stackBase);
    a.add(stackPtr, function.locals_count * 4);
    
    for (const auto& op : function.code) {
        CompileOperation(a, stackPtr, stackBase, op);
    }
    
    // epilogue
    a.pop(asmjit::x86::r15);
    a.pop(asmjit::x86::r14);
    a.pop(asmjit::x86::r13);
    a.pop(asmjit::x86::r12);
    a.pop(asmjit::x86::rbx);
    a.pop(asmjit::x86::rbp);
    a.ret();
    
    void* funcPtr = nullptr;
    runtime.add(&funcPtr, &code);
    
    return CompiledRuntimeFunction(funcPtr, code.code_size());
}


}  // namespace czffvm_jit
