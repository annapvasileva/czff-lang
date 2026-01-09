#pragma once

#include <stdexcept>
#include <cstdint>
#include <string>
#include <vector>
#include <unordered_map>
#include <variant>

namespace czffvm {
enum class OperationCode : uint16_t {
    LDC = 0x0001,
    DUP = 0x0002,
    SWAP = 0x0003,
    STORE = 0x0004,
    LDV = 0x0005,
    ADD = 0x0006,
    PRINT = 0x0007,
    RET = 0x0008,
    HALT = 0x0009,
    NEWARR = 0x000A,
    STELEM = 0x000B,
    LDELEM = 0x000C,
    MUL = 0x000D,
    MIN = 0x000E,
    SUB = 0x000F,
    DIV = 0x0010,
    CALL = 0x0011,
};

struct Operation {
    OperationCode code;
    std::vector<uint8_t> arguments;
};

enum class ConstantTag : uint8_t {
    U1 = 0x01,
    U2 = 0x02,
    U4 = 0x03,
    I4 = 0x04,
    STRING = 0x05,
    U8 = 0x06,
    I8 = 0x07,
    U16 = 0x08,
    I16 = 0x09,
    BOOL = 0x0A,
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
    std::vector<Operation> code;
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
    std::vector<Operation> code;
};

struct HeapRef {
    uint32_t id = 0;

    bool operator==(const HeapRef& other) const;

    bool operator!=(const HeapRef& other) const;
};

using Value = std::variant<
    uint8_t,        // U1
    uint16_t,       // U2
    uint32_t,       // U4
    int32_t,        // I4
    std::string,    // STRING
    uint64_t,       // U8
    int64_t,        // I8
    bool,           // BOOL
    HeapRef
>;

Value ConstantToValue(const Constant& c);

}

namespace std {

template<>
struct hash<czffvm::HeapRef> {
    size_t operator()(const czffvm::HeapRef& r) const noexcept;
};

}
