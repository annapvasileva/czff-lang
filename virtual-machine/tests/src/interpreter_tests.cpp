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

    EXPECT_NO_THROW(interpreter.Execute(loader.EntryPoint()));
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

    interpreter.Execute(loader.EntryPoint());

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

    interpreter.Execute(loader.EntryPoint());

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

    ASSERT_NO_THROW(interpreter.Execute(loader.EntryPoint()));

    std::cout.rdbuf(old_buf);

    std::string output = captured.str();

    EXPECT_EQ(output, "5");
    EXPECT_TRUE(rda.GetStack().Empty());
}

TEST(InterpreterIntegrationTestSuite, ExecutesArrayProgram) {
    RuntimeDataArea rda;
    ClassLoader loader(rda);
    Interpreter interpreter(rda);

    auto data = MakeArrayProgramBall();
    TempFile tmp("array.ball");
    WriteFile(tmp.path,data);

    loader.LoadProgram(tmp.path);

    std::ostringstream out;
    auto* old = std::cout.rdbuf(out.rdbuf());

    interpreter.Execute(loader.EntryPoint());

    std::cout.rdbuf(old);

    EXPECT_EQ(out.str(), "-1212-1");
    EXPECT_TRUE(rda.GetStack().Empty());
}

TEST(InterpreterIntegrationTestSuite, ExecutesCallProgram) {
    RuntimeDataArea rda;
    ClassLoader loader(rda);
    Interpreter interpreter(rda);

    auto data = MakeCallProgramBall();
    TempFile tmp("call.ball");
    WriteFile(tmp.path,data);

    loader.LoadProgram(tmp.path);

    std::ostringstream out;
    auto* old = std::cout.rdbuf(out.rdbuf());

    interpreter.Execute(loader.EntryPoint());

    std::cout.rdbuf(old);

    EXPECT_EQ(out.str(), "3");
    EXPECT_TRUE(rda.GetStack().Empty());
}

TEST(InterpreterIntegrationTestSuite, ArrayIsMutatedInsideFunction) {
    RuntimeDataArea rda;
    ClassLoader loader(rda);
    Interpreter interpreter(rda);

    auto data = MakeArrayMutationCallBall();
    TempFile tmp("array_mut.ball");
    WriteFile(tmp.path,data);

    loader.LoadProgram(tmp.path);

    std::ostringstream out;
    auto* old = std::cout.rdbuf(out.rdbuf());

    interpreter.Execute(loader.EntryPoint());

    std::cout.rdbuf(old);

    EXPECT_EQ(out.str(),"42");
    EXPECT_TRUE(rda.GetStack().Empty());
}

TEST(InterpreterJumpTests, JmpSkipsCode) {
    RuntimeDataArea rda;
    ClassLoader loader(rda);
    Interpreter i(rda);

    auto data = MakeJmpProgramBall();
    TempFile tmp("jmp.ball");
    WriteFile(tmp.path,data);

    loader.LoadProgram(tmp.path);

    std::ostringstream out;
    auto* old = std::cout.rdbuf(out.rdbuf());

    i.Execute(loader.EntryPoint());
    std::cout.rdbuf(old);

    EXPECT_EQ(out.str(),"13");
}

TEST(InterpreterJumpTests, JzWorks) {
    RuntimeDataArea rda;
    ClassLoader loader(rda);
    Interpreter i(rda);

    auto data = MakeJzProgramBall();
    TempFile tmp("jz.ball");
    WriteFile(tmp.path,data);

    loader.LoadProgram(tmp.path);

    std::ostringstream out;
    auto* old = std::cout.rdbuf(out.rdbuf());

    i.Execute(loader.EntryPoint());
    std::cout.rdbuf(old);

    EXPECT_EQ(out.str(),"2");
}

TEST(InterpreterJumpTests, JnzWorks) {
    RuntimeDataArea rda;
    ClassLoader loader(rda);
    Interpreter i(rda);

    auto data = MakeJnzProgramBall();
    TempFile tmp("jnz.ball");
    WriteFile(tmp.path,data);

    loader.LoadProgram(tmp.path);

    std::ostringstream out;
    auto* old = std::cout.rdbuf(out.rdbuf());

    i.Execute(loader.EntryPoint());
    std::cout.rdbuf(old);

    EXPECT_EQ(out.str(),"3");
}

TEST(InterpreterIntegrationTestSuite, ExecutesFactorialProgram) {
    RuntimeDataArea rda;
    ClassLoader loader(rda);
    Interpreter interpreter(rda);

    auto data = MakeFactorialProgramBall();
    TempFile tmp("factorial.ball");
    WriteFile(tmp.path,data);

    loader.LoadProgram(tmp.path);

    std::ostringstream out;
    auto* old = std::cout.rdbuf(out.rdbuf());

    interpreter.Execute(loader.EntryPoint());

    std::cout.rdbuf(old);

    EXPECT_EQ(out.str(), "720");
    EXPECT_TRUE(rda.GetStack().Empty());
}

TEST(InterpreterIntegrationTestSuite, ExecutesSecondFactorialProgram) {
    RuntimeDataArea rda;
    ClassLoader loader(rda);
    Interpreter interpreter(rda);

    auto data = MakeFactorialProgramBall2();
    TempFile tmp("factorial2.ball");
    WriteFile(tmp.path,data);

    loader.LoadProgram(tmp.path);

    std::ostringstream out;
    auto* old = std::cout.rdbuf(out.rdbuf());

    interpreter.Execute(loader.EntryPoint());

    std::cout.rdbuf(old);

    EXPECT_EQ(out.str(), "720");
    EXPECT_TRUE(rda.GetStack().Empty());
}

TEST(InterpreterIntegrationTestSuite, ExecutesConditionalProgram) {
    RuntimeDataArea rda;
    ClassLoader loader(rda);
    Interpreter interpreter(rda);

    auto data = MakeConditionalProgramBall();
    TempFile tmp("conditional.ball");
    WriteFile(tmp.path,data);

    loader.LoadProgram(tmp.path);

    std::ostringstream out;
    auto* old = std::cout.rdbuf(out.rdbuf());

    interpreter.Execute(loader.EntryPoint());

    std::cout.rdbuf(old);

    EXPECT_EQ(out.str(), "102");
    EXPECT_TRUE(rda.GetStack().Empty());
}

TEST(InterpreterIntegrationTestSuite, ExecutesArrayWithForProgram) {
    RuntimeDataArea rda;
    ClassLoader loader(rda);
    Interpreter interpreter(rda);

    auto data = MakeArrayWithForProgramBall();
    TempFile tmp("array_and_for.ball");
    WriteFile(tmp.path,data);

    loader.LoadProgram(tmp.path);

    std::ostringstream out;
    auto* old = std::cout.rdbuf(out.rdbuf());

    interpreter.Execute(loader.EntryPoint());

    std::cout.rdbuf(old);

    EXPECT_EQ(out.str(), "4545454545");
    EXPECT_TRUE(rda.GetStack().Empty());
}
