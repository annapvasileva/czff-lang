#pragma once

#include <cstdint>
#include <memory>

#include "common.hpp"
#include "method_area.hpp"
#include "stack_data_area.hpp"
#include "heap_data_area.hpp"

namespace czffvm {

class RuntimeDataArea {
public:
    RuntimeDataArea(uint32_t max_heap_size_in_bytes = DEFAULT_MAX_HEAP_SIZE_IN_BYTES);
    ~RuntimeDataArea();

    RuntimeDataArea(const RuntimeDataArea&) = delete;
    RuntimeDataArea& operator=(const RuntimeDataArea&) = delete;

    MethodArea& GetMethodArea();
    StackDataArea& GetStack();
    Heap& GetHeap();

private:
    StackDataArea stack_;
    MethodArea method_area_;
    Heap heap_;
};

}  // namespace czffvm
