#include "runtime_data_area.hpp"

namespace czffvm {

RuntimeDataArea::RuntimeDataArea() = default;
RuntimeDataArea::~RuntimeDataArea() = default;

MethodArea& RuntimeDataArea::GetMethodArea() {
    return method_area_;
}

}  // namespace czffvm
