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
    w.u2(0); // code len

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
    w.u2(0); // code len

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
    w.u2(0); // code len

    // ---- Classes ----
    w.u2(0);

    return w.b;
}
