#pragma once

#include <vector>
#include <fstream>
#include <string>
#include <cstdint>

inline void WriteBinaryFile(
    const std::string& path,
    const std::vector<uint8_t>& data
) {
    std::ofstream f(path, std::ios::binary);
    f.write(reinterpret_cast<const char*>(data.data()), data.size());
}
