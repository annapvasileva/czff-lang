#include <fstream>
#include <sstream>

#include "class_loader.hpp"

namespace czffvm {

const int32_t kByteInBits = 8;
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
    uint16_t v = data_.at(offset_) |
                 (data_.at(offset_ + 1) << kByteInBits);
    offset_ += 2;

    return v;
}

uint32_t ByteReader::ReadU4() {
    uint32_t v = data_.at(offset_) |
                 (data_.at(offset_ + 1) << kByteInBits) |
                 (data_.at(offset_ + 2) << (2 * kByteInBits)) |
                 (data_.at(offset_ + 3) << (3 * kByteInBits));
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
    Verify();
    Link();
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
    LoadClasses(reader);
    LoadFunctions(reader);

    file_constants_.clear();
}

// TODO: осуществлять проверку хедера
void ClassLoader::LoadHeader(ByteReader& r) {
    uint32_t magic = r.ReadU4();
    if (magic != kMagicNumber)
        throw ClassLoaderError("Header", "Invalid magic number");

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
        c.str = r.ReadString();
        file_constants_.push_back(c);
        rda_.InternConstant(c);
    }
}

void ClassLoader::LoadClasses(ByteReader& r) {
    uint16_t count = r.ReadU2();

    for (uint16_t i = 0; i < count; ++i) {
        auto* cls = rda_.Allocate<RuntimeClass>();
        cls->name = r.ReadString();

        uint16_t fields = r.ReadU2();
        for (uint16_t f = 0; f < fields; ++f) {
            RuntimeField field;
            field.name = r.ReadString();
            field.type = r.ReadString();
            cls->fields.push_back(field);
        }

        uint16_t methods = r.ReadU2();
        for (uint16_t m = 0; m < methods; ++m) {
            RuntimeMethod method;
            method.name       = r.ReadString();
            method.params     = r.ReadString();
            method.returnType = r.ReadString();
            method.maxStack   = r.ReadU2();
            method.locals     = r.ReadU2();

            uint16_t codeLen = r.ReadU2();
            method.code.resize(codeLen);
            for (uint16_t b = 0; b < codeLen; ++b)
                method.code[b] = r.ReadU1();

            cls->methods.push_back(method);
        }

        rda_.RegisterClass(cls);
    }
}

void ClassLoader::LoadFunctions(ByteReader& r) {
    uint16_t count = r.ReadU2();

    for (uint16_t i = 0; i < count; ++i) {
        auto* fn = rda_.Allocate<RuntimeFunction>();

        fn->name       = r.ReadString();
        fn->params     = r.ReadString();
        fn->returnType = r.ReadString();
        fn->maxStack   = r.ReadU2();
        fn->locals     = r.ReadU2();

        uint16_t codeLen = r.ReadU2();
        fn->code.resize(codeLen);
        for (uint16_t b = 0; b < codeLen; ++b)
            fn->code[b] = r.ReadU1();

        rda_.RegisterFunction(fn);
    }
}

void ClassLoader::Verify() {
    VerifyClasses();
    VerifyFunctions();
}

void ClassLoader::VerifyClasses() {
    for (auto& [name, cls] : rda_.Classes()) {
        std::unordered_map<std::string, bool> seen;

        for (auto& f : cls->fields) {
            if (seen[f.name]) {
                throw ClassLoaderError("Verification", "Duplicate field", cls->name);
            }
            seen[f.name] = true;
        }
    }
}

void ClassLoader::VerifyFunctions() {
    for (auto& [name, fn] : rda_.Functions()) {
        if (fn->maxStack == 0){
            throw ClassLoaderError("Verification", "max_stack == 0", name);
        }
    }
}

void ClassLoader::Link() {
    PrepareLayouts();
}

void ClassLoader::PrepareLayouts() {
    for (auto& [_, cls] : rda_.Classes()) {
        uint16_t offset = 0;
        for (auto& f : cls->fields) {
            f.offset = offset++;
        }
    }
}

void ClassLoader::ResolveEntryPoint() {
    auto it = rda_.Functions().find("Main");
    if (it == rda_.Functions().end())
        throw ClassLoaderError("EntryPoint", "Main not found");

    auto fn = it->second;

    if (fn->returnType != "void" || fn->params != "")
        throw ClassLoaderError("EntryPoint", "Invalid Main signature");

    entry_point_ = fn;
}

} // namespace czffvm
