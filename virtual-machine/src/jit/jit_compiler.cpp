#include "jit/jit_compiler.hpp"
#include "jit/jit_x86_64.hpp"
#include "jit/jit_dummy.hpp"

namespace czffvm_jit {

std::unique_ptr<JitCompiler> JitCompiler::create() {
#if defined(CZFF_JIT_X86_64)
    return std::make_unique<X86JitCompiler>();
#else
    return std::make_unique<DummyJitCompiler>();
#endif
    return nullptr;
}

} // namespace czffvm_jit