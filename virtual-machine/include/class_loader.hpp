#pragma once

#include <cstdint>
#include <string>
#include <vector>
#include <unordered_map>
#include <memory>

namespace czffvm {

class ClassLoaderError : public std::runtime_error {
public:
    ClassLoaderError(
        const std::string& stage,
        const std::string& message,
        const std::string& context = ""
    );
};

class ByteReader {
public:
    explicit ByteReader(const std::vector<uint8_t>& data);

    uint8_t ReadU1();
    uint16_t ReadU2();
    uint32_t ReadU4();
    std::string ReadString();

    bool Eof() const;

private:
    const std::vector<uint8_t>& data_;
    size_t offset_;
};

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

class ClassLoader {
public:
    ClassLoader();

    void LoadStdlib(const std::string& path);
    void LoadProgram(const std::string& path);

    RuntimeFunction* entryPoint() const;
private:
    std::unordered_map<std::string, RuntimeClass> classes_;
    std::unordered_map<std::string, RuntimeFunction> functions_;
    std::vector<Constant> constantPool_;
    RuntimeFunction* entryPoint_ = nullptr;

    void LoadFile(const std::string& path);
    void Verify();
    void Link();
    void ResolveEntryPoint();

    void LoadHeader(ByteReader& reader);
    void LoadConstantPool(ByteReader& reader);
    void LoadClasses(ByteReader& reader);
    void LoadFunctions(ByteReader& reader);

    void VerifyClasses();
    void VerifyFunctions();

    void PrepareLayouts();
};

}  // namespace czffvm
