#include "virtual_machine.hpp"

namespace czffvm {

VirtualMachine::VirtualMachine()
    : runtime_data_area_(),
      loader_(runtime_data_area_),
      interpreter_(runtime_data_area_) {}

VirtualMachine::VirtualMachine(uint32_t max_heap_size_in_kib)
    : runtime_data_area_(max_heap_size_in_kib),
      loader_(runtime_data_area_),
      interpreter_(runtime_data_area_) {}

void VirtualMachine::LoadStdlib(const std::string& path) {
    loader_.LoadStdlib(path);
}

void VirtualMachine::LoadProgram(const std::string& path) {
    loader_.LoadProgram(path);
}

void VirtualMachine::Run() {
    interpreter_.Execute(loader_.EntryPoint());
}

void VirtualMachine::EnableJIT() {
#ifdef CZFF_JIT_DISABLED
    throw std::runtime_error("JIT is disabled on this platform");
#else
    interpreter_.SetJitCompiler(czffvm_jit::JitCompiler::create());
#endif
}

} // namespace czffvm
