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

bool IsTerminator(const Operation& op) {
    return IsJump(op) || IsReturn(op);
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
