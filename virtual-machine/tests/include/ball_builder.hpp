#pragma once
#include <vector>
#include <string>
#include <cstdint>

namespace ball {

struct Builder {
    std::vector<uint8_t> b;

    void u1(uint8_t v) { b.push_back(v); }
    void u2(uint16_t v) {
        b.push_back(v >> 8);
        b.push_back(v & 0xFF);
    }
    void u4(uint32_t v) {
        b.push_back((v >> 24) & 0xFF);
        b.push_back((v >> 16) & 0xFF);
        b.push_back((v >> 8) & 0xFF);
        b.push_back(v & 0xFF);
    }

    void string(const std::string& s) {
        u2(s.size());
        for (char c : s) b.push_back(c);
    }
};

} // namespace ball
