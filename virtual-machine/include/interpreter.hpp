#pragma once

#include "runtime_data_area.hpp"

namespace czffvm {

class Interpreter {
public:
    explicit Interpreter(RuntimeDataArea& rda);

    void Execute();

private:
    RuntimeDataArea& rda_;
};

} // namespace czffvm
