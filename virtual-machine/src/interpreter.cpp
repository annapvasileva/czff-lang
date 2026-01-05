#include <iostream>

#include "interpreter.hpp"
#include "call_frame.hpp"
#include "runtime_data_area.hpp"
#include "common.hpp"

namespace czffvm {

Interpreter::Interpreter(RuntimeDataArea& rda)
    : rda_(rda) {}

void Interpreter::Execute() {
    RuntimeFunction* entry = rda_.GetMethodArea().GetFunction("Main");
    if (!entry) {
        throw std::runtime_error("Main not found");
    }

    rda_.GetStack().PushFrame(entry);

    while (!rda_.GetStack().Empty()) {
        CallFrame& f = rda_.GetStack().CurrentFrame();

        if (f.pc == f.function->code.size()) {
            rda_.GetStack().PopFrame();

            continue;
        }

        if (f.pc > f.function->code.size()) {
            throw std::runtime_error("PC out of bounds");
        }

        const Operation& op = f.function->code[f.pc++];
        switch (op.code) {
            case OperationCode::LDC: {
                uint16_t idx = (op.arguments[0] << 8) | op.arguments[1];
                const Constant& c = rda_.GetMethodArea().GetConstant(idx);
                f.operand_stack.push_back(ConstantToValue(c));
                break;
            }
            case OperationCode::STORE: {
                uint16_t idx = (op.arguments[0] << 8) | op.arguments[1];
                f.locals.at(idx) = f.operand_stack.back();
                f.operand_stack.pop_back();
                break;
            }
            case OperationCode::LDV: {
                uint16_t idx = (op.arguments[0] << 8) | op.arguments[1];
                f.operand_stack.push_back(f.locals.at(idx));
                break;
            }
            case OperationCode::ADD: {
                Value b = f.operand_stack.back(); f.operand_stack.pop_back();
                Value a = f.operand_stack.back(); f.operand_stack.pop_back();

                auto result = std::visit(
                    [](auto x, auto y) -> Value {
                        using X = std::decay_t<decltype(x)>;
                        using Y = std::decay_t<decltype(y)>;

                        if constexpr (std::is_same_v<X, Y> &&
                                    (std::is_integral_v<X>)) {
                            return x + y;
                        } else {
                            throw std::runtime_error("ADD: incompatible types");
                        }
                    },
                    a, b
                );

                f.operand_stack.push_back(result);
                break;
            }
            case OperationCode::PRINT: {
                Value v = f.operand_stack.back();
                f.operand_stack.pop_back();

                std::visit([](auto&& x) {
                    std::cout << x;
                }, v);
                std::cout << '\n';

                break;
            }
            case OperationCode::HALT:
                while (!rda_.GetStack().Empty()) {
                    rda_.GetStack().PopFrame();
                }
                return;
            case OperationCode::DUP:
                break;
            case OperationCode::SWAP:
                break;
            default:
                throw std::runtime_error("Unknown opcode");
        }
    }
}


}  // namespace czffvm
