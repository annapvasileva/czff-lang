#include <fstream>
#include <sstream>

#include "class_loader.hpp"

namespace czffvm {

const int32_t kBitsInByte = 8;
const uint32_t kMagicNumber = 0x62616c6c;

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

    file_constants_.clear();
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
    file_constants_.clear();
    file_constants_.reserve(count);

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
            case ConstantTag::STRING: {
                std::string s = r.ReadString();
                c.data = std::vector<uint8_t>(s.begin(), s.end());
                break;
            }
            case ConstantTag::BOOL:
                c.data.push_back(r.ReadU1());
                break;
            default:
                throw ClassLoaderError("FileLoading", "Cannot determine the type of constant in constant pool");
        }
        file_constants_.push_back(c);
        rda_.InternConstant(c);
    }
}

void ClassLoader::LoadClasses(ByteReader& r) {
    uint16_t count = r.ReadU2();

    for (uint16_t i = 0; i < count; ++i) {
        auto* cls = rda_.Allocate<RuntimeClass>();
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
            RuntimeMethod method;
            method.name_index = r.ReadU2();
            method.params_descriptor_index = r.ReadU2();
            method.return_type_index = r.ReadU2();

            method.max_stack = r.ReadU2();
            method.locals_count = r.ReadU2();

            uint16_t code_len = r.ReadU2();
            method.code.resize(code_len);
            for (uint16_t b = 0; b < code_len; ++b){
                method.code[b] = r.ReadU1();
            }

            cls->methods.push_back(method);
        }

        rda_.RegisterClass(cls);
    }
}

void ClassLoader::LoadFunctions(ByteReader& r) {
    uint16_t count = r.ReadU2();

    for (uint16_t i = 0; i < count; ++i) {
        auto* fn = rda_.Allocate<RuntimeFunction>();

        fn->name_index = r.ReadU2();
        fn->params_descriptor_index = r.ReadU2();
        fn->return_type_index = r.ReadU2();

        fn->max_stack = r.ReadU2();
        fn->locals_count = r.ReadU2();

        uint16_t code_len = r.ReadU2();
        fn->code.resize(code_len);
        for (uint16_t b = 0; b < code_len; ++b)
            fn->code[b] = r.ReadU1();

        rda_.RegisterFunction(fn);
    }
}

void ClassLoader::ResolveEntryPoint() {
    auto it = rda_.Functions().find("Main");
    if (it == rda_.Functions().end()) {
        throw ClassLoaderError("EntryPoint", "Main not found");
    }

    auto fn = it->second;

    std::vector<uint8_t> return_type_raw = rda_.GetConstant(fn->return_type_index).data;
    std::string return_type(return_type_raw.begin(), return_type_raw.end());

    std::vector<uint8_t> params_raw = rda_.GetConstant(fn->params_descriptor_index).data;
    std::string params(params_raw.begin(), params_raw.end());

    if (return_type != "void" || params != "") {
        throw ClassLoaderError("EntryPoint", "Invalid Main signature");
    }

    entry_point_ = fn;
}

} // namespace czffvm
