#pragma once

#include <cstdint>
#include <memory>

#include "common.hpp"

namespace czffvm {

class RuntimeDataArea {
public:
    RuntimeDataArea();
    ~RuntimeDataArea();

    template<typename T, typename... Args>
    T* Allocate(Args&&... args);

    void RegisterClass(RuntimeClass* cls);
    void RegisterFunction(RuntimeFunction* fn);
    uint32_t RegisterConstant(const Constant& c);

    RuntimeClass* GetClass(const std::string& name) const;
    RuntimeFunction* GetFunction(const std::string& name) const;
    const Constant& GetConstant(uint16_t index) const;

    const std::unordered_map<std::string, RuntimeClass*>& Classes() const;
    const std::unordered_map<std::string, RuntimeFunction*>& Functions() const;
    const std::vector<Constant>& Constants() const;

private:
    std::vector<std::unique_ptr<RuntimeClass>> classes_;
    std::vector<std::unique_ptr<RuntimeFunction>> functions_;
    std::vector<Constant> constant_pool_;

    std::unordered_map<std::string, RuntimeClass*> class_table_;
    std::unordered_map<std::string, RuntimeFunction*> function_table_;
};

}  // namespace czffvm
