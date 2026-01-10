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
    w.string("void;");

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
    w.string("void;");

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
    w.string("I;");

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
    w.string("void;");

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
    w.u2(2);       // return_index ("void;")

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

inline std::vector<uint8_t> MakeArrayProgramBall() {
    using namespace ball;
    Builder w;

    // ---------- Header ----------
    w.u4(0x62616c6c);
    w.u1(1); w.u1(0); w.u1(0);
    w.u1(0);

    // ---------- Constant pool ----------
    w.u2(11);

    // 0 "Main"
    w.u1((uint8_t)czffvm::ConstantTag::STRING);
    w.string("Main");

    // 1 params ""
    w.u1((uint8_t)czffvm::ConstantTag::STRING);
    w.string("");

    // 2 return "void"
    w.u1((uint8_t)czffvm::ConstantTag::STRING);
    w.string("void;");

    // 3 int 5
    w.u1((uint8_t)czffvm::ConstantTag::I4); w.u4(5);

    // 4 type "[I;"
    w.u1((uint8_t)czffvm::ConstantTag::STRING);
    w.string("[I;");

    // values
    w.u1((uint8_t)czffvm::ConstantTag::I4); w.u4(0); // 5
    w.u1((uint8_t)czffvm::ConstantTag::I4); w.u4(-1); // 6
    w.u1((uint8_t)czffvm::ConstantTag::I4); w.u4(1); // 7
    w.u1((uint8_t)czffvm::ConstantTag::I4); w.u4(2); // 8
    w.u1((uint8_t)czffvm::ConstantTag::I4); w.u4(3); // 9
    w.u1((uint8_t)czffvm::ConstantTag::I4); w.u4(4); // 10

    // ---------- Functions ----------
    w.u2(1);

    w.u2(0); // Main
    w.u2(1); // ""
    w.u2(2); // void;

    w.u2(6); // max stack
    w.u2(2); // locals

    std::vector<uint16_t> code;

    auto op  = [&](auto c){ code.push_back((uint16_t)c); };
    auto op2 = [&](auto c,uint16_t a){
        code.push_back((uint16_t)c);
        code.push_back(a);
    };

    // n = 5
    op2(czffvm::OperationCode::LDC,3);
    op2(czffvm::OperationCode::STORE,0);

    // arr = new int[n]
    op2(czffvm::OperationCode::LDV,0);
    op2(czffvm::OperationCode::NEWARR,4);
    op2(czffvm::OperationCode::STORE,1);

    // arr[0] = -1
    op2(czffvm::OperationCode::LDV,1);
    op2(czffvm::OperationCode::LDC,5);
    op2(czffvm::OperationCode::LDC,6);
    op (czffvm::OperationCode::STELEM);

    // arr[1] = 2
    op2(czffvm::OperationCode::LDV,1);
    op2(czffvm::OperationCode::LDC,7);
    op2(czffvm::OperationCode::LDC,8);
    op (czffvm::OperationCode::STELEM);

    // arr[2] = arr[0] + arr[1]
    op2(czffvm::OperationCode::LDV,1);
    op2(czffvm::OperationCode::LDC,8);

    op2(czffvm::OperationCode::LDV,1);
    op2(czffvm::OperationCode::LDC,5);
    op (czffvm::OperationCode::LDELEM);

    op2(czffvm::OperationCode::LDV,1);
    op2(czffvm::OperationCode::LDC,7);
    op (czffvm::OperationCode::LDELEM);

    op (czffvm::OperationCode::ADD);
    op (czffvm::OperationCode::STELEM);

    // arr[3] = -(arr[0] * arr[1])
    op2(czffvm::OperationCode::LDV,1);
    op2(czffvm::OperationCode::LDC,9);

    op2(czffvm::OperationCode::LDV,1);
    op2(czffvm::OperationCode::LDC,5);
    op (czffvm::OperationCode::LDELEM);

    op2(czffvm::OperationCode::LDV,1);
    op2(czffvm::OperationCode::LDC,7);
    op (czffvm::OperationCode::LDELEM);

    op (czffvm::OperationCode::MUL);
    op (czffvm::OperationCode::MIN);
    op (czffvm::OperationCode::STELEM);

    // arr[4] = arr[0] * (arr[1]+arr[2]) + arr[3]
    op2(czffvm::OperationCode::LDV,1);
    op2(czffvm::OperationCode::LDC,10);

    op2(czffvm::OperationCode::LDV,1);
    op2(czffvm::OperationCode::LDC,5);
    op (czffvm::OperationCode::LDELEM);

    op2(czffvm::OperationCode::LDV,1);
    op2(czffvm::OperationCode::LDC,7);
    op (czffvm::OperationCode::LDELEM);

    op2(czffvm::OperationCode::LDV,1);
    op2(czffvm::OperationCode::LDC,8);
    op (czffvm::OperationCode::LDELEM);

    op (czffvm::OperationCode::ADD);
    op (czffvm::OperationCode::MUL);

    op2(czffvm::OperationCode::LDV,1);
    op2(czffvm::OperationCode::LDC,9);
    op (czffvm::OperationCode::LDELEM);

    op (czffvm::OperationCode::ADD);
    op (czffvm::OperationCode::STELEM);

    // print arr[0..4]
    op2(czffvm::OperationCode::LDV,1);
    op2(czffvm::OperationCode::LDC,5);
    op (czffvm::OperationCode::LDELEM);
    op (czffvm::OperationCode::PRINT);

    op2(czffvm::OperationCode::LDV,1);
    op2(czffvm::OperationCode::LDC,7);
    op (czffvm::OperationCode::LDELEM);
    op (czffvm::OperationCode::PRINT);

    op2(czffvm::OperationCode::LDV,1);
    op2(czffvm::OperationCode::LDC,8);
    op (czffvm::OperationCode::LDELEM);
    op (czffvm::OperationCode::PRINT);

    op2(czffvm::OperationCode::LDV,1);
    op2(czffvm::OperationCode::LDC,9);
    op (czffvm::OperationCode::LDELEM);
    op (czffvm::OperationCode::PRINT);

    op2(czffvm::OperationCode::LDV,1);
    op2(czffvm::OperationCode::LDC,10);
    op (czffvm::OperationCode::LDELEM);
    op (czffvm::OperationCode::PRINT);

    op(czffvm::OperationCode::RET);

    // count instructions
    size_t instr = 0;
    for (size_t i=0;i<code.size();) {
        instr++;
        auto o=(czffvm::OperationCode)code[i];
        switch(o){
            case czffvm::OperationCode::LDC:
            case czffvm::OperationCode::STORE:
            case czffvm::OperationCode::LDV:
            case czffvm::OperationCode::NEWARR:
            case czffvm::OperationCode::CALL:
            case czffvm::OperationCode::HALT:
                i+=2; break;
            default:
                i+=1;
        }
    }

    w.u2(instr);
    for(auto x:code) w.u2(x);

    // ---------- Classes ----------
    w.u2(0);

    return w.b;
}

inline std::vector<uint8_t> MakeCallProgramBall() {
    using namespace ball;
    Builder w;

    // ---------- Header ----------
    w.u4(0x62616c6c);
    w.u1(1); w.u1(0); w.u1(0);
    w.u1(0);

    // ---------- Constant pool ----------
    w.u2(8);

    // 0 "Sum"
    w.u1((uint8_t)czffvm::ConstantTag::STRING);
    w.string("Sum");

    // 1 "I;I;"
    w.u1((uint8_t)czffvm::ConstantTag::STRING);
    w.string("I;I;");

    // 2 "I;"
    w.u1((uint8_t)czffvm::ConstantTag::STRING);
    w.string("I;");

    // 3 "Main"
    w.u1((uint8_t)czffvm::ConstantTag::STRING);
    w.string("Main");

    // 4 ""
    w.u1((uint8_t)czffvm::ConstantTag::STRING);
    w.string("");

    // 5 "void;"
    w.u1((uint8_t)czffvm::ConstantTag::STRING);
    w.string("void;");

    // 6 int 1
    w.u1((uint8_t)czffvm::ConstantTag::I4);
    w.u4(1);

    // 7 int 2
    w.u1((uint8_t)czffvm::ConstantTag::I4);
    w.u4(2);

    // ---------- Functions ----------
    w.u2(2);

    auto op = [&](auto c){ w.u2((uint16_t)c); };
    auto op2=[&](auto c,uint16_t a){
        w.u2((uint16_t)c);
        w.u1(a>>8); w.u1(a&0xFF);
    };

    /* ---------- Sum ---------- */
    w.u2(0); // name "Sum"
    w.u2(1); // params "I;I;"
    w.u2(2); // return "I;"

    w.u2(2); // max stack
    w.u2(2); // locals

    w.u2(6); // code length

    op2(czffvm::OperationCode::STORE,0); // a
    op2(czffvm::OperationCode::STORE,1); // b
    op2(czffvm::OperationCode::LDV,0);
    op2(czffvm::OperationCode::LDV,1);
    op (czffvm::OperationCode::ADD);
    op (czffvm::OperationCode::RET);

    /* ---------- Main ---------- */
    w.u2(3); // "Main"
    w.u2(4); // ""
    w.u2(5); // "void;"

    w.u2(3); // max stack
    w.u2(3); // locals

    w.u2(11);

    // x = 1
    op2(czffvm::OperationCode::LDC,6);
    op2(czffvm::OperationCode::STORE,0);

    // y = 2
    op2(czffvm::OperationCode::LDC,7);
    op2(czffvm::OperationCode::STORE,1);

    // z = Sum(x,y)
    op2(czffvm::OperationCode::LDV,0);
    op2(czffvm::OperationCode::LDV,1);
    op2(czffvm::OperationCode::CALL,0); // Sum index = 0
    op2(czffvm::OperationCode::STORE,2);

    // print z
    op2(czffvm::OperationCode::LDV,2);
    op (czffvm::OperationCode::PRINT);

    op (czffvm::OperationCode::RET);

    // ---------- Classes ----------
    w.u2(0);

    return w.b;
}

