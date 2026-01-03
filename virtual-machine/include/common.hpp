#pragma once

#include <cstdint>
#include <string>
#include <vector>
#include <unordered_map>

namespace czffvm {
enum class ConstantTag : uint8_t {
    U1 = 0x1,
    U2 = 0x2,
    U4 = 0x3,
    I4 = 0x4,
    STRING = 0x5,
    BOOL = 0x6,
};

struct Constant {
    ConstantTag tag;
    std::vector<uint8_t> data;
};

struct RuntimeField {
    uint16_t name_index;
    uint16_t field_descriptor_index;
};

struct RuntimeMethod {
    uint16_t name_index;
    uint16_t params_descriptor_index;
    uint16_t return_type_index;

    uint16_t max_stack;
    uint16_t locals_count;
    std::vector<uint8_t> code;
};

struct RuntimeClass {
    uint16_t name_index;
    std::vector<RuntimeField> fields;
    std::vector<RuntimeMethod> methods;
};

struct RuntimeFunction {
    uint16_t name_index;
    uint16_t params_descriptor_index;
    uint16_t return_type_index;

    uint16_t max_stack;
    uint16_t locals_count;
    std::vector<uint8_t> code;
};
}