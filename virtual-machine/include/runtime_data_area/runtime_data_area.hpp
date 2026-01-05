#pragma once

#include <cstdint>
#include <memory>

#include "common.hpp"
#include "method_area.hpp"

namespace czffvm {

class RuntimeDataArea {
public:
    RuntimeDataArea();
    ~RuntimeDataArea();

    RuntimeDataArea(const RuntimeDataArea&) = delete;
    RuntimeDataArea& operator=(const RuntimeDataArea&) = delete;

    MethodArea& GetMethodArea();

private:
    MethodArea method_area_;
};

}  // namespace czffvm
