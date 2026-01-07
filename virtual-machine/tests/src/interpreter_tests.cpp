#include <fstream>
#include <gtest/gtest.h>

#include "runtime_data_area.hpp"
#include "class_loader.hpp"
#include "interpreter.hpp"
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

TEST(InterpreterIntegrationTestSuite, RunsEmptyMainWithoutCrashing) {
    RuntimeDataArea rda;
    ClassLoader loader(rda);
    Interpreter interpreter(rda);

    auto data = MakeMinimalBallWithMain();
    TempFile tmp("empty_main.ball");
    WriteFile(tmp.path, data);

    ASSERT_NO_THROW(loader.LoadProgram(tmp.path));

    EXPECT_NO_THROW(interpreter.Execute());
}

TEST(InterpreterIntegrationTestSuite, StackIsEmptyAfterExecution) {
    RuntimeDataArea rda;
    ClassLoader loader(rda);
    Interpreter interpreter(rda);

    auto data = MakeMinimalBallWithMain();
    TempFile tmp("stack_cleanup.ball");
    WriteFile(tmp.path, data);

    loader.LoadProgram(tmp.path);

    EXPECT_TRUE(rda.GetStack().Empty());

    interpreter.Execute();

    EXPECT_TRUE(rda.GetStack().Empty());
}

TEST(InterpreterIntegrationTestSuite, MainFrameIsCreatedAndDestroyed) {
    RuntimeDataArea rda;
    ClassLoader loader(rda);
    Interpreter interpreter(rda);

    auto data = MakeMinimalBallWithMain();
    TempFile tmp("frame_lifecycle.ball");
    WriteFile(tmp.path, data);

    loader.LoadProgram(tmp.path);

    EXPECT_TRUE(rda.GetStack().Empty());

    interpreter.Execute();

    EXPECT_TRUE(rda.GetStack().Empty());
}

TEST(InterpreterIntegrationTestSuite, ExecutesFirstProgramAndPrintsResult) {
    RuntimeDataArea rda;
    ClassLoader loader(rda);
    Interpreter interpreter(rda);

    auto data = MakeFirstProgramBall();
    TempFile tmp("first.ball");
    WriteFile(tmp.path, data);
    ASSERT_NO_THROW(loader.LoadProgram(tmp.path));

    RuntimeFunction* entry = loader.EntryPoint();
    ASSERT_NE(entry, nullptr);

    std::ostringstream captured;
    auto* old_buf = std::cout.rdbuf(captured.rdbuf());

    ASSERT_NO_THROW(interpreter.Execute());

    std::cout.rdbuf(old_buf);

    std::string output = captured.str();

    EXPECT_EQ(output, "5");
    EXPECT_TRUE(rda.GetStack().Empty());
}
