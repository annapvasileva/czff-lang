#include <gtest/gtest.h>

#include "common.hpp"
#include "jit/generic_jit_optimizer.hpp"

using namespace czffvm;
using namespace czffvm_jit;

class GenericJitOptimizerTest : public testing::Test {
protected:
    MethodArea method_area;
    std::vector<Operation> code;

    void runDCE() {
        GenericJitOptimizer opt(code, method_area);
        opt.BuildControlFlowGraph();
        opt.MarkReachableBlocks();
        opt.RemoveDeadCode();
        opt.CompactCode();
    }

    void runDSE() {
        GenericJitOptimizer opt(code, method_area);
        opt.DeadStackElimination();
    }

    void runRRJ() {
        GenericJitOptimizer opt(code, method_area);
        opt.RemoveRedundantJumps();
    }


    Operation makeLDC(int value) {
        Constant c{ConstantTag::U2, {uint8_t((value >> 8) & 0xFF), uint8_t(value & 0xFF)}};
        int idx = method_area.RegisterConstant(c);
        return Operation{
            OperationCode::LDC,
            {uint8_t((idx >> 8) & 0xFF), uint8_t(idx & 0xFF)}
        };
    }


    Operation makeJump(OperationCode op, uint16_t target_addr) {
        return Operation{
            op,
            {static_cast<uint8_t>((target_addr >> 8) & 0xFF),
            static_cast<uint8_t>(target_addr & 0xFF)}
        };
    }

    Operation makeOp(OperationCode op) {
        return Operation{op, {}};
    }
};

TEST_F(GenericJitOptimizerTest, RemovesUnreachableBlock) {
    // 0: JMP -> 2
    // 1: LDC 1      (dead)
    // 2: RET

    code = {
        makeJump(OperationCode::JMP, 2),
        makeLDC(1),
        makeOp(OperationCode::RET)
    };

    runDCE();

    ASSERT_EQ(code.size(), 2);
    EXPECT_EQ(code[0].code, OperationCode::JMP);
    EXPECT_EQ(code[1].code, OperationCode::RET);
}

TEST_F(GenericJitOptimizerTest, RemovesCodeAfterUnconditionalJump) {
    // 0: JMP -> 2
    // 1: LDC 42     (dead)
    // 2: RET

    code = {
        makeJump(OperationCode::JMP, 2),
        makeLDC(42),
        makeOp(OperationCode::RET)
    };

    runDCE();

    ASSERT_EQ(code.size(), 2);
    EXPECT_EQ(code[1].code, OperationCode::RET);
}


TEST_F(GenericJitOptimizerTest, ConditionalJumpKeepsFallthrough) {
    // 0: JZ -> 3
    // 1: LDC 1
    // 2: RET
    // 3: RET

    code = {
        makeJump(OperationCode::JZ, 3),
        makeLDC(1),
        makeOp(OperationCode::RET),
        makeOp(OperationCode::RET)
    };

    runDCE();

    ASSERT_EQ(code.size(), 4);
    EXPECT_EQ(code[0].code, OperationCode::JZ);
    EXPECT_EQ(code[1].code, OperationCode::LDC);
    EXPECT_EQ(code[2].code, OperationCode::RET);
}


TEST_F(GenericJitOptimizerTest, JumpTargetIsUpdatedAfterCompaction) {
    // 0: JMP -> 3
    // 1: LDC 1      (dead)
    // 2: LDC 2      (dead)
    // 3: RET

    code = {
        makeJump(OperationCode::JMP, 3),
        makeLDC(1),
        makeLDC(2),
        makeOp(OperationCode::RET)
    };

    runDCE();

    ASSERT_EQ(code.size(), 2);

    const Operation& jmp = code[0];
    int target_addr = (jmp.arguments[0] << 8) | jmp.arguments[1];

    EXPECT_EQ(target_addr, 1);
}



TEST_F(GenericJitOptimizerTest, RemovesDeadTail) {
    // 0: RET
    // 1: ADD (dead)
    // 2: ADD (dead)

    code = {
        makeOp(OperationCode::RET),
        makeOp(OperationCode::ADD),
        makeOp(OperationCode::ADD)
    };

    runDCE();

    ASSERT_EQ(code.size(), 1);
    EXPECT_EQ(code[0].code, OperationCode::RET);
}

TEST_F(GenericJitOptimizerTest, LoopBackwardJump) {
    // 0: LDC 0
    // 1: LDC 1
    // 2: JNZ -> 1
    // 3: RET

    code = {
        makeLDC(0),
        makeLDC(1),
        makeJump(OperationCode::JNZ, 1),
        makeOp(OperationCode::RET)
    };

    runDCE();

    ASSERT_EQ(code.size(), 4);
    EXPECT_EQ(code[2].code, OperationCode::JNZ);
}



TEST_F(GenericJitOptimizerTest, MultipleJumpsToSameBlock) {
    // 0: JZ -> 4
    // 1: JMP -> 4
    // 2: ADD (dead)
    // 3: ADD (dead)
    // 4: RET

    code = {
        makeJump(OperationCode::JZ, 4),
        makeJump(OperationCode::JMP, 4),
        makeOp(OperationCode::ADD),
        makeOp(OperationCode::ADD),
        makeOp(OperationCode::RET)
    };

    runDCE();

    // Должны остаться только переходы и RET
    ASSERT_EQ(code.size(), 3);
    EXPECT_EQ(code[0].code, OperationCode::JZ);
    EXPECT_EQ(code[1].code, OperationCode::JMP);
    EXPECT_EQ(code[2].code, OperationCode::RET);

    for (int i = 0; i < 2; ++i) {
        const Operation& instr = code[i];
        int target_addr = (instr.arguments[0] << 8) | instr.arguments[1];
        EXPECT_EQ(target_addr, 2); // target теперь на RET
    }
}

TEST_F(GenericJitOptimizerTest, JumpToMiddleOfBlock) {
    // 0: LDC 0
    // 1: LDC 1
    // 2: LDC 2
    // 3: JMP -> 1
    // 4: RET (dead)

    code = {
        makeLDC(0),
        makeLDC(1),
        makeLDC(2),
        makeJump(OperationCode::JMP, 1), // jump на инструкцию с индексом 1
        makeOp(OperationCode::RET)
    };

    runDCE();

    ASSERT_EQ(code.size(), 4);
    EXPECT_EQ(code[3].code, OperationCode::JMP);

    // Проверим, что target остался 1
    int hi = code[3].arguments[0];
    int lo = code[3].arguments[1];
    int target = (hi << 8) | lo;
    EXPECT_EQ(target, 1);
}



TEST_F(GenericJitOptimizerTest, ConditionalFollowedByUnconditional) {
    // 0: JZ -> 3
    // 1: ADD
    // 2: JMP -> 4
    // 3: ADD
    // 4: RET

    code = {
        makeJump(OperationCode::JZ, 3),
        makeOp(OperationCode::ADD),
        makeJump(OperationCode::JMP, 4),
        makeOp(OperationCode::ADD),
        makeOp(OperationCode::RET)
    };

    runDCE();

    ASSERT_EQ(code.size(), 5);

    // Проверяем что инструкции после безусловного JMP не удаляются неправильно
    EXPECT_EQ(code[2].code, OperationCode::JMP);
    EXPECT_EQ(code[3].code, OperationCode::ADD);
}


TEST_F(GenericJitOptimizerTest, FullyUnreachableBlock) {
    // 0: RET
    // 1: ADD (unreachable)
    // 2: JMP -> 4 (unreachable)
    // 3: ADD (unreachable)
    // 4: RET

    code = {
        makeOp(OperationCode::RET),
        makeOp(OperationCode::ADD),
        makeJump(OperationCode::JMP, 4),
        makeOp(OperationCode::ADD),
        makeOp(OperationCode::RET)
    };

    runDCE();

    // Должны остаться только reachable instructions
    ASSERT_EQ(code.size(), 1);
    EXPECT_EQ(code[0].code, OperationCode::RET);
}





class ConstantFoldingTest : public testing::Test {
protected:
    MethodArea method_area;
    std::vector<Operation> code;

    void runFolding() {
        GenericJitOptimizer opt(code, method_area);
        opt.ConstantFolding();  // вызов folding
    }

    Operation makeLDC(int value) {
        Constant c{ConstantTag::U2, {uint8_t((value >> 8) & 0xFF), uint8_t(value & 0xFF)}};
        int idx = method_area.RegisterConstant(c);
        return Operation{OperationCode::LDC, {uint8_t((idx >> 8) & 0xFF), uint8_t(idx & 0xFF)}};
    }

    Operation makeLDV(int local_index) {
        return Operation{OperationCode::LDV, {0, uint8_t(local_index)}};
    }

    Operation makeOp(OperationCode op) {
        return Operation{op, {}};
    }
};

TEST_F(ConstantFoldingTest, SimpleAdd) {
    // 2 + 3
    code = {
        makeLDC(2),
        makeLDC(3),
        makeOp(OperationCode::ADD),
        makeOp(OperationCode::RET)
    };

    runFolding();

    ASSERT_EQ(code.size(), 2);
    EXPECT_EQ(code[0].code, OperationCode::LDC);

    int idx = (code[0].arguments[0] << 8) | code[0].arguments[1];
    const Constant& c = method_area.GetConstant(idx);
    int val = (c.data[0] << 24) | (c.data[1] << 16) | (c.data[2] << 8) | c.data[3];
    EXPECT_EQ(val, 5);

    EXPECT_EQ(code[1].code, OperationCode::RET);
}

TEST_F(ConstantFoldingTest, AddThenMul) {
    // (2 + 3) * 4
    code = {
        makeLDC(2),
        makeLDC(3),
        makeOp(OperationCode::ADD),
        makeLDC(4),
        makeOp(OperationCode::MUL),
        makeOp(OperationCode::RET)
    };

    runFolding();

    // После folding: LDC 5, LDC 4, MUL
    ASSERT_EQ(code.size(), 2);
    EXPECT_EQ(code[0].code, OperationCode::LDC);

    int idx = (code[0].arguments[0] << 8) | code[0].arguments[1];
    const Constant& c = method_area.GetConstant(idx);
    int val = (c.data[0] << 24) | (c.data[1] << 16) | (c.data[2] << 8) | c.data[3];
    EXPECT_EQ(val, 20);

    EXPECT_EQ(code[1].code, OperationCode::RET);
}


TEST_F(ConstantFoldingTest, AddWithVariable) {
    // LDV a + LDC 3
    code = {
        makeLDV(0),       // неизвестное значение
        makeLDC(3),
        makeOp(OperationCode::ADD),
        makeOp(OperationCode::RET)
    };

    runFolding();

    // Ничего не сворачивается, код остается
    ASSERT_EQ(code.size(), 4);
    EXPECT_EQ(code[0].code, OperationCode::LDV);
    EXPECT_EQ(code[1].code, OperationCode::LDC);
    EXPECT_EQ(code[2].code, OperationCode::ADD);
}


TEST_F(ConstantFoldingTest, ChainAdd) {
    // 1 + 2 + 3
    code = {
        makeLDC(1),
        makeLDC(2),
        makeOp(OperationCode::ADD),
        makeLDC(3),
        makeOp(OperationCode::ADD),
        makeOp(OperationCode::RET)
    };

    runFolding();

    // Сначала 1+2 -> 3, затем 3+3 -> 6
    ASSERT_EQ(code.size(), 2);
    int idx = (code[0].arguments[0] << 8) | code[0].arguments[1];
    const Constant& c = method_area.GetConstant(idx);
    int val = (c.data[0] << 24) | (c.data[1] << 16) | (c.data[2] << 8) | c.data[3];
    EXPECT_EQ(val, 6);

    EXPECT_EQ(code[1].code, OperationCode::RET);
}

TEST_F(GenericJitOptimizerTest, DSE_RemovesUnusedLDC) {
    // 0: LDC 10
    // 1: RET

    code = {
        makeLDC(10),
        makeOp(OperationCode::RET)
    };

    runDSE();

    ASSERT_EQ(code.size(), 2);
    EXPECT_EQ(code[0].code, OperationCode::LDC);
    EXPECT_EQ(code[1].code, OperationCode::RET);
}

TEST_F(GenericJitOptimizerTest, DSE_RemovesUnusedArithmeticChain) {
    // 0: LDC 1
    // 1: LDC 2
    // 2: ADD
    // 3: RET

    code = {
        makeLDC(1),
        makeLDC(2),
        makeOp(OperationCode::ADD),
        makeOp(OperationCode::RET)
    };

    runDSE();

    ASSERT_EQ(code.size(), 4);
    EXPECT_EQ(code[0].code, OperationCode::LDC);
    EXPECT_EQ(code[1].code, OperationCode::LDC);
    EXPECT_EQ(code[2].code, OperationCode::ADD);
    EXPECT_EQ(code[3].code, OperationCode::RET);
}

TEST_F(GenericJitOptimizerTest, DSE_KeepsValueUsedByPrint) {
    // 0: LDC 5
    // 1: PRINT
    // 2: RET

    code = {
        makeLDC(5),
        makeOp(OperationCode::PRINT),
        makeOp(OperationCode::RET)
    };

    runDSE();

    ASSERT_EQ(code.size(), 3);
    EXPECT_EQ(code[0].code, OperationCode::LDC);
    EXPECT_EQ(code[1].code, OperationCode::PRINT);
    EXPECT_EQ(code[2].code, OperationCode::RET);
}

TEST_F(GenericJitOptimizerTest, DSE_KeepsValueStored) {
    // 0: LDC 42
    // 1: STORE 0
    // 2: RET

    code = {
        makeLDC(42),
        Operation{OperationCode::STORE, {0, 0}},
        makeOp(OperationCode::RET)
    };

    runDSE();

    ASSERT_EQ(code.size(), 3);
    EXPECT_EQ(code[0].code, OperationCode::LDC);
    EXPECT_EQ(code[1].code, OperationCode::STORE);
    EXPECT_EQ(code[2].code, OperationCode::RET);
}

TEST_F(GenericJitOptimizerTest, DSE_DupOneValueUnused) {
    // 0: LDC 7
    // 1: DUP
    // 2: PRINT
    // 3: RET

    code = {
        makeLDC(7),
        makeOp(OperationCode::DUP),
        makeOp(OperationCode::PRINT),
        makeOp(OperationCode::RET)
    };

    runDSE();

    // DUP нужен, но второй LDC не удаляется
    ASSERT_EQ(code.size(), 4);
    EXPECT_EQ(code[1].code, OperationCode::DUP);
}

TEST_F(GenericJitOptimizerTest, DSE_KeepsUsedArithmetic) {
    // 0: LDC 2
    // 1: LDC 3
    // 2: MUL
    // 3: PRINT
    // 4: RET

    code = {
        makeLDC(2),
        makeLDC(3),
        makeOp(OperationCode::MUL),
        makeOp(OperationCode::PRINT),
        makeOp(OperationCode::RET)
    };

    runDSE();

    ASSERT_EQ(code.size(), 5);
    EXPECT_EQ(code[2].code, OperationCode::MUL);
}

TEST_F(GenericJitOptimizerTest, DSE_ClearsStackOnJump) {
    // 0: LDC 1
    // 1: JMP -> 3
    // 2: LDC 2 (dead)
    // 3: RET

    code = {
        makeLDC(1),
        makeJump(OperationCode::JMP, 3),
        makeLDC(2),
        makeOp(OperationCode::RET)
    };

    runDSE();
    runDCE();
    runRRJ();

    ASSERT_EQ(code.size(), 2);
    EXPECT_EQ(code[0].code, OperationCode::LDC);
    EXPECT_EQ(code[1].code, OperationCode::RET);
}

TEST_F(GenericJitOptimizerTest, DSE_RemovesUnusedLDV) {
    // 0: LDV 0
    // 1: RET

    code = {
        Operation{OperationCode::LDV, {0, 0}},
        makeOp(OperationCode::RET)
    };

    runDSE();

    ASSERT_EQ(code.size(), 2);
    EXPECT_EQ(code[0].code, OperationCode::LDV);
    EXPECT_EQ(code[1].code, OperationCode::RET);
}





