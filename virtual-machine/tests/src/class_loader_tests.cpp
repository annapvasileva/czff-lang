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
    
    const auto& functions = rda.GetMethodArea().Functions();
    bool found = false;
    for (auto f : functions) {
        Constant name_raw = rda.GetMethodArea().GetConstant(f->name_index);
        std::string name(name_raw.data.begin(), name_raw.data.end());
        if (name == "Main") {
            found = true;
            break;
        }
    }
    EXPECT_TRUE(found);
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

TEST(ClassLoaderIntegrationTestSuite, ConstantPoolIsCorrect) {
    RuntimeDataArea rda;
    ClassLoader loader(rda);

    auto data = MakeFirstProgramBall();
    TempFile tmp("first.ball");
    WriteFile(tmp.path, data);
    ASSERT_NO_THROW(loader.LoadProgram(tmp.path));

    const auto& constants = rda.GetMethodArea().ConstantPool();
    ASSERT_GE(constants.size(), 5u);

    // 0: "Main"
    {
        const auto& c = constants[0];
        EXPECT_EQ(c.tag, ConstantTag::STRING);
        std::string s(c.data.begin(), c.data.end());
        EXPECT_EQ(s, "Main");
    }

    // 1: ""
    {
        const auto& c = constants[1];
        EXPECT_EQ(c.tag, ConstantTag::STRING);
        std::string s(c.data.begin(), c.data.end());
        EXPECT_EQ(s, "");
    }

    // 2: "void;"
    {
        const auto& c = constants[2];
        EXPECT_EQ(c.tag, ConstantTag::STRING);
        std::string s(c.data.begin(), c.data.end());
        EXPECT_EQ(s, "void;");
    }

    // 3: 2
    {
        const auto& c = constants[3];
        EXPECT_EQ(c.tag, ConstantTag::I4);
        ASSERT_EQ(c.data.size(), 4u);
        uint32_t val = (c.data[0] << 24) | (c.data[1] << 16) | (c.data[2] << 8) | c.data[3];
        EXPECT_EQ(val, 2u);
    }

    // 4: 3
    {
        const auto& c = constants[4];
        EXPECT_EQ(c.tag, ConstantTag::I4);
        ASSERT_EQ(c.data.size(), 4u);
        uint32_t val = (c.data[0] << 24) | (c.data[1] << 16) | (c.data[2] << 8) | c.data[3];
        EXPECT_EQ(val, 3u);
    }
}


TEST(ClassLoaderIntegrationTestSuite, LoadsFirstProgramBall) {
    RuntimeDataArea rda;
    ClassLoader loader(rda);

    auto data = MakeFirstProgramBall();
    TempFile tmp("first.ball");
    WriteFile(tmp.path, data);
    ASSERT_NO_THROW(loader.LoadProgram(tmp.path));

    RuntimeFunction* entry = loader.EntryPoint();
    ASSERT_NE(entry, nullptr);

    auto fn_name_raw = rda.GetMethodArea().GetConstant(entry->name_index).data;
    std::string fn_name(fn_name_raw.begin(), fn_name_raw.end());
    EXPECT_EQ(fn_name, "Main");

    auto return_type_raw = rda.GetMethodArea().GetConstant(entry->return_type_index).data;
    std::string return_type(return_type_raw.begin(), return_type_raw.end());
    EXPECT_EQ(return_type, "void;");

    auto params_raw = rda.GetMethodArea().GetConstant(entry->params_descriptor_index).data;
    std::string params(params_raw.begin(), params_raw.end());
    EXPECT_EQ(params, "");

    EXPECT_EQ(entry->locals_count, 3);
    EXPECT_EQ(entry->max_stack, 2);

    const auto& functions = rda.GetMethodArea().Functions();
    bool found = false;
    for (auto f : functions) {
        Constant name_raw = rda.GetMethodArea().GetConstant(f->name_index);
        std::string name(name_raw.data.begin(), name_raw.data.end());
        if (name == "Main") {
            found = true;
            break;
        }
    }
    EXPECT_TRUE(found);

    EXPECT_FALSE(entry->code.empty());
}

TEST(ClassLoaderIntegrationTestSuite, MainFunctionOperations) {
    RuntimeDataArea rda;
    ClassLoader loader(rda);

    auto data = MakeFirstProgramBall();
    TempFile tmp("first.ball");
    WriteFile(tmp.path, data);
    ASSERT_NO_THROW(loader.LoadProgram(tmp.path));

    RuntimeFunction* entry = loader.EntryPoint();
    ASSERT_NE(entry, nullptr);

    struct ExpectedOp {
        OperationCode code;
        std::vector<uint8_t> args;
    };

    std::vector<ExpectedOp> expected = {
        {OperationCode::LDC,   {0x00, 0x03}},
        {OperationCode::STORE, {0x00, 0x00}},
        {OperationCode::LDC,   {0x00, 0x04}},
        {OperationCode::STORE, {0x00, 0x01}},
        {OperationCode::LDV,   {0x00, 0x00}},
        {OperationCode::LDV,   {0x00, 0x01}},
        {OperationCode::ADD,   {}},
        {OperationCode::STORE, {0x00, 0x02}},
        {OperationCode::LDV,   {0x00, 0x02}},
        {OperationCode::PRINT, {}},
        {OperationCode::RET,  {}},
    };

    ASSERT_EQ(entry->code.size(), expected.size());

    for (size_t i = 0; i < expected.size(); ++i) {
        const auto& got = entry->code[i];
        const auto& exp = expected[i];

        EXPECT_EQ(got.code, exp.code) << "Operation #" << i << " has wrong opcode";

        EXPECT_EQ(got.arguments.size(), exp.args.size()) << "Operation #" << i << " has wrong arguments size";

        for (size_t j = 0; j < exp.args.size(); ++j) {
            EXPECT_EQ(got.arguments[j], exp.args[j]) << "Operation #" << i << " argument #" << j << " mismatch";
        }
    }
}
