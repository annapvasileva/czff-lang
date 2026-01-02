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
    uint16_t v = data_.at(offset_) |
                 (data_.at(offset_ + 1) << 8);
    offset_ += 2;
    return v;
}

uint32_t ByteReader::ReadU4() {
    uint32_t v = data_.at(offset_) |
                 (data_.at(offset_ + 1) << 8) |
                 (data_.at(offset_ + 2) << 16) |
                 (data_.at(offset_ + 3) << 24);
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

ClassLoader::ClassLoader() = default;

void ClassLoader::LoadStdlib(const std::string& path) {
    LoadFile(path);
}

void ClassLoader::LoadProgram(const std::string& path) {
    LoadFile(path);
    Verify();
    Link();
    ResolveEntryPoint();
}

RuntimeFunction* ClassLoader::entryPoint() const {
    return entryPoint_;
}

void ClassLoader::LoadFile(const std::string& path) {
    std::ifstream file(path, std::ios::binary);
    if (!file)
        throw ClassLoaderError("FileLoading", "Cannot open file", path);

    std::vector<uint8_t> data(
        (std::istreambuf_iterator<char>(file)),
        std::istreambuf_iterator<char>()
    );

    ByteReader reader(data);

    LoadHeader(reader);
    LoadConstantPool(reader);
    LoadClasses(reader);
    LoadFunctions(reader);
}

void ClassLoader::LoadHeader(ByteReader& r) {
    uint32_t magic = r.ReadU4();
    if (magic != 0x62616c6c)
        throw ClassLoaderError("Header", "Invalid magic number");

    r.ReadU1(); // version major
    r.ReadU1(); // version minor
    r.ReadU1(); // version patch

    r.ReadU1(); // flags
}

void ClassLoader::LoadConstantPool(ByteReader& r) {
    uint16_t count = r.ReadU2();
    constantPool_.reserve(count);

    for (uint16_t i = 0; i < count; ++i) {
        Constant c;
        c.tag = static_cast<ConstantTag>(r.ReadU1());

        if (c.tag == ConstantTag::STRING ||
            c.tag == ConstantTag::CLASS) {
            c.str = r.ReadString();
        } else {
            throw ClassLoaderError("ConstantPool", "Unsupported constant tag");
        }

        constantPool_.push_back(c);
    }
}

void ClassLoader::LoadClasses(ByteReader& r) {
    uint16_t classCount = r.ReadU2();

    for (uint16_t i = 0; i < classCount; ++i) {
        RuntimeClass cls;
        cls.name = r.ReadString();

        if (classes_.count(cls.name))
            throw ClassLoaderError("ClassLoading", "Duplicate class", cls.name);

        uint16_t fieldCount = r.ReadU2();
        for (uint16_t f = 0; f < fieldCount; ++f) {
            RuntimeField field;
            field.name = r.ReadString();
            field.type = r.ReadString();
            cls.fields.push_back(field);
        }

        uint16_t methodCount = r.ReadU2();
        for (uint16_t m = 0; m < methodCount; ++m) {
            RuntimeMethod method;
            method.name = r.ReadString();
            method.params = r.ReadString();
            method.returnType = r.ReadString();
            method.maxStack = r.ReadU2();
            method.locals = r.ReadU2();

            uint16_t codeLen = r.ReadU2();
            method.code.resize(codeLen);
            for (uint16_t b = 0; b < codeLen; ++b)
                method.code[b] = r.ReadU1();

            cls.methods.push_back(method);
        }

        classes_.emplace(cls.name, std::move(cls));
    }
}

void ClassLoader::LoadFunctions(ByteReader& r) {
    uint16_t count = r.ReadU2();

    for (uint16_t i = 0; i < count; ++i) {
        RuntimeFunction fn;
        fn.name = r.ReadString();
        fn.params = r.ReadString();
        fn.returnType = r.ReadString();
        fn.maxStack = r.ReadU2();
        fn.locals = r.ReadU2();

        uint16_t codeLen = r.ReadU2();
        fn.code.resize(codeLen);
        for (uint16_t b = 0; b < codeLen; ++b)
            fn.code[b] = r.ReadU1();

        if (functions_.count(fn.name))
            throw ClassLoaderError("FunctionLoading", "Duplicate function", fn.name);

        functions_.emplace(fn.name, std::move(fn));
    }
}

void ClassLoader::Verify() {
    VerifyClasses();
    VerifyFunctions();
}

void ClassLoader::VerifyClasses() {
    for (auto& [name, cls] : classes_) {
        std::unordered_map<std::string, bool> seen;

        for (auto& f : cls.fields) {
            if (seen[f.name])
                throw ClassLoaderError("Verification", "Duplicate field", cls.name);
            seen[f.name] = true;
        }
    }
}

void ClassLoader::VerifyFunctions() {
    for (auto& [name, fn] : functions_) {
        if (fn.maxStack == 0)
            throw ClassLoaderError("Verification", "max_stack == 0", name);
    }
}

void ClassLoader::Link() {
    PrepareLayouts();
}

void ClassLoader::PrepareLayouts() {
    for (auto& [_, cls] : classes_) {
        uint16_t offset = 0;
        for (auto& f : cls.fields) {
            f.offset = offset++;
        }
    }
}

void ClassLoader::ResolveEntryPoint() {
    auto it = functions_.find("Main");
    if (it == functions_.end())
        throw ClassLoaderError("EntryPoint", "Main not found");

    auto& fn = it->second;

    if (fn.returnType != "void" || fn.params != "")
        throw ClassLoaderError("EntryPoint", "Invalid Main signature");

    entryPoint_ = &fn;
}

} // namespace czffvm
