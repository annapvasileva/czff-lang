#include <fstream>
#include <sstream>

#include "class_loader.hpp"

namespace czffvm {

ClassLoaderError::ClassLoaderError(
    const std::string& stage,
    const std::string& message,
    const std::string& context
)
    : std::runtime_error(
        "[ClassLoader][" + stage + "] " +
        (context.empty() ? "" : context + ": ") +
        message
      )
{}

ByteReader::ByteReader(const std::vector<uint8_t>& data)
    : data_(data), offset_(0) {}

uint8_t ByteReader::ReadU1() {
    return data_.at(offset_++);
}

uint16_t ByteReader::ReadU2() {
    uint16_t v = (data_.at(offset_) << kBitsInByte) |
                 data_.at(offset_ + 1);
    offset_ += 2;

    return v;
}

uint32_t ByteReader::ReadU4() {
    uint32_t v = (data_.at(offset_) << (3 * kBitsInByte)) |
                 (data_.at(offset_ + 1) << ( 2 * kBitsInByte)) |
                 (data_.at(offset_ + 2) << kBitsInByte) |
                 data_.at(offset_ + 3);
    offset_ += 4;

    return v;
}

std::string ByteReader::ReadString() {
    uint16_t len = ReadU2();

    if (offset_ + len > data_.size()) {
        throw ClassLoaderError(
            "ByteReader",
            "Unexpected end of input while reading string",
            "offset=" + std::to_string(offset_) +
            ", len=" + std::to_string(len) +
            ", size=" + std::to_string(data_.size())
        );
    }

    std::string s(reinterpret_cast<const char*>(&data_[offset_]), len);
    offset_ += len;

    return s;
}

bool ByteReader::Eof() const {
    return offset_ >= data_.size();
}

ClassLoader::ClassLoader(RuntimeDataArea& rda)
    : rda_(rda) { }

void ClassLoader::LoadStdlib(const std::string& path) {
    LoadFile(path);
}

void ClassLoader::LoadProgram(const std::string& path) {
    LoadFile(path);
    ResolveEntryPoint();
}

RuntimeFunction* ClassLoader::EntryPoint() const {
    return entry_point_;
}

void ClassLoader::LoadFile(const std::string& path) {
    std::ifstream file(path, std::ios::binary);
    if (!file) {
        throw ClassLoaderError("FileLoading", "Cannot open file", path);
    }

    std::vector<uint8_t> data(
        (std::istreambuf_iterator<char>(file)),
        std::istreambuf_iterator<char>()
    );

    ByteReader reader(data);

    LoadHeader(reader);
    LoadConstantPool(reader);
    LoadFunctions(reader);
    LoadClasses(reader);
}

void ClassLoader::LoadHeader(ByteReader& r) {
    uint32_t magic = r.ReadU4();
    if (magic != kMagicNumber) {
        throw ClassLoaderError("Header", "Invalid magic number");
    }

    r.ReadU1(); // version major
    r.ReadU1(); // version minor
    r.ReadU1(); // version patch

    r.ReadU1(); // flags
}

void ClassLoader::LoadConstantPool(ByteReader& r) {
    uint16_t count = r.ReadU2();

    for (uint16_t i = 0; i < count; ++i) {
        Constant c;
        c.tag = static_cast<ConstantTag>(r.ReadU1());
        switch (c.tag) {
            case ConstantTag::U1:
                c.data.push_back(r.ReadU1());
                break;
            case ConstantTag::U2:
                c.data.push_back(r.ReadU1());
                c.data.push_back(r.ReadU1());
                break;
            case ConstantTag::U4:
                for (int i = 0; i < 4; i++) {
                    c.data.push_back(r.ReadU1());
                }
                break;
            case ConstantTag::I4:
                for (int i = 0; i < 4; i++) {
                    c.data.push_back(r.ReadU1());
                }
                break;
            case ConstantTag::STRING: {
                std::string s = r.ReadString();
                c.data = std::vector<uint8_t>(s.begin(), s.end());
                break;
            }
            case ConstantTag::U8:
                for (int i = 0; i < 8; i++) {
                    c.data.push_back(r.ReadU1());
                }
                break;
            case ConstantTag::I8:
                for (int i = 0; i < 8; i++) {
                    c.data.push_back(r.ReadU1());
                }
                break;
            case ConstantTag::U16:
                for (int i = 0; i < 16; i++) {
                    c.data.push_back(r.ReadU1());
                }
                break;
            case ConstantTag::I16:
                for (int i = 0; i < 16; i++) {
                    c.data.push_back(r.ReadU1());
                }
                break;
            case ConstantTag::BOOL:
                c.data.push_back(r.ReadU1());
                break;
            default:
                throw ClassLoaderError("FileLoading", "Unsupported type of constant in constant pool");
        }
        rda_.GetMethodArea().RegisterConstant(c);
    }
}

void ClassLoader::LoadClasses(ByteReader& r) {
    uint16_t count = r.ReadU2();

    for (uint16_t i = 0; i < count; ++i) {
        auto cls = new RuntimeClass();
        cls->name_index = r.ReadU2();

        uint16_t fields_count = r.ReadU2();
        for (uint16_t f = 0; f < fields_count; ++f) {
            RuntimeField field;
            field.name_index = r.ReadU2();
            field.field_descriptor_index = r.ReadU2();
            cls->fields.push_back(field);
        }

        uint16_t methods_count = r.ReadU2();
        for (uint16_t m = 0; m < methods_count; ++m) {
            uint16_t method_index = r.ReadU2();

            cls->methods.push_back(method_index);
        }

        rda_.GetMethodArea().RegisterClass(cls);
    }
}

void ClassLoader::LoadFunctions(ByteReader& r) {
    uint16_t count = r.ReadU2();

    for (uint16_t i = 0; i < count; ++i) {
        auto fn = new RuntimeFunction();

        fn->name_index = r.ReadU2();
        fn->params_descriptor_index = r.ReadU2();
        fn->return_type_index = r.ReadU2();

        fn->max_stack = r.ReadU2();
        fn->locals_count = r.ReadU2();

        uint16_t code_len = r.ReadU2();
        fn->code.resize(code_len);
        for (uint16_t b = 0; b < code_len; ++b) {
            Operation op;
            op.code = static_cast<OperationCode>(r.ReadU2());
            switch (op.code) {
                case OperationCode::STELEM:
                case OperationCode::LDELEM:
                case OperationCode::MUL:
                case OperationCode::MIN:
                case OperationCode::SUB:
                case OperationCode::DIV:
                case OperationCode::DUP:
                case OperationCode::SWAP:
                case OperationCode::ADD:
                case OperationCode::PRINT:
                case OperationCode::RET:
                case OperationCode::EQ:
                case OperationCode::LT:
                case OperationCode::LEQ:
                case OperationCode::NEG:
                case OperationCode::MOD:
                case OperationCode::LOR:
                case OperationCode::LAND:
                    break;
                case OperationCode::NEWARR:
                case OperationCode::CALL:
                case OperationCode::HALT:
                case OperationCode::LDC:
                case OperationCode::STORE:
                case OperationCode::LDV:
                case OperationCode::JMP:
                case OperationCode::JZ:
                case OperationCode::JNZ:
                    op.arguments.push_back(r.ReadU1());
                    op.arguments.push_back(r.ReadU1());
                    break;
            }
            fn->code[b] = op;
        }

        rda_.GetMethodArea().RegisterFunction(fn);
    }
}

void ClassLoader::ResolveEntryPoint() {
    const auto& functions = rda_.GetMethodArea().Functions();
    RuntimeFunction* fn = NULL;
    for (auto f : functions) {
        Constant name_raw = rda_.GetMethodArea().GetConstant(f->name_index);
        std::string name(name_raw.data.begin(), name_raw.data.end());
        if (name == "Main") {
            fn = f;
            break;
        }
    }
    
    if (!fn) {
        throw ClassLoaderError("EntryPoint", "Main not found");
    }

    const auto& ret =
        rda_.GetMethodArea().GetConstant(fn->return_type_index).data;
    const auto& params =
        rda_.GetMethodArea().GetConstant(fn->params_descriptor_index).data;

    if (std::string(ret.begin(), ret.end()) != "void;" ||
        !params.empty()) {
        throw ClassLoaderError("EntryPoint", "Invalid Main signature");
    }

    entry_point_ = fn;
}

} // namespace czffvm
