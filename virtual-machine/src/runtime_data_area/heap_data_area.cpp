#include "heap_data_area.hpp"

namespace czffvm {

HeapRef Heap::Allocate(const std::string& type,
                       std::vector<Value> fields) {

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

void Heap::Collect(StackDataArea& stack) {
    MarkFromRoots(stack);
    Sweep();
}

void Heap::MarkFromRoots(StackDataArea& stack) {
    for (auto& frame : stack.GetFrames()) {
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

void Heap::Sweep() {
    for (auto it = objects_.begin(); it != objects_.end();) {
        if (!it->second.marked)
            it = objects_.erase(it);
        else {
            it->second.marked = false;
            ++it;
        }
    }
}

}
