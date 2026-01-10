#pragma once

#include <unordered_map>
#include <vector>

#include "common.hpp"
#include "stack_data_area.hpp"

namespace czffvm {

struct HeapObject {
    bool marked = false;
    std::string type;
    std::vector<Value> fields;
    uint8_t generation = 0;
};

class Heap {
public:
    Heap(StackDataArea& stack, uint32_t max_heap_size);
    HeapRef Allocate(const std::string& type,
                     std::vector<Value> fields = {});

    HeapObject& Get(HeapRef ref);

    void Collect(size_t generation = 0);

private:
    std::unordered_map<HeapRef, HeapObject> objects_;
    uint32_t next_id_ = 1;
    StackDataArea& stack_;
    uint32_t max_heap_size_;

    void MarkFromRoots();
    void Mark(const HeapRef& ref);
    void Sweep(size_t generation = 0);
};

}  // namespace czffvm
