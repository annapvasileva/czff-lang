#include "runtime_data_area.hpp"

namespace czffvm {

RuntimeDataArea::RuntimeDataArea() = default;
RuntimeDataArea::~RuntimeDataArea() = default;

template<typename T, typename... Args>
T* RuntimeDataArea::Allocate(Args&&... args) {
    return new T(std::forward<Args>(args)...);
}

template RuntimeClass* RuntimeDataArea::Allocate<RuntimeClass>();
template RuntimeFunction* RuntimeDataArea::Allocate<RuntimeFunction>();

void RuntimeDataArea::RegisterClass(RuntimeClass* cls) {
    if (class_table_.count(cls->name))
        throw std::runtime_error("Duplicate class: " + cls->name);

    classes_.emplace_back(cls);
    class_table_[cls->name] = cls;
}

void RuntimeDataArea::RegisterFunction(RuntimeFunction* fn) {
    if (function_table_.count(fn->name))
        throw std::runtime_error("Duplicate function: " + fn->name);

    functions_.emplace_back(fn);
    function_table_[fn->name] = fn;
}

uint32_t RuntimeDataArea::InternConstant(const Constant& c) {
    constant_pool_.push_back(c);
    
    return static_cast<uint32_t>(constant_pool_.size() - 1);
}

RuntimeClass* RuntimeDataArea::GetClass(const std::string& name) const {
    auto it = class_table_.find(name);
    return it == class_table_.end() ? nullptr : it->second;
}

RuntimeFunction* RuntimeDataArea::GetFunction(const std::string& name) const {
    auto it = function_table_.find(name);
    return it == function_table_.end() ? nullptr : it->second;
}

const Constant& RuntimeDataArea::GetConstant(uint32_t index) const {
    if (index >= constant_pool_.size())
        throw std::out_of_range("Constant pool index out of range");
    return constant_pool_[index];
}

const std::unordered_map<std::string, RuntimeClass*>& RuntimeDataArea::Classes() const {
    return class_table_;
}

const std::unordered_map<std::string, RuntimeFunction*>& RuntimeDataArea::Functions() const {
    return function_table_;
}

}  // namespace czffvm
