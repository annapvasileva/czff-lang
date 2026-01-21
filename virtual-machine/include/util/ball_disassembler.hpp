#pragma once
#include <fstream>
#include <iostream>
#include <vector>
#include <string>

#include "class_loader.hpp"

namespace czffvm {

class BallDisassembler {
public:
    BallDisassembler(const std::string& path);

    void Disassemble();

private:
    std::string path_;

    void LoadHeader(ByteReader& r);

    void LoadConstantPool(ByteReader& r);

    static bool HasArguments(OperationCode code);

    static std::string OperationCodeToString(OperationCode code);
};

} // namespace czffvm
