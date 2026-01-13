#include "runtime_data_area.hpp"

namespace czffvm {

RuntimeDataArea::RuntimeDataArea(uint32_t max_heap_size_in_bytes)
    : stack_(),
    method_area_(),
    heap_(Heap(stack_, max_heap_size_in_bytes)) { }

RuntimeDataArea::~RuntimeDataArea() = default;

MethodArea& RuntimeDataArea::GetMethodArea() {
    return method_area_;
}

StackDataArea& RuntimeDataArea::GetStack() {
    return stack_;
}

Heap& RuntimeDataArea::GetHeap() {
    return heap_;
}

}  // namespace czffvm
