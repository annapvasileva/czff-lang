#pragma once

#include "class_loader.hpp"

namespace czffvm {

class VirtualMachine {
public:
    VirtualMachine(ClassLoader& class_loader);
private:
    ClassLoader& class_loader_;
};

}  // namespace czffvm
