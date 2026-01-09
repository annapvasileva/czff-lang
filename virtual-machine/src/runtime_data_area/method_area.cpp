#include "method_area.hpp"

namespace czffvm {

uint32_t MethodArea::RegisterConstant(const Constant& constant) {
    constant_pool_.push_back(constant);

    return static_cast<uint32_t>(constant_pool_.size() - 1);
}

const Constant& MethodArea::GetConstant(uint16_t index) const {
    if (index >= constant_pool_.size()) {
        throw std::out_of_range("MethodArea: constant pool index out of range");
    }

    return constant_pool_[index];
}

RuntimeClass* MethodArea::RegisterClass(std::unique_ptr<RuntimeClass> cls) {
    if (!cls) {
        throw std::invalid_argument("MethodArea: null RuntimeClass");
    }

    std::string name = ResolveName(cls->name_index);

    if (class_table_.count(name)) {
        throw std::runtime_error("Duplicate class: " + name);
    }

    RuntimeClass* raw = cls.get();
    classes_.push_back(std::move(cls));
    class_table_[name] = raw;

    return raw;
}

RuntimeFunction* MethodArea::RegisterFunction(std::unique_ptr<RuntimeFunction> fn) {
    if (!fn) {
        throw std::invalid_argument("MethodArea: null RuntimeFunction");
    }

    std::string name = ResolveName(fn->name_index);

    if (function_table_.count(name)) {
        throw std::runtime_error("Duplicate function: " + name);
    }

    RuntimeFunction* raw = fn.get();
    functions_.push_back(std::move(fn));
    function_table_[name] = raw;

    return raw;
}

RuntimeClass* MethodArea::GetClass(const std::string& name) const {
    auto it = class_table_.find(name);

    return it == class_table_.end() ? nullptr : it->second;
}

RuntimeFunction* MethodArea::GetFunction(const std::string& name) const {
    auto it = function_table_.find(name);

    return it == function_table_.end() ? nullptr : it->second;
}

const std::unordered_map<std::string, RuntimeClass*>& MethodArea::Classes() const {
    return class_table_;
}

const std::unordered_map<std::string, RuntimeFunction*>& MethodArea::Functions() const {
    return function_table_;
}

const std::vector<Constant>& MethodArea::ConstantPool() const {
    return constant_pool_;
}

std::string MethodArea::ResolveName(uint16_t constant_index) const {
    const Constant& c = GetConstant(constant_index);

    if (c.tag != ConstantTag::STRING) {
        throw std::runtime_error("MethodArea: name constant is not STRING");
    }

    return std::string(c.data.begin(), c.data.end());
}

} // namespace czffvm
