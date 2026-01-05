#pragma once

#include <vector>

#include "common.hpp"

namespace czffvm {

struct CallFrame {
    RuntimeFunction* function = nullptr;
    std::vector<Value> operand_stack;
    std::vector<Value> locals;
    size_t pc = 0;
};

}  // namespace czffvm
