#include "heap_data_area.hpp"

namespace czffvm {

Heap::Heap(StackDataArea& stack, uint32_t max_heap_size_in_bytes)
    : stack_(stack), max_heap_size_in_bytes_(max_heap_size_in_bytes) {}

HeapRef Heap::Allocate(const std::string& type,
                       std::vector<Value>&& fields) {
    size_t approximate_size = EstimateSize(type, fields);
    if (used_bytes_ + approximate_size > max_heap_size_in_bytes_) {
        Collect();
    }

    if (used_bytes_ + approximate_size > max_heap_size_in_bytes_) {
        throw std::runtime_error("Heap memory limit exceeded");
    }

    if (!free_list_.empty()) {
        uint32_t id = free_list_.back();
        free_list_.pop_back();
        objects_[id] = HeapObject{
            .marked = false,
            .type = type,
            .fields = std::move(fields)
        };
        used_bytes_ += approximate_size;

        return HeapRef(id);
    }

    HeapRef ref{ objects_.size() };

    objects_.push_back(HeapObject{
        .marked = false,
        .type = type,
        .fields = std::move(fields)
    });
    used_bytes_ += approximate_size;

    return ref;
}

HeapRef Heap::Allocate(const std::string& type,
                       std::vector<Value>& fields) {
    size_t approximate_size = EstimateSize(type, fields);
    if (used_bytes_ + approximate_size > max_heap_size_in_bytes_) {
        Collect();
    }

    if (used_bytes_ + approximate_size > max_heap_size_in_bytes_) {
        throw std::runtime_error("Heap memory limit exceeded");
    }

    if (!free_list_.empty()) {
        uint32_t id = free_list_.back();
        free_list_.pop_back();
        objects_[id] = HeapObject{
            .marked = false,
            .type = type,
            .fields = std::move(fields)
        };
        used_bytes_ += approximate_size;

        return HeapRef(id);
    }

    HeapRef ref{ objects_.size() };

    objects_.push_back(HeapObject{
        .marked = false,
        .type = type,
        .fields = std::move(fields)
    });
    used_bytes_ += approximate_size;

    return ref;
}

HeapObject& Heap::Get(HeapRef ref) {
    if (ref.id >= objects_.size() || !objects_[ref.id]) {
        throw std::runtime_error("Invalid heap reference");
    }

    return objects_[ref.id].value();
}

void Heap::Collect() {
    MarkFromRoots();
    Sweep();
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
    if (ref.id >= objects_.size() || !objects_[ref.id]) return;

    auto& obj = objects_[ref.id];
    if (obj->marked) return;

    obj->marked = true;

    for (auto& f : obj->fields)
        if (auto* r = std::get_if<HeapRef>(&f))
            Mark(*r);
}

void Heap::Sweep() {
    for (uint32_t i = 0; i < objects_.size(); ++i) {
        auto& obj = objects_[i];
        if (!obj) continue;

        if (!obj->marked) {
            used_bytes_ -= EstimateSize(obj->type, obj->fields);
            obj.reset();
            free_list_.push_back(i);
        } else {
            obj->marked=false;
        }
    }
}

size_t Heap::EstimateSize(const std::string& type, const std::vector<Value>& fields) {
    size_t size = sizeof(HeapObject);

    size += type.size();

    size += fields.size() * sizeof(Value);

    for (auto& v : fields) {
        if (auto* s = std::get_if<std::string>(&v))
            size += s->size();
    }

    return size;
}

size_t Heap::EstimateSize(const std::string& type, const std::vector<Value>&& fields) {
    size_t size = sizeof(HeapObject);

    size += type.size();

    size += fields.size() * sizeof(Value);

    for (auto& v : fields) {
        if (auto* s = std::get_if<std::string>(&v))
            size += s->size();
    }

    return size;
}


}
