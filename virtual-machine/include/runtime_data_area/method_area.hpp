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

    RuntimeClass* RegisterClass(std::unique_ptr<RuntimeClass> cls);
    RuntimeFunction* RegisterFunction(std::unique_ptr<RuntimeFunction> fn);
    uint32_t RegisterConstant(const Constant& c);

    RuntimeClass* GetClass(const std::string& name) const;
    RuntimeFunction* GetFunction(const std::string& name) const;
    const Constant& GetConstant(uint16_t index) const;

    const std::unordered_map<std::string, RuntimeClass*>& Classes() const;
    const std::unordered_map<std::string, RuntimeFunction*>& Functions() const;
    const std::vector<Constant>& ConstantPool() const;

private:
    std::vector<std::unique_ptr<RuntimeClass>> classes_;
    std::vector<std::unique_ptr<RuntimeFunction>> functions_;
    std::vector<Constant> constant_pool_;

    std::unordered_map<std::string, RuntimeClass*> class_table_;
    std::unordered_map<std::string, RuntimeFunction*> function_table_;

    std::string ResolveName(uint16_t constant_index) const;
};

}