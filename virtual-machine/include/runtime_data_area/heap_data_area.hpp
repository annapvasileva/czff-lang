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
};

class Heap {
public:
    HeapRef Allocate(const std::string& type,
                     std::vector<Value> fields = {});

    HeapObject& Get(HeapRef ref);

    void Collect(StackDataArea& stack);

private:
    std::unordered_map<HeapRef, HeapObject> objects_;
    uint32_t next_id_ = 1;

    void MarkFromRoots(StackDataArea& stack);
    void Mark(const HeapRef& ref);
    void Sweep();
};

}  // namespace czffvm
