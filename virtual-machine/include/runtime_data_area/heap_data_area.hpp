#pragma once

#include <vector>
#include <optional>

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
    Heap(StackDataArea& stack, uint32_t max_heap_size_in_bytes);
    HeapRef Allocate(const std::string& type,
                     std::vector<Value>&& fields);
    HeapRef Allocate(const std::string& type,
                     std::vector<Value>& fields);

    HeapObject& Get(HeapRef ref);

    void Collect();

private:
    std::vector<std::optional<HeapObject>> objects_;
    std::vector<uint32_t> free_list_;
    uint32_t next_id_ = 1;
    StackDataArea& stack_;
    uint64_t used_bytes_ = 0;
    uint64_t max_heap_size_in_bytes_;

    void MarkFromRoots();
    void Mark(const HeapRef& ref);
    void Sweep();
    size_t EstimateSize(const std::string& type, const std::vector<Value>& fields);
    size_t EstimateSize(const std::string& type, const std::vector<Value>&& fields);
};

}  // namespace czffvm
