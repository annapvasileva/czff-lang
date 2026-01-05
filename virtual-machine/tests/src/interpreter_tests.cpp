#include <gtest/gtest.h>

#include "runtime_data_area.hpp"
#include "class_loader.hpp"
#include "interpreter.hpp"

using namespace czffvm;

TEST(InterpreterIntegrationTestSuite, ExecutesFirstProgramAndPrintsResult) {
    RuntimeDataArea rda;
    ClassLoader loader(rda);
    Interpreter interpreter(rda);

    ASSERT_NO_THROW(loader.LoadProgram("FirstProgram.ball"));

    RuntimeFunction* entry = loader.EntryPoint();
    ASSERT_NE(entry, nullptr);

    std::ostringstream captured;
    auto* old_buf = std::cout.rdbuf(captured.rdbuf());

    ASSERT_NO_THROW(interpreter.Execute());

    std::cout.rdbuf(old_buf);

    std::string output = captured.str();

    EXPECT_EQ(output, "5\n");
}