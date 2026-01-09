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
    RuntimeDataArea();
    ~RuntimeDataArea();

    RuntimeDataArea(const RuntimeDataArea&) = delete;
    RuntimeDataArea& operator=(const RuntimeDataArea&) = delete;

    MethodArea& GetMethodArea();
    StackDataArea& GetStack();
    Heap& GetHeap();

private:
    MethodArea method_area_;
    StackDataArea stack_;
    Heap heap_;
};

}  // namespace czffvm
