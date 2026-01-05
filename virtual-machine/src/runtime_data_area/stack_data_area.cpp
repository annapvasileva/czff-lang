#include "stack_data_area.hpp"

namespace czffvm {

void StackDataArea::PushFrame(RuntimeFunction* fn) {
    CallFrame frame;
    frame.function = fn;
    frame.pc = 0;
    frame.locals.resize(fn->locals_count);
    frame.operand_stack.reserve(fn->max_stack);

    frames_.push_back(std::move(frame));
}

void StackDataArea::PopFrame() {
    if (frames_.empty()) {
        throw std::runtime_error("Stack underflow");
    }
    frames_.pop_back();
}

CallFrame& StackDataArea::CurrentFrame() {
    if (frames_.empty()) {
        throw std::runtime_error("No active frame");
    }
    return frames_.back();
}

bool StackDataArea::Empty() const {
    return frames_.empty();
}

} // namespace czffvm
