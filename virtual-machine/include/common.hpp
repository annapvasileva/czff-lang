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
    CLASS = 0xF,
};

struct Constant {
    ConstantTag tag;
    std::string str;
};

struct RuntimeField {
    std::string name;
    std::string type;
    uint16_t offset = 0;
};

struct RuntimeMethod {
    std::string name;
    std::string params;
    std::string returnType;

    uint16_t maxStack;
    uint16_t locals;
    std::vector<uint8_t> code;
};

struct RuntimeClass {
    std::string name;
    std::vector<RuntimeField> fields;
    std::vector<RuntimeMethod> methods;
};

struct RuntimeFunction {
    std::string name;
    std::string params;
    std::string returnType;

    uint16_t maxStack;
    uint16_t locals;
    std::vector<uint8_t> code;
};
}