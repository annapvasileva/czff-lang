
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

#ifdef DEBUG_BUILD
    std::cout << "[TEST] After call:" << std::endl;
    for (int i = 0; i < 4; i++) {
        std::cout << "  stack[" << i << "] = " << stack[i] << std::endl;
    }
#endif

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

#ifdef DEBUG_BUILD
    std::cout << "[TEST] After call:" << std::endl;
    for (int i = 0; i < 4; i++) {
        std::cout << "  stack[" << i << "] = " << stack[i] << std::endl;
    }
#endif

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

#ifdef DEBUG_BUILD
    std::cout << "[TEST] After call:" << std::endl;
    for (int i = 0; i < func.max_stack; i++) {
        std::cout << "  stack[" << i << "] = " << stack[i] << std::endl;
    }
#endif

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

#ifdef DEBUG_BUILD
    std::cout << "[TEST] After call:" << std::endl;
    for (int i = 0; i < func.max_stack; i++) {
        std::cout << "  stack[" << i << "] = " << stack[i] << std::endl;
    }
#endif

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

#ifdef DEBUG_BUILD
    std::cout << "[TEST] After call:" << std::endl;
    for (int i = 0; i < func.max_stack; i++) {
        std::cout << "  stack[" << i << "] = " << stack[i] << std::endl;
    }
#endif

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
        {czffvm::OperationCode::NEWARR, {0x00, 0x00}}, // type_idx
    };

    int32_t stack[16] = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};

    ASSERT_NO_THROW(CompileAndExecute(
        rda,
        jit, 
        func, 
        {{czffvm::ConstantTag::U1, {'I'}}},
        stack
    ));

#ifdef DEBUG_BUILD
    std::cout << "[TEST] After call:" << std::endl;
    for (int i = 0; i < 16; i++) {
        std::cout << "  stack[" << i << "] = " << stack[i] << std::endl;
    }
#endif

    ASSERT_NO_THROW(rda.GetHeap().Get({1}));
    auto array = rda.GetHeap().Get({1});

    ASSERT_EQ(array.type, "[I");
    ASSERT_EQ(array.fields.size(), 3);
}

TEST(BasicJITCompilationTestSuite, ArrayStore) {
    auto rda = czffvm::RuntimeDataArea(10000);
    auto jit = std::make_unique<czffvm_jit::X86JitCompiler>();

    czffvm::RuntimeFunction func;
    func.locals_count = 2;  // a, b, c
    func.max_stack = 32;

    // arr = new int[3]

    func.code = {
        // --- new int[3] ---
        {czffvm::OperationCode::LDC,    {3}},        // size = 3
        {czffvm::OperationCode::NEWARR, {0x00, 0x00}}, // type_idx

        {czffvm::OperationCode::DUP, {}},            // arr
        {czffvm::OperationCode::LDC, {0}},            // index 0
        {czffvm::OperationCode::LDC, {12}},            // a
        {czffvm::OperationCode::STELEM, {}},
    };

    int32_t stack[16] = {12, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};

    ASSERT_NO_THROW(CompileAndExecute(
        rda,
        jit, 
        func, 
        {{czffvm::ConstantTag::U2, {'I', ';'}}},
        stack
    ));

#ifdef DEBUG_BUILD
    std::cout << "[TEST] After call:" << std::endl;
    for (int i = 0; i < 16; i++) {
        std::cout << "  stack[" << i << "] = " << stack[i] << std::endl;
    }
#endif

    ASSERT_NO_THROW(rda.GetHeap().Get({1}));
    auto array = rda.GetHeap().Get({1});

    ASSERT_EQ(array.type, "[I;");
    ASSERT_EQ(array.fields.size(), 3);

    uint32_t arr_elem;
    if (auto p = std::get_if<uint8_t>(&array.fields[0]))      arr_elem = *p;
    else if (auto p = std::get_if<uint16_t>(&array.fields[0])) arr_elem = *p;
    else if (auto p = std::get_if<uint32_t>(&array.fields[0])) arr_elem = *p;
    else if (auto p = std::get_if<int32_t>(&array.fields[0]))  arr_elem = static_cast<uint32_t>(*p);
    else {
        throw std::runtime_error("NEWARR: array size must be integer");
    }
    ASSERT_EQ(arr_elem, 12);

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
        // new int[3]
        {czffvm::OperationCode::LDC,    {8}},
        {czffvm::OperationCode::NEWARR, {0x00, 0x00}},   // arr

        // arr[0] = a
        {czffvm::OperationCode::DUP, {}},
        {czffvm::OperationCode::LDC, {0}},
        {czffvm::OperationCode::LDV, {0}},
        {czffvm::OperationCode::STELEM, {}},

        // arr[1] = b
        {czffvm::OperationCode::DUP, {}},
        {czffvm::OperationCode::LDC, {1}},
        {czffvm::OperationCode::LDV, {1}},
        {czffvm::OperationCode::STELEM, {}},

        // arr[2] = c
        {czffvm::OperationCode::DUP, {}},
        {czffvm::OperationCode::LDC, {2}},
        {czffvm::OperationCode::LDV, {2}},
        {czffvm::OperationCode::STELEM, {}},

        // ===== ВЫЧИСЛЕНИЕ =====

        // arr[1]
        {czffvm::OperationCode::DUP, {}},
        {czffvm::OperationCode::DUP, {}},        // arr arr
        {czffvm::OperationCode::LDC, {1}},       // arr arr 1
        {czffvm::OperationCode::LDELEM, {}},     // arr v1

        // arr[2]
        {czffvm::OperationCode::SWAP, {}},       // arr v1 arr
        {czffvm::OperationCode::LDC, {2}},       // arr v1 arr 2
        {czffvm::OperationCode::LDELEM, {}},     // arr v1 v2

        {czffvm::OperationCode::MUL, {}},        // arr (v1*v2)

        // arr[0]
        {czffvm::OperationCode::SWAP, {}},       // arr (v1*v2) arr
        {czffvm::OperationCode::LDC, {0}},       // arr (v1*v2) arr 0
        {czffvm::OperationCode::LDELEM, {}},     // arr (v1*v2) v0

        {czffvm::OperationCode::ADD, {}},        // arr result

        {czffvm::OperationCode::RET, {}}
    };


    int32_t stack[16] = {2, 3, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};

    ASSERT_NO_THROW(CompileAndExecute(
        rda,
        jit, 
        func, 
        {{czffvm::ConstantTag::U1, {'I'}}},
        stack
    ));

#ifdef DEBUG_BUILD
    std::cout << "[TEST] After call:" << std::endl;
    for (int i = 0; i < 16; i++) {
        std::cout << "  stack[" << i << "] = " << stack[i] << std::endl;
    }
#endif

    ASSERT_NO_THROW(rda.GetHeap().Get({1}));
    auto array = rda.GetHeap().Get({1});

    ASSERT_EQ(array.type, "[I");
    ASSERT_EQ(array.fields.size(), 8);
    ASSERT_EQ(stack[0], 14);
}


