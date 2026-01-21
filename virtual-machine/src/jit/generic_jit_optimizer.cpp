#include <optional>

#include "jit/generic_jit_optimizer.hpp"

namespace czffvm_jit {
void GenericJitOptimizer::BuildControlFlowGraph() {
    basic_blocks_.clear();
    addr_to_block_.clear();

    std::unordered_set<int> leaders;
    leaders.insert(0);

    for (int i = 0; i < (int)code_.size(); ++i) {
        const Operation& instr = code_[i];

        if (IsJump(instr)) {
            int target = decodeJumpTarget(instr);
            leaders.insert(target);       // jump target is a leader

            if (IsConditionalJump(instr)) {
                leaders.insert(i + 1);    // fallthrough is a leader
            }
        } else if (IsReturn(instr) && i + 1 < (int)code_.size()) {
            leaders.insert(i + 1);
        }
    }

    std::vector<int> sorted_leaders(leaders.begin(), leaders.end());
    std::sort(sorted_leaders.begin(), sorted_leaders.end());

    for (size_t i = 0; i < sorted_leaders.size(); ++i) {
        int start = sorted_leaders[i];
        int end = (i + 1 < sorted_leaders.size()) ? sorted_leaders[i + 1] : code_.size();

        BasicBlock bb;
        bb.id = basic_blocks_.size();
        bb.start = start;
        bb.end = end;
        basic_blocks_.push_back(bb);

        for (int a = start; a < end; ++a) {
            addr_to_block_[a] = bb.id;
        }
    }

    for (auto& bb : basic_blocks_) {
        int last_addr = bb.end - 1;
        const Operation& last = code_[last_addr];

        if (IsReturn(last)) continue;

        if (IsJump(last)) {
            int target = decodeJumpTarget(last);
            int target_bb = addr_to_block_[target];

            bb.succs.push_back(target_bb);
            basic_blocks_[target_bb].preds.push_back(bb.id);

            if (IsConditionalJump(last)) {
                int fallthrough = bb.end;
                if (addr_to_block_.count(fallthrough)) {
                    int ft_bb = addr_to_block_[fallthrough];
                    bb.succs.push_back(ft_bb);
                    basic_blocks_[ft_bb].preds.push_back(bb.id);
                }
            }
        } else {
            int fallthrough = bb.end;
            if (addr_to_block_.count(fallthrough)) {
                int ft_bb = addr_to_block_[fallthrough];
                bb.succs.push_back(ft_bb);
                basic_blocks_[ft_bb].preds.push_back(bb.id);
            }
        }
    }
}

void GenericJitOptimizer::MarkReachableBlocks() {
    if (basic_blocks_.empty()) return;

    std::vector<int> stack{0};

    while (!stack.empty()) {
        int id = stack.back();
        stack.pop_back();

        auto& bb = basic_blocks_[id];
        if (bb.reachable) continue;

        bb.reachable = true;
        for (int s : bb.succs) {
            stack.push_back(s);
        }
    }
}

void GenericJitOptimizer::RemoveDeadCode() {
    for (const auto& bb : basic_blocks_) {
        if (!bb.reachable) {
            for (int i = bb.start; i < bb.end; ++i) {
                code_[i].code = OperationCode::NOP;
                code_[i].arguments.clear();
            }
            continue;
        }

        bool after_terminator = false;
        for (int i = bb.start; i < bb.end; ++i) {
            if (after_terminator) {
                code_[i].code = OperationCode::NOP;
                code_[i].arguments.clear();
                continue;
            }
            if (IsTerminator(code_[i])) {
                after_terminator = true;
            }
        }
    }
}

void GenericJitOptimizer::CompactCode() {
    std::vector<Operation> new_code;
    std::vector<int> old_to_new(code_.size(), -1);

    for (int i = 0; i < (int)code_.size(); ++i) {
        if (code_[i].code != OperationCode::NOP) {
            old_to_new[i] = new_code.size();
            new_code.push_back(code_[i]);
        }
    }

    auto resolve = [&](int old_addr) {
        while (old_addr < (int)code_.size() && old_to_new[old_addr] == -1) ++old_addr;
        return old_to_new[old_addr];
    };

    for (int i = 0; i < (int)new_code.size(); ++i) {
        Operation& instr = new_code[i];
        if (!IsJump(instr)) continue;

        int old_addr = -1;
        for (int j = 0; j < (int)code_.size(); ++j) {
            if (old_to_new[j] == i) {
                old_addr = j;
                break;
            }
        }

        int old_target = decodeJumpTarget(instr);
        int new_target = resolve(old_target);

        encodeJumpTarget(instr, new_target);
    }

    code_ = std::move(new_code);
}


void GenericJitOptimizer::ConstantFolding() {
    struct StackEntry {
        std::optional<Value> value;
        size_t addr; // индекс инструкции в code_
    };

    std::vector<StackEntry> stack;

    for (size_t i = 0; i < code_.size(); ++i) {
        Operation& instr = code_[i];

        switch (instr.code) {
        case OperationCode::LDC: {
            int idx = (instr.arguments[0] << 8) | instr.arguments[1];
            const Constant& c = method_area_.GetConstant(idx);

            if ((c.tag == ConstantTag::U1 || c.tag == ConstantTag::U2 || c.tag == ConstantTag::U4 ||
                 c.tag == ConstantTag::I1 || c.tag == ConstantTag::I2 || c.tag == ConstantTag::I4)) {

                int64_t val = 0;
                for (auto b : c.data) val = (val << 8) | b;
                stack.push_back({val, i});
            } else {
                stack.push_back({std::nullopt, i});
            }
            break;
        }

        case OperationCode::ADD:
        case OperationCode::SUB:
        case OperationCode::MUL:
        case OperationCode::DIV: {
            if (stack.size() < 2) {
                stack.clear();
                break;
            }

            auto b = stack.back(); stack.pop_back();
            auto a = stack.back(); stack.pop_back();

            if (!a.value.has_value() || !b.value.has_value()) {
                stack.push_back({std::nullopt, i});
                break;
            }

            // Преобразуем Value к int64_t
            auto pa = SafeValueToInteger<int64_t>(a.value.value());
            auto pb = SafeValueToInteger<int64_t>(b.value.value());
            if (!pa.has_value() || !pb.has_value()) {
                stack.push_back({std::nullopt, i});
                break;
            }

            int64_t result = 0;
            switch (instr.code) {
                case OperationCode::ADD: result = pa.value() + pb.value(); break;
                case OperationCode::SUB: result = pa.value() - pb.value(); break;
                case OperationCode::MUL: result = pa.value() * pb.value(); break;
                case OperationCode::DIV: 
                    if (pb.value() == 0) {
                        stack.push_back({std::nullopt, i});
                        continue;
                    }
                    result = pa.value() / pb.value(); 
                    break;
                default: break;
            }

            // Теперь заменяем инструкции по их адресам
            size_t addr1 = a.addr;
            size_t addr2 = b.addr;
            size_t op_addr = i;

            int const_idx = method_area_.RegisterConstant(
                Constant{ConstantTag::I4, {
                    uint8_t((result >> 24) & 0xFF),
                    uint8_t((result >> 16) & 0xFF),
                    uint8_t((result >> 8) & 0xFF),
                    uint8_t(result & 0xFF)
                }});

            code_[addr1].code = OperationCode::LDC;
            code_[addr1].arguments = {uint8_t((const_idx >> 8) & 0xFF), uint8_t(const_idx & 0xFF)};
            
            code_[addr2].code = OperationCode::NOP;
            code_[addr2].arguments.clear();

            code_[op_addr].code = OperationCode::NOP;
            code_[op_addr].arguments.clear();

            stack.push_back({result, addr1});
            break;
        }

        default:
            stack.clear();
            break;
        }
    }

    CompactCode();
}

void GenericJitOptimizer::RemoveRedundantJumps() {
    for (size_t i = 0; i < code_.size(); ++i) {
        Operation& instr = code_[i];

        if (instr.code == OperationCode::JMP) {
            int target = decodeJumpTarget(instr);

            // JMP на следующую инструкцию — бессмысленный
            if (target == static_cast<int>(i + 1)) {
                instr.code = OperationCode::NOP;
                instr.arguments.clear();
            }
        }
    }

    CompactCode();
}

}  // namespace czffvm_jit
