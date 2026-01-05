#include <gtest/gtest.h>
#include <fstream>
#include <cstdio>

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

//
// ---------- happy path ----------
//

TEST(InterpreterIntegrationTest, RunsEmptyMainWithoutCrashing) {
    RuntimeDataArea rda;
    ClassLoader loader(rda);
    Interpreter interpreter(rda);

    auto data = MakeMinimalBallWithMain();
    TempFile tmp("empty_main.ball");
    WriteFile(tmp.path, data);

    ASSERT_NO_THROW(loader.LoadProgram(tmp.path));

    EXPECT_NO_THROW(interpreter.Execute());
}

//
// ---------- stack behavior ----------
//

TEST(InterpreterIntegrationTest, StackIsEmptyAfterExecution) {
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

//
// ---------- entry point ----------
//

TEST(InterpreterIntegrationTest, MainFrameIsCreatedAndDestroyed) {
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

//
// ---------- error propagation ----------
//

TEST(InterpreterIntegrationTest, MissingMainPropagatesError) {
    RuntimeDataArea rda;
    ClassLoader loader(rda);
    Interpreter interpreter(rda);

    auto data = MakeMinimalBallWithoutMain();
    TempFile tmp("no_main.ball");
    WriteFile(tmp.path, data);

    EXPECT_THROW(loader.LoadProgram(tmp.path), ClassLoaderError);
}

//
// ---------- wrong signature ----------
//

TEST(InterpreterIntegrationTest, WrongMainSignatureDoesNotStartInterpreter) {
    RuntimeDataArea rda;
    ClassLoader loader(rda);
    Interpreter interpreter(rda);

    auto data = MakeMinimalBallWithWrongMainSignature();
    TempFile tmp("bad_sig.ball");
    WriteFile(tmp.path, data);

    EXPECT_THROW(loader.LoadProgram(tmp.path), ClassLoaderError);
}
