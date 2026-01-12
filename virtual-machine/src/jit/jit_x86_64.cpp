#include <array>
#include <vector>
#include <asmjit/x86.h>

#include "jit/jit_compiler.hpp"
#include "common.hpp"

namespace czffvm_jit {

using namespace czffvm;

class CompiledRuntimeFunction {
private:
    asmjit::x86::Gp stackPtr;
    asmjit::x86::Gp stackBase;
    asmjit::x86::Gp framePtr;
    asmjit::x86::Gp pc;
    asmjit::x86::Gp memPtr;
    
    asmjit::x86::Assembler* a;
    std::vector<asmjit::v1_21::Label> labels;

public:
    CompiledRuntimeFunction(
        asmjit::x86::Gp stackPtr,
        asmjit::x86::Gp stackBase,
        asmjit::x86::Gp framePtr,
        asmjit::x86::Gp pc,
        asmjit::x86::Gp memPtr,
        asmjit::x86::Assembler* a,
        std::vector<asmjit::v1_21::Label> labels
    ) : stackPtr(stackPtr), stackBase(stackBase), framePtr(framePtr), pc(pc), memPtr(memPtr), a(a), labels(labels) {}
};

class CompiledRuntimeFunctionBuilder {
public:
    asmjit::x86::Gp stackPtr;
    asmjit::x86::Gp stackBase;
    asmjit::x86::Gp framePtr;
    asmjit::x86::Gp pc;
    asmjit::x86::Gp memPtr;
    
    asmjit::x86::Assembler* a;
    std::vector<asmjit::v1_21::Label> labels;

    CompiledRuntimeFunction Build() {
        return CompiledRuntimeFunction {
            stackPtr, stackBase, framePtr, pc, memPtr, a, std::move(labels)
        };
    }
};

class X86JitCompiler : public JitCompiler {
    // TODO
};

}  // namespace czffvm_jit
