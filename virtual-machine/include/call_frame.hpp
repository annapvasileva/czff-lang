#pragma once

#include "common.hpp"

namespace czffvm {

struct CallFrame {
    const RuntimeFunction* function;
    std::vector<Value> operand_stack;
    std::vector<Value> locals;
    size_t pc = 0;
};

}  // namespace czffvm
