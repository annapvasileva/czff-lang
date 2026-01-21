#include <gtest/gtest.h>
#include <sstream>

#include "int128.hpp"
#include "uint128.hpp"

using namespace stdint128;

TEST(UInt128, BasicArithmetic){
    uint128_t a = 5;
    uint128_t b = 10;

    EXPECT_EQ(a+b, 15);
    EXPECT_EQ(b-a, 5);
    EXPECT_EQ(a*b, 50);
    EXPECT_EQ(b/a, 2);
    EXPECT_EQ(b%a, 0);
}

TEST(UInt128, Overflow){
    uint128_t a(0xFFFFFFFFFFFFFFFFULL,0xFFFFFFFFFFFFFFFFULL);
    uint128_t r = a + 1;

    EXPECT_EQ(r.hi, 0);
    EXPECT_EQ(r.lo, 0);
}

TEST(UInt128, Shift){
    uint128_t a = 1;
    a = a << 100;

    EXPECT_EQ(a.hi, (1ull<<(100-64)));
    EXPECT_EQ(a.lo, 0);

    EXPECT_EQ(a >> 100, 1);
}

TEST(UInt128, Bitwise){
    uint128_t a(0xFFFF,0);
    uint128_t b(0x0F0F,0);

    auto c = a & b;
    EXPECT_EQ(c.hi, 0x0F0F);

    c = a | b;
    EXPECT_EQ(c.hi, 0xFFFF);

    c = ~a;
    EXPECT_EQ(c.hi, ~0xFFFFull);
}

TEST(UInt128, Print){
    std::stringstream ss;

    ss << uint128_t(0);
    EXPECT_EQ(ss.str(), "0");

    ss.str(""); ss.clear();
    ss << uint128_t(123456789);
    EXPECT_EQ(ss.str(), "123456789");

    ss.str(""); ss.clear();
    uint128_t x = (uint128_t(1)<<100) + 5;
    ss << x;
    EXPECT_EQ(ss.str(), "1267650600228229401496703205381");
}

TEST(Int128, Basic){
    int128_t a = -5;
    int128_t b = 3;

    EXPECT_EQ(a+b, -2);
    EXPECT_EQ(a-b, -8);
    EXPECT_EQ(a*b, -15);
    EXPECT_EQ(a/b, -1);
}

TEST(Int128, Sign){
    int128_t a = -10;
    int128_t b = -5;

    EXPECT_EQ(a+b, -15);
    EXPECT_EQ(a*b, 50);
    EXPECT_EQ(a/b, 2);
}

TEST(Int128, Large){
    int128_t a = (int128_t(1) << 100);
    int128_t b = -1;

    EXPECT_EQ(a+b, ((int128_t)1<<100)-1);
    EXPECT_EQ(a*b, -((int128_t)1<<100));
}
