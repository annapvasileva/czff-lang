#include "virtual_machine.hpp"

namespace czffvm {

VirtualMachine::VirtualMachine()
    : runtime_data_area_(),
      loader_(runtime_data_area_),
      interpreter_(runtime_data_area_) {}

void VirtualMachine::LoadStdlib(const std::string& path) {
    loader_.LoadStdlib(path);
}

void VirtualMachine::LoadProgram(const std::string& path) {
    loader_.LoadProgram(path);
}

void VirtualMachine::Run() {
    interpreter_.Execute();
}

} // namespace czffvm
