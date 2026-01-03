#pragma once

#include <cstdint>
#include <string>
#include <vector>
#include <unordered_map>
#include <memory>

#include "common.hpp"
#include "runtime_data_area.hpp"

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

class ClassLoader {
public:
    explicit ClassLoader(RuntimeDataArea& rda);

    void LoadStdlib(const std::string& path);
    void LoadProgram(const std::string& path);

    RuntimeFunction* EntryPoint() const;
private:
    RuntimeDataArea& rda_;
    RuntimeFunction* entry_point_ = nullptr;
    std::vector<Constant> file_constants_;

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
};

}  // namespace czffvm
