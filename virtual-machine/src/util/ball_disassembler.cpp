#include <fstream>
#include <iostream>
#include <vector>
#include <string>

#include "ball_disassembler.hpp"

namespace czffvm {

BallDisassembler::BallDisassembler(const std::string& path)
        : path_(path) {}

void BallDisassembler::Disassemble() {
    std::ifstream file(path_, std::ios::binary);
    if (!file) {
        throw std::runtime_error("Cannot open file: " + path_);
    }

    std::vector<uint8_t> data(
        (std::istreambuf_iterator<char>(file)),
        std::istreambuf_iterator<char>()
    );

    ByteReader reader(data);

    // Пропускаем заголовок и константы
    LoadHeader(reader);
    LoadConstantPool(reader);

    // Читаем функции и печатаем opcodes
    uint16_t fn_count = reader.ReadU2();
    for (uint16_t i = 0; i < fn_count; ++i) {
        reader.ReadU2(); // name_index
        reader.ReadU2(); // params_descriptor_index
        reader.ReadU2(); // return_type_index
        reader.ReadU2(); // max_stack
        reader.ReadU2(); // locals_count

        uint16_t code_len = reader.ReadU2();
        for (uint16_t b = 0; b < code_len; ++b) {
            uint16_t op_code_raw = reader.ReadU2();
            OperationCode op_code = static_cast<OperationCode>(op_code_raw);

            std::string op_name = OperationCodeToString(op_code);

            std::cout << b << "\t" << op_name;

            // Считываем аргументы, если есть
            if (HasArguments(op_code)) {
                uint8_t arg1 = reader.ReadU1();
                uint8_t arg2 = reader.ReadU1();
                std::cout << "\t[" << (int)arg1 << ", " << (int)arg2 << "]";
            }

            std::cout << "\n";
        }
    }
}

void BallDisassembler::LoadHeader(ByteReader& r) {
    uint32_t magic = r.ReadU4();
    if (magic != kMagicNumber)
        throw std::runtime_error("Invalid magic number");

    r.ReadU1(); // version major
    r.ReadU1(); // version minor
    r.ReadU1(); // version patch
    r.ReadU1(); // flags
}

void BallDisassembler::LoadConstantPool(ByteReader& r) {
    uint16_t count = r.ReadU2();
    for (uint16_t i = 0; i < count; ++i) {
        ConstantTag tag = static_cast<ConstantTag>(r.ReadU1());
        switch (tag) {
            case ConstantTag::U1: r.ReadU1(); break;
            case ConstantTag::U2: r.ReadU1(); r.ReadU1(); break;
            case ConstantTag::U4: r.ReadU1(); r.ReadU1(); r.ReadU1(); r.ReadU1(); break;
            case ConstantTag::I4: r.ReadU1(); r.ReadU1(); r.ReadU1(); r.ReadU1(); break;
            case ConstantTag::STRING: r.ReadString(); break;
            case ConstantTag::U8: for(int i=0;i<8;i++) r.ReadU1(); break;
            case ConstantTag::I8: for(int i=0;i<8;i++) r.ReadU1(); break;
            case ConstantTag::U16: for(int i=0;i<16;i++) r.ReadU1(); break;
            case ConstantTag::I16: for(int i=0;i<16;i++) r.ReadU1(); break;
            case ConstantTag::BOOL: r.ReadU1(); break;
            default:
                std::cerr << "Got tag: " << (int)static_cast<uint8_t>(tag) << "\n";
                throw std::runtime_error("Unsupported constant in pool");
        }
    }
}

bool BallDisassembler::HasArguments(OperationCode code) {
    switch(code) {
        case OperationCode::NEWARR:
        case OperationCode::CALL:
        case OperationCode::HALT:
        case OperationCode::LDC:
        case OperationCode::STORE:
        case OperationCode::LDV:
        case OperationCode::JMP:
        case OperationCode::JZ:
        case OperationCode::JNZ:
            return true;
        default:
            return false;
    }
}

std::string BallDisassembler::OperationCodeToString(OperationCode code) {
    switch(code) {
        case OperationCode::LDC: return "LDC";
        case OperationCode::DUP: return "DUP";
        case OperationCode::SWAP: return "SWAP";
        case OperationCode::STORE: return "STORE";
        case OperationCode::LDV: return "LDV";
        case OperationCode::ADD: return "ADD";
        case OperationCode::PRINT: return "PRINT";
        case OperationCode::RET: return "RET";
        case OperationCode::HALT: return "HALT";
        case OperationCode::NEWARR: return "NEWARR";
        case OperationCode::STELEM: return "STELEM";
        case OperationCode::LDELEM: return "LDELEM";
        case OperationCode::MUL: return "MUL";
        case OperationCode::MIN: return "MIN";
        case OperationCode::SUB: return "SUB";
        case OperationCode::DIV: return "DIV";
        case OperationCode::CALL: return "CALL";
        case OperationCode::EQ: return "EQ";
        case OperationCode::LT: return "LT";
        case OperationCode::LEQ: return "LEQ";
        case OperationCode::JMP: return "JMP";
        case OperationCode::JZ: return "JZ";
        case OperationCode::JNZ: return "JNZ";
        default: return "UNKNOWN";
    }
}

} // namespace czffvm
