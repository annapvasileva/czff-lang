#include <vector>
#include <cstdint>
#include <string>

#include <gtest/gtest.h>

#include "class_loader.hpp"

TEST(ByteReaderTestSuite, ReadU1ReadsSingleByte) {
    std::vector<uint8_t> data = {0xAB, 0xCD};
    czffvm::ByteReader reader(data);

    EXPECT_EQ(reader.ReadU1(), 0xAB);
    EXPECT_EQ(reader.ReadU1(), 0xCD);
    EXPECT_TRUE(reader.Eof());
}

TEST(ByteReaderTestSuite, ReadU1OutOfBoundsThrows) {
    std::vector<uint8_t> data = {};
    czffvm::ByteReader reader(data);

    EXPECT_THROW(reader.ReadU1(), std::out_of_range);
}

TEST(ByteReaderTestSuite, ReadU2ReadsBigEndian) {
    std::vector<uint8_t> data = {0x12, 0x34};
    czffvm::ByteReader reader(data);

    EXPECT_EQ(reader.ReadU2(), 0x1234);
    EXPECT_TRUE(reader.Eof());
}

TEST(ByteReaderTestSuite, ReadU2AdvancesOffset) {
    std::vector<uint8_t> data = {0x00, 0x01, 0xFF};
    czffvm::ByteReader reader(data);

    reader.ReadU2();
    EXPECT_EQ(reader.ReadU1(), 0xFF);
}

TEST(ByteReaderTestSuite, ReadU2OutOfBoundsThrows) {
    std::vector<uint8_t> data = {0x01};
    czffvm::ByteReader reader(data);

    EXPECT_THROW(reader.ReadU2(), std::out_of_range);
}

TEST(ByteReaderTestSuite, ReadU4ReadsBigEndian) {
    std::vector<uint8_t> data = {0x01, 0x02, 0x03, 0x04};
    czffvm::ByteReader reader(data);

    EXPECT_EQ(reader.ReadU4(), 0x01020304);
    EXPECT_TRUE(reader.Eof());
}

TEST(ByteReaderTestSuite, ReadU4OutOfBoundsThrows) {
    std::vector<uint8_t> data = {0x00, 0x01, 0x02};
    czffvm::ByteReader reader(data);

    EXPECT_THROW(reader.ReadU4(), std::out_of_range);
}

TEST(ByteReaderTestSuite, ReadStringReadsLengthPrefixedString) {
    // length = 0x0005, "Hello"
    std::vector<uint8_t> data = {
        0x00, 0x05,
        'H', 'e', 'l', 'l', 'o'
    };

    czffvm::ByteReader reader(data);

    EXPECT_EQ(reader.ReadString(), "Hello");
    EXPECT_TRUE(reader.Eof());
}

TEST(ByteReaderTestSuite, ReadStringWithEmptyString) {
    std::vector<uint8_t> data = {0x00, 0x00};
    czffvm::ByteReader reader(data);

    EXPECT_EQ(reader.ReadString(), "");
    EXPECT_TRUE(reader.Eof());
}

TEST(ByteReaderTestSuite, ReadStringOutOfBoundsThrows) {
    // length says 5, but only 3 bytes present
    std::vector<uint8_t> data = {
        0x00, 0x05,
        'a', 'b', 'c'
    };

    czffvm::ByteReader reader(data);

    EXPECT_THROW(reader.ReadString(), czffvm::ClassLoaderError);
}

TEST(ByteReaderTestSuite, EofInitiallyFalse) {
    std::vector<uint8_t> data = {0x01};
    czffvm::ByteReader reader(data);

    EXPECT_FALSE(reader.Eof());
}

TEST(ByteReaderTestSuite, EofBecomesTrueAfterReads) {
    std::vector<uint8_t> data = {0x01, 0x02};
    czffvm::ByteReader reader(data);

    reader.ReadU2();
    EXPECT_TRUE(reader.Eof());
}
