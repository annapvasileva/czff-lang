#include "method_area.hpp"

namespace czffvm {

uint16_t MethodArea::RegisterConstant(const Constant& constant) {
    constant_pool_.push_back(constant);

    return static_cast<uint16_t>(constant_pool_.size() - 1);
}

const Constant& MethodArea::GetConstant(uint16_t index) const {
    if (index >= constant_pool_.size()) {
        throw std::out_of_range("MethodArea: constant pool index out of range");
    }

    return constant_pool_[index];
}

uint16_t MethodArea::RegisterClass(RuntimeClass* cls) {
    if (!cls) {
        throw std::invalid_argument("MethodArea: null RuntimeClass");
    }

    classes_.push_back(std::move(cls));

    return static_cast<uint16_t>(classes_.size() - 1);
}

uint16_t MethodArea::RegisterFunction(RuntimeFunction* fn) {
    if (!fn) {
        throw std::invalid_argument("MethodArea: null RuntimeFunction");
    }

    functions_.push_back(fn);

    return static_cast<uint16_t>(functions_.size() - 1);
}

const RuntimeClass* MethodArea::GetClass(uint16_t index) const {
    if (index >= classes_.size()) {
        throw std::out_of_range("MethodArea: class pool index out of range");
    }

    return classes_[index];
}

const RuntimeFunction* MethodArea::GetFunction(uint16_t index) const {
    if (index >= functions_.size()) {
        throw std::out_of_range("MethodArea: function pool index out of range");
    }

    return functions_[index];
}

const std::vector<RuntimeClass*>& MethodArea::Classes() const  {
    return classes_;
}

const std::vector<RuntimeFunction*>& MethodArea::Functions() const {
    return functions_;
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
