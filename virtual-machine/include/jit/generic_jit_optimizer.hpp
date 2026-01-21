#pragma once

#include <iostream>
#include <vector>
#include <algorithm>
#include <unordered_map>
#include <unordered_set>
#include <memory>
#include <cstdint>
#include "common.hpp"
#include "runtime_data_area/method_area.hpp"
#include "jit/jit_compiler.hpp"


namespace czffvm_jit {

using namespace czffvm;

bool IsUnconditionalJump(const Operation& op) {
    return op.code == OperationCode::JMP;
}

bool IsConditionalJump(const Operation& op) {
    return op.code == OperationCode::JZ || op.code == OperationCode::JNZ;
}

bool IsJump(const Operation& op) {
    return IsConditionalJump(op) || IsUnconditionalJump(op);
}

bool IsReturn(const Operation& op) {
    return op.code == OperationCode::RET;
}

bool IsStore(OperationCode op) {
    return op == OperationCode::STORE;
}

bool IsLoad(OperationCode op) {
    return op == OperationCode::LDV;
}

bool IsTerminator(const Operation& op) {
    return IsJump(op) || IsReturn(op);
}

int StackProduces(OperationCode op) {
    switch (op) {
        case OperationCode::LDC:
        case OperationCode::LDV:
        case OperationCode::NEWARR:
        case OperationCode::LDELEM:
            return 1;

        case OperationCode::ADD:
        case OperationCode::SUB:
        case OperationCode::MUL:
        case OperationCode::DIV:
        case OperationCode::MOD:
        case OperationCode::EQ:
        case OperationCode::LT:
        case OperationCode::LEQ:
        case OperationCode::LOR:
        case OperationCode::LAND:
            return 1;

        case OperationCode::DUP:
            return 2;

        default:
            return 0;
    }
}

int StackConsumes(OperationCode op) {
    switch (op) {
        case OperationCode::STELEM:
            return 3;

        case OperationCode::ADD:
        case OperationCode::SUB:
        case OperationCode::MUL:
        case OperationCode::DIV:
        case OperationCode::MOD:
        case OperationCode::EQ:
        case OperationCode::LT:
        case OperationCode::LEQ:
        case OperationCode::LAND:
        case OperationCode::LOR:
        case OperationCode::LDELEM:
            return 2;

        case OperationCode::NEG:
        case OperationCode::STORE:
        case OperationCode::PRINT:
        case OperationCode::RET:
        case OperationCode::JZ:
        case OperationCode::JNZ:
        case OperationCode::NEWARR:
        case OperationCode::DUP:
            return 1;

        default:
            return 0;
    }
}


struct BasicBlock {
    int id;
    int start;   // inclusive
    int end;     // exclusive
    std::vector<Operation> instructions;
    std::vector<int> preds;
    std::vector<int> succs;
    bool reachable = false;
    
    bool containsAddress(int addr) const {
        return addr >= start && addr < end;
    }
};

struct StackSlot {
    size_t producer;
};

bool operator==(const czffvm_jit::StackSlot& lhs, const czffvm_jit::StackSlot& rhs) {
    return lhs.producer == rhs.producer;
}

using StackState = std::vector<StackSlot>;


class GenericJitOptimizer {
private:
    MethodArea& method_area_;
    std::vector<Operation>& code_;
    std::vector<BasicBlock> basic_blocks_;
    std::unordered_map<int, int> addr_to_block_;

    int decodeJumpTarget(const Operation& instr) const {
        int hi = instr.arguments[0];
        int lo = instr.arguments[1];
        return (hi << 8) | lo;
    }

    void encodeJumpTarget(Operation& instr, int target_addr) {
        instr.arguments[0] = (target_addr >> 8) & 0xFF;
        instr.arguments[1] = target_addr & 0xFF;
    }


public:
    GenericJitOptimizer(
        std::vector<Operation>& code,
        MethodArea& method_area
    ) : code_(code), method_area_(method_area) {}

    void BuildControlFlowGraph();
    void MarkReachableBlocks();
    void RemoveDeadCode();
    void CompactCode();
    void ConstantFolding();
};

}  // namespace czffvm_jit
