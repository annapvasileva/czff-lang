
#include <gtest/gtest.h>

#include "jit/jit_x86_64.hpp"
#include "common.hpp"

using namespace czffvm;
using namespace czffvm_jit;

void CompileAndExecute(RuntimeDataArea& rda, std::unique_ptr<czffvm_jit::X86JitCompiler>& jit, RuntimeFunction func, std::vector<Constant> constants, int32_t* stack) {
    for (auto con : constants) {
        rda.GetMethodArea().RegisterConstant(con);
    }

    czffvm_jit::X86JitHeapHelper heapHelper(rda);

    try {
        auto compiled_func = jit->CompileFunction(func);

        if (compiled_func) {
            using VMFunc = void(*)(int32_t*, czffvm_jit::X86JitHeapHelper*);
            VMFunc func_ptr = compiled_func->getFunction<VMFunc>();

            func_ptr(stack, &heapHelper);
        }
    } catch (const std::exception& e) {
        std::cout << "Error: " << e.what() << std::endl;
    }
}


TEST(BasicJITCompilationTestSuite, SimpleFunction) {
    auto rda = czffvm::RuntimeDataArea(10000);
    auto jit = std::make_unique<czffvm_jit::X86JitCompiler>();

    czffvm::RuntimeFunction func;
    func.locals_count = 0;
    func.max_stack = 4;
    func.code = {
        {czffvm::OperationCode::LDC, {0x67, 0x32, 0, 0}},
        {czffvm::OperationCode::RET, {}}
    };

    int32_t stack[4] = {0, 0, 0, 0};

    ASSERT_NO_THROW(CompileAndExecute(rda, jit, func, {}, stack));

    std::cout << "[TEST] After call:" << std::endl;
    for (int i = 0; i < 4; i++) {
        std::cout << "  stack[" << i << "] = " << stack[i] << std::endl;
    }

    ASSERT_EQ(stack[0], (((int32_t)0x32) << 8) + 0x67);
}

TEST(BasicJITCompilationTestSuite, BasicAddition) {
    auto rda = czffvm::RuntimeDataArea(10000);
    auto jit = std::make_unique<czffvm_jit::X86JitCompiler>();

    czffvm_jit::RuntimeFunction func;
    func.locals_count = 2;
    func.max_stack = 16;
    
    func.code = {
        {czffvm::OperationCode::LDV, {0}},  // LDV 0
        {czffvm::OperationCode::LDV, {1}},  // LDV 1
        {czffvm::OperationCode::ADD, {}},   // ADD
        {czffvm::OperationCode::RET, {}}    // RET
    };

    int32_t stack[4] = {10, 20, 0, 0};

    ASSERT_NO_THROW(CompileAndExecute(rda, jit, func, {}, stack));

    std::cout << "[TEST] After call:" << std::endl;
    for (int i = 0; i < 4; i++) {
        std::cout << "  stack[" << i << "] = " << stack[i] << std::endl;
    }

    ASSERT_EQ(stack[0], 30);
}

TEST(BasicJITCompilationTestSuite, ConstantAddition) {
    auto rda = czffvm::RuntimeDataArea(10000);
    auto jit = std::make_unique<czffvm_jit::X86JitCompiler>();

    czffvm::RuntimeFunction func;
    func.locals_count = 0;
    func.max_stack = 8;
    
    func.code = {
        {czffvm::OperationCode::LDC, {5, 0, 0, 0}},
        {czffvm::OperationCode::LDC, {3, 0, 0, 0}},
        {czffvm::OperationCode::ADD, {}},
        {czffvm::OperationCode::RET, {}}
    };

    int32_t stack[func.max_stack] = {0, 0, 0, 0, 0, 0, 0, 0};

    ASSERT_NO_THROW(CompileAndExecute(rda, jit, func, {}, stack));

    std::cout << "[TEST] After call:" << std::endl;
    for (int i = 0; i < func.max_stack; i++) {
        std::cout << "  stack[" << i << "] = " << stack[i] << std::endl;
    }

    ASSERT_EQ(stack[0], 8);
}

TEST(BasicJITCompilationTestSuite, Multiplication) {
    auto rda = czffvm::RuntimeDataArea(10000);
    auto jit = std::make_unique<czffvm_jit::X86JitCompiler>();

    czffvm::RuntimeFunction func;
    func.locals_count = 0;
    func.max_stack = 8;
    
    func.code = {
        {czffvm::OperationCode::LDC, {6, 0, 0, 0}},
        {czffvm::OperationCode::LDC, {7, 0, 0, 0}},
        {czffvm::OperationCode::MUL, {}},
        {czffvm::OperationCode::RET, {}}
    };

    int32_t stack[func.max_stack] = {10, 20, 0, 0, 0, 0, 0, 0};

    ASSERT_NO_THROW(CompileAndExecute(rda, jit, func, {}, stack));

    std::cout << "[TEST] After call:" << std::endl;
    for (int i = 0; i < func.max_stack; i++) {
        std::cout << "  stack[" << i << "] = " << stack[i] << std::endl;
    }

    ASSERT_EQ(stack[0], 42);
}

TEST(BasicJITCompilationTestSuite, ComplexExpression) {
    auto rda = czffvm::RuntimeDataArea(10000);
    auto jit = std::make_unique<czffvm_jit::X86JitCompiler>();

    czffvm::RuntimeFunction func;
    func.locals_count = 3;  // a, b, c
    func.max_stack = 16;
    
    func.code = {
        {czffvm::OperationCode::LDV, {0}},  // a
        {czffvm::OperationCode::LDV, {1}},  // b
        {czffvm::OperationCode::ADD, {}},   // a + b
        {czffvm::OperationCode::LDV, {2}},  // c
        {czffvm::OperationCode::MUL, {}},   // (a+b) * c
        {czffvm::OperationCode::RET, {}}
    };

    int32_t stack[func.max_stack] = {5, 3, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};

    ASSERT_NO_THROW(CompileAndExecute(rda, jit, func, {}, stack));

    std::cout << "[TEST] After call:" << std::endl;
    for (int i = 0; i < func.max_stack; i++) {
        std::cout << "  stack[" << i << "] = " << stack[i] << std::endl;
    }

    ASSERT_EQ(stack[0], 32);
}

TEST(BasicJITCompilationTestSuite, ArrayCreation) {
    auto rda = czffvm::RuntimeDataArea(10000);
    auto jit = std::make_unique<czffvm_jit::X86JitCompiler>();

    czffvm::RuntimeFunction func;
    func.locals_count = 2;  // a, b, c
    func.max_stack = 32;

    // arr = new int[3]

    func.code = {
        // --- new int[3] ---
        {czffvm::OperationCode::LDC,    {3}},        // size = 3
        {czffvm::OperationCode::NEWARR, {0x00, 0x01}}, // type_idx
    };

    int32_t stack[16] = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};

    ASSERT_NO_THROW(CompileAndExecute(
        rda,
        jit, 
        func, 
        {{czffvm::ConstantTag::I4, {0x02, 0x00, 0x00, 0x00}}, {czffvm::ConstantTag::U1, {'I'}}},
        stack
    ));

    std::cout << "[TEST] After call:" << std::endl;
    for (int i = 0; i < 16; i++) {
        std::cout << "  stack[" << i << "] = " << stack[i] << std::endl;
    }

    ASSERT_NO_THROW(rda.GetHeap().Get({1}));
    ASSERT_EQ(rda.GetHeap().Get({1}).type, "[I");
    ASSERT_EQ(rda.GetHeap().Get({1}).fields.size(), 3);
}

TEST(BasicJITCompilationTestSuite, ArrayOperations) {
    auto rda = czffvm::RuntimeDataArea(10000);
    auto jit = std::make_unique<czffvm_jit::X86JitCompiler>();

    czffvm::RuntimeFunction func;
    func.locals_count = 3;  // a, b, c
    func.max_stack = 32;

    // arr = new int[3]
    // arr[0] = a
    // arr[1] = b
    // arr[2] = c
    // return arr[0] + arr[1] * arr[2]

    func.code = {
        // --- new int[3] ---
        {czffvm::OperationCode::LDC,    {3}},        // size = 3
        {czffvm::OperationCode::NEWARR, {0x00, 0x01}}, // type_idx

        // --- arr[0] = a ---
        {czffvm::OperationCode::DUP, {}},            // arr
        {czffvm::OperationCode::LDC, {0}},            // index 0
        {czffvm::OperationCode::LDV, {0}},            // a
        {czffvm::OperationCode::STELEM, {}},

        // --- arr[1] = b ---
        {czffvm::OperationCode::DUP, {}},            // arr
        {czffvm::OperationCode::LDC, {1}},            // index 1
        {czffvm::OperationCode::LDV, {1}},            // b
        {czffvm::OperationCode::STELEM, {}},

        // --- arr[2] = c ---
        {czffvm::OperationCode::DUP, {}},            // arr
        {czffvm::OperationCode::LDC, {2}},            // index 2
        {czffvm::OperationCode::LDV, {2}},            // c
        {czffvm::OperationCode::STELEM, {}},

        // --- arr[0] + arr[1] * arr[2] ---
        {czffvm::OperationCode::DUP, {}},            // arr
        {czffvm::OperationCode::LDC, {0}},
        {czffvm::OperationCode::LDELEM, {}},          // arr[0]

        {czffvm::OperationCode::DUP, {}},            // arr
        {czffvm::OperationCode::LDC, {1}},
        {czffvm::OperationCode::LDELEM, {}},          // arr[1]

        {czffvm::OperationCode::DUP, {}},            // arr
        {czffvm::OperationCode::LDC, {2}},
        {czffvm::OperationCode::LDELEM, {}},          // arr[2]

        {czffvm::OperationCode::MUL, {}},             // arr[1] * arr[2]
        {czffvm::OperationCode::ADD, {}},             // arr[0] + (...)

        {czffvm::OperationCode::RET, {}}
    };

    int32_t stack[16] = {2, 3, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};

    ASSERT_NO_THROW(CompileAndExecute(
        rda,
        jit, 
        func, 
        {{czffvm::ConstantTag::I4, {0x02, 0x00, 0x00, 0x00}}, {czffvm::ConstantTag::U1, {'I'}}},
        stack
    ));

    std::cout << "[TEST] After call:" << std::endl;
    for (int i = 0; i < 16; i++) {
        std::cout << "  stack[" << i << "] = " << stack[i] << std::endl;
    }
}


