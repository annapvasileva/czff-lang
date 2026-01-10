#pragma once

#include <memory>

#include "common.hpp"

namespace czffvm {
class MethodArea {
public:
    MethodArea() = default;
    ~MethodArea() = default;

    MethodArea(const MethodArea&) = delete;
    MethodArea& operator=(const MethodArea&) = delete;

    uint16_t RegisterClass(RuntimeClass* cls);
    uint16_t RegisterFunction(RuntimeFunction* fn);
    uint16_t RegisterConstant(const Constant& c);

    const RuntimeClass* GetClass(uint16_t) const;
    const RuntimeFunction* GetFunction(uint16_t index) const;
    const Constant& GetConstant(uint16_t index) const;

    const std::vector<RuntimeClass*>& Classes() const;
    const std::vector<RuntimeFunction*>& Functions() const;
    const std::vector<Constant>& ConstantPool() const;

private:
    std::vector<RuntimeClass*> classes_;
    std::vector<RuntimeFunction*> functions_;
    std::vector<Constant> constant_pool_;

    std::string ResolveName(uint16_t constant_index) const;
};
}
