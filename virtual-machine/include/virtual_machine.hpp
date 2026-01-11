#pragma once

#include "runtime_data_area.hpp"
#include "class_loader.hpp"
#include "interpreter.hpp"

namespace czffvm {

class VirtualMachine {
public:
    VirtualMachine();
    VirtualMachine(uint32_t max_heap_size);
    ~VirtualMachine() = default;

    VirtualMachine(const VirtualMachine&) = delete;
    VirtualMachine& operator=(const VirtualMachine&) = delete;

    void LoadStdlib(const std::string& path);
    void LoadProgram(const std::string& path);
    void Run();

private:
    RuntimeDataArea runtime_data_area_;
    ClassLoader loader_;
    Interpreter interpreter_;
};

}  // namespace czffvm
