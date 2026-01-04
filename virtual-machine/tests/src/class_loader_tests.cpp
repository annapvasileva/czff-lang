#include <gtest/gtest.h>
#include <fstream>
#include <cstdio>

#include "class_loader.hpp"
#include "runtime_data_area.hpp"
#include "minimal_ball.hpp"

using namespace czffvm;

static void WriteFile(const std::string& path,
                      const std::vector<uint8_t>& data) {
    std::ofstream f(path, std::ios::binary);
    f.write(reinterpret_cast<const char*>(data.data()), data.size());
}

struct TempFile {
    std::string path;
    explicit TempFile(const std::string& p) : path(p) {}
    ~TempFile() { std::remove(path.c_str()); }
};

// ---------- happy path ----------

TEST(ClassLoaderTestSuite, LoadsMinimalProgramAndResolvesEntryPoint) {
    RuntimeDataArea rda;
    ClassLoader loader(rda);

    auto data = MakeMinimalBallWithMain();
    TempFile tmp("minimal.ball");
    WriteFile(tmp.path, data);

    ASSERT_NO_THROW(loader.LoadProgram(tmp.path));
    ASSERT_NE(loader.EntryPoint(), nullptr);

    EXPECT_TRUE(rda.Functions().contains("Main"));
}

// ---------- header ----------

TEST(ClassLoaderTestSuite, InvalidMagicThrows) {
    RuntimeDataArea rda;
    ClassLoader loader(rda);

    auto data = MakeMinimalBallWithMain();
    data[0] = 0x00;

    TempFile tmp("bad_magic.ball");
    WriteFile(tmp.path, data);

    EXPECT_THROW(loader.LoadProgram(tmp.path), ClassLoaderError);
}

// ---------- entry point ----------

TEST(ClassLoaderTestSuite, MissingMainThrows) {
    RuntimeDataArea rda;
    ClassLoader loader(rda);

    auto data = MakeMinimalBallWithoutMain();

    TempFile tmp("no_main.ball");
    WriteFile(tmp.path, data);

    EXPECT_THROW(loader.LoadProgram(tmp.path), ClassLoaderError);
}

TEST(ClassLoaderTestSuite, InvalidMainSignatureThrows) {
    RuntimeDataArea rda;
    ClassLoader loader(rda);

    auto data = MakeMinimalBallWithWrongMainSignature();

    TempFile tmp("bad_sig.ball");
    WriteFile(tmp.path, data);

    EXPECT_THROW(loader.LoadProgram(tmp.path), ClassLoaderError);
}

TEST(ClassLoaderIntegrationTestSuite, LoadsFirstProgramBall) {
    RuntimeDataArea rda;
    ClassLoader loader(rda);

    ASSERT_NO_THROW(loader.LoadProgram("FirstProgram.ball"));

    RuntimeFunction* entry = loader.EntryPoint();
    ASSERT_NE(entry, nullptr);

    auto fn_name_raw = rda.GetConstant(entry->name_index).data;
    std::string fn_name(fn_name_raw.begin(), fn_name_raw.end());
    EXPECT_EQ(fn_name, "Main");

    auto return_type_raw = rda.GetConstant(entry->return_type_index).data;
    std::string return_type(return_type_raw.begin(), return_type_raw.end());
    EXPECT_EQ(return_type, "void");

    auto params_raw = rda.GetConstant(entry->params_descriptor_index).data;
    std::string params(params_raw.begin(), params_raw.end());
    EXPECT_EQ(params, "");

    EXPECT_EQ(entry->locals_count, 3);
    EXPECT_EQ(entry->max_stack, 0);

    EXPECT_TRUE(rda.Functions().contains("Main"));

    EXPECT_FALSE(entry->code.empty());
}

