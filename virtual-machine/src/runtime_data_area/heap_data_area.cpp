#include "heap_data_area.hpp"

namespace czffvm {

Heap::Heap(StackDataArea& stack, uint32_t max_heap_size)
    : stack_(stack), max_heap_size_(max_heap_size) {}

HeapRef Heap::Allocate(const std::string& type,
                       std::vector<Value> fields) {

    if (next_id_ > max_heap_size_) {
        Collect(0);
    }

    HeapRef ref{ next_id_++ };

    objects_.emplace(ref, HeapObject{
        .marked = false,
        .type = type,
        .fields = std::move(fields)
    });

    return ref;
}

HeapObject& Heap::Get(HeapRef ref) {
    auto it = objects_.find(ref);
    if (it == objects_.end())
        throw std::runtime_error("Invalid heap reference");

    return it->second;
}

void Heap::Collect(size_t generation) {
    MarkFromRoots();
    Sweep(generation);
}

void Heap::MarkFromRoots() {
    for (auto& frame : stack_.GetFrames()) {
        for (auto& v : frame.locals)
            if (auto* r = std::get_if<HeapRef>(&v))
                Mark(*r);

        for (auto& v : frame.operand_stack)
            if (auto* r = std::get_if<HeapRef>(&v))
                Mark(*r);
    }
}

void Heap::Mark(const HeapRef& ref) {
    auto it = objects_.find(ref);
    if (it == objects_.end()) return;

    auto& obj = it->second;
    if (obj.marked) return;

    obj.marked = true;

    for (auto& f : obj.fields)
        if (auto* r = std::get_if<HeapRef>(&f))
            Mark(*r);
}

void Heap::Sweep(size_t generation) {
    for (auto it = objects_.begin(); it != objects_.end();) {
        HeapObject& obj = it->second;
        if (obj.generation != generation) {
            ++it;
            continue;
        }

        if (!obj.marked) {
            it = objects_.erase(it);
        } else {
            if (obj.generation < 2)
                obj.generation++;
            obj.marked = false;
            ++it;
        }
    }
}

}
