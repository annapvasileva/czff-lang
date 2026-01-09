#pragma once

#include <vector>
#include <stdexcept>

#include "call_frame.hpp"

namespace czffvm {

/**
 * Stack Data Area (VM Stack)
 *
 * Stores call frames for bytecode execution.
 */
class StackDataArea {
public:
    void PushFrame(RuntimeFunction* fn);
    void PopFrame();
    const std::vector<CallFrame>& GetFrames() const;

    CallFrame& CurrentFrame();
    bool Empty() const;

private:
    std::vector<CallFrame> frames_;
};

} // namespace czffvm
