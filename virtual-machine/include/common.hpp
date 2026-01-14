#pragma once

#include <stdexcept>
#include <cstdint>
#include <string>
#include <vector>
#include <unordered_map>
#include <variant>
#include <memory>

#include "util/int128.hpp"
#include "util/uint128.hpp"

namespace czffvm {
const uint32_t kBytesInKiB = 1024;
const uint32_t kDefaultMaxHeapSizeInKiB = kBytesInKiB * 50; // 5 MiB

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
    EQ = 0x0012,
    LT = 0x0013,
    LEQ = 0x0014,
    JMP = 0x0015,
    JZ = 0x0016,
    JNZ = 0x0017,
    NEG = 0x0018,
    MOD = 0x0019,
    LOR = 0x001A,
    LAND = 0x001B
};

struct Operation {
    OperationCode code;
    std::vector<uint8_t> arguments;
};

enum class ConstantTag : uint8_t {
    U1 = 0x01,
    U2 = 0x02,
    U4 = 0x03,
    I1 = 0x04,
    I2 = 0x05,
    I4 = 0x06,
    U8 = 0x07,
    I8 = 0x08,
    U16 = 0x09,
    I16 = 0x0A,
    STRING = 0x0B,
    BOOL = 0x0C,
};

struct Constant {
    ConstantTag tag;
    std::vector<uint8_t> data;
};

struct RuntimeField {
    uint16_t name_index;
    uint16_t field_descriptor_index;
};

struct RuntimeClass {
    uint16_t name_index;
    std::vector<RuntimeField> fields;
    std::vector<uint16_t> methods;
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

using StringRef = std::shared_ptr<std::string>;

using Value = std::variant<
    int8_t,                     // I1
    uint8_t,                    // U1
    int16_t,                    // I2
    uint16_t,                   // U2
    uint32_t,                   // U4
    int32_t,                    // I4
    StringRef,                // STRING
    uint64_t,                   // U8
    int64_t,                    // I8
    stdint128::int128_t,        // I16
    stdint128::uint128_t,       // U16
    bool,                       // BOOL
    HeapRef
>;

Value ConstantToValue(const Constant& c);

std::string dump(const Value& v);

}

namespace std {

template<>
struct hash<czffvm::HeapRef> {
    size_t operator()(const czffvm::HeapRef& r) const noexcept;
};

}
