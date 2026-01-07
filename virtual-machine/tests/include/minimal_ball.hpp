#pragma once
#include "ball_builder.hpp"
#include "common.hpp"

inline std::vector<uint8_t> MakeMinimalBallWithMain() {
    using namespace ball;
    Builder w;

    // ---- Header ----
    w.u4(0x62616c6c);
    w.u1(1); w.u1(0); w.u1(0);
    w.u1(0);

    // ---- Constant pool ----
    w.u2(3);

    // 0: "Main"
    w.u1(static_cast<uint8_t>(czffvm::ConstantTag::STRING));
    w.string("Main");

    // 1: params ""
    w.u1(static_cast<uint8_t>(czffvm::ConstantTag::STRING));
    w.string("");

    // 2: return "void"
    w.u1(static_cast<uint8_t>(czffvm::ConstantTag::STRING));
    w.string("void");

    // ---- Functions ----
    w.u2(1);
    w.u2(0); // name
    w.u2(1); // params
    w.u2(2); // return
    w.u2(1); // max_stack
    w.u2(0); // locals
    w.u2(1); // code len
    w.u2(static_cast<uint16_t>(czffvm::OperationCode::RET));

    // ---- Classes ----
    w.u2(0);

    return w.b;
}

inline std::vector<uint8_t> MakeMinimalBallWithoutMain() {
    using namespace ball;
    Builder w;

    // ---- Header ----
    w.u4(0x62616c6c);
    w.u1(1); w.u1(0); w.u1(0);
    w.u1(0);

    // ---- Constant pool ----
    w.u2(3);

    // 0: "Main"
    w.u1(static_cast<uint8_t>(czffvm::ConstantTag::STRING));
    w.string("Foo");

    // 1: params ""
    w.u1(static_cast<uint8_t>(czffvm::ConstantTag::STRING));
    w.string("");

    // 2: return "void"
    w.u1(static_cast<uint8_t>(czffvm::ConstantTag::STRING));
    w.string("void");

    // ---- Functions ----
    w.u2(1);
    w.u2(0); // name
    w.u2(1); // params
    w.u2(2); // return
    w.u2(1); // max_stack
    w.u2(0); // locals
    w.u2(1); // code len
    w.u2(static_cast<uint16_t>(czffvm::OperationCode::RET));

    // ---- Classes ----
    w.u2(0);

    return w.b;
}

inline std::vector<uint8_t> MakeMinimalBallWithWrongMainSignature() {
    using namespace ball;
    Builder w;

    // ---- Header ----
    w.u4(0x62616c6c);
    w.u1(1); w.u1(0); w.u1(0);
    w.u1(0);

    // ---- Constant pool ----
    w.u2(3);

    // 0: "Main"
    w.u1(static_cast<uint8_t>(czffvm::ConstantTag::STRING));
    w.string("Main");

    // 1: params ""
    w.u1(static_cast<uint8_t>(czffvm::ConstantTag::STRING));
    w.string("");

    // 2: return "void"
    w.u1(static_cast<uint8_t>(czffvm::ConstantTag::STRING));
    w.string("int");

    // ---- Functions ----
    w.u2(1);
    w.u2(0); // name
    w.u2(1); // params
    w.u2(2); // return
    w.u2(1); // max_stack
    w.u2(0); // locals
    w.u2(1); // code len
    w.u2(static_cast<uint16_t>(czffvm::OperationCode::RET));

    // ---- Classes ----
    w.u2(0);

    return w.b;
}

inline std::vector<uint8_t> MakeFirstProgramBall() {
    using namespace ball;
    Builder w;

    // ---------- Header ----------
    w.u4(0x62616c6c);      // magic "ball"
    w.u1(1); w.u1(0); w.u1(0); // version
    w.u1(0);               // flags

    // ---------- Constant pool ----------
    w.u2(5);

    // 0: "Main"
    w.u1(static_cast<uint8_t>(czffvm::ConstantTag::STRING));
    w.string("Main");

    // 1: params ""
    w.u1(static_cast<uint8_t>(czffvm::ConstantTag::STRING));
    w.string("");

    // 2: return "void"
    w.u1(static_cast<uint8_t>(czffvm::ConstantTag::STRING));
    w.string("void");

    // 3: int 2
    w.u1(static_cast<uint8_t>(czffvm::ConstantTag::I4));
    w.u4(2);

    // 4: int 3
    w.u1(static_cast<uint8_t>(czffvm::ConstantTag::I4));
    w.u4(3);

    // ---------- Functions ----------
    w.u2(1);       // functions count

    w.u2(0);       // name_index ("Main")
    w.u2(1);       // params_index ("")
    w.u2(2);       // return_index ("void")

    w.u2(2);       // max_stack (a,b for ADD)
    w.u2(3);       // locals_count (a,b,c)

    w.u2(11);      // code length

    auto op = [&](czffvm::OperationCode c) {
        w.u2(static_cast<uint16_t>(c));
    };
    auto op2 = [&](czffvm::OperationCode c, uint16_t arg) {
        w.u2(static_cast<uint16_t>(c));
        w.u1(arg >> 8);
        w.u1(arg & 0xFF);
    };

    op2(czffvm::OperationCode::LDC, 3);
    op2(czffvm::OperationCode::STORE, 0);
    op2(czffvm::OperationCode::LDC, 4);
    op2(czffvm::OperationCode::STORE, 1);
    op2(czffvm::OperationCode::LDV, 0);
    op2(czffvm::OperationCode::LDV, 1);
    op (czffvm::OperationCode::ADD);
    op2(czffvm::OperationCode::STORE, 2);
    op2(czffvm::OperationCode::LDV, 2);
    op (czffvm::OperationCode::PRINT);
    op (czffvm::OperationCode::RET);

    // ---------- Classes ----------
    w.u2(0); // no classes

    return w.b;
}

