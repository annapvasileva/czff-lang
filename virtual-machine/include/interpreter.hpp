#pragma once

#include "runtime_data_area.hpp"
#include "call_frame.hpp"

namespace czffvm {

class Interpreter {
public:
    explicit Interpreter(RuntimeDataArea& rda)
        : rda_(rda) {}

    void Execute();

private:
    RuntimeDataArea& rda_;
    std::vector<CallFrame> call_stack_;

    int32_t ReadI4(const Constant& c);
};


}  // namespace czffvm
