#include <iostream>
#include <optional>

#include "interpreter.hpp"
#include "call_frame.hpp"
#include "runtime_data_area.hpp"
#include "common.hpp"

namespace czffvm {

Interpreter::Interpreter(RuntimeDataArea& rda)
    : rda_(rda) {}

static bool CheckReturnType(const std::string& expected, const Value& v) {
    return std::visit([&](auto&& x) {
        using T = std::decay_t<decltype(x)>;

        if (expected == "u1")     return std::is_same_v<T, uint8_t>;
        if (expected == "u2")     return std::is_same_v<T, uint16_t>;
        if (expected == "u4")     return std::is_same_v<T, uint32_t>;
        if (expected == "i4")     return std::is_same_v<T, int32_t>;
        if (expected == "u8")     return std::is_same_v<T, uint64_t>;
        if (expected == "i8")     return std::is_same_v<T, int64_t>;
        if (expected == "bool")   return std::is_same_v<T, bool>;
        if (expected == "string") return std::is_same_v<T, std::string>;

        return false;
    }, v);
}

void Interpreter::Execute() {
    RuntimeFunction* entry = rda_.GetMethodArea().GetFunction("Main");
    if (!entry) {
        throw std::runtime_error("Main not found");
    }

    rda_.GetStack().PushFrame(entry);

    while (!rda_.GetStack().Empty()) {
        CallFrame& f = rda_.GetStack().CurrentFrame();

        if (f.pc == f.function->code.size()) {
            throw std::runtime_error("Missing RET instruction");
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
                if (f.operand_stack.empty()) {
                    throw std::runtime_error("STORE: Operand stack underflow");
                }
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
                if (f.operand_stack.empty()) {
                    throw std::runtime_error("ADD: Operand stack underflow");
                }
                Value b = f.operand_stack.back(); f.operand_stack.pop_back();
                if (f.operand_stack.empty()) {
                    throw std::runtime_error("ADD: Operand stack underflow");
                }
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
                if (f.operand_stack.empty()) {
                    throw std::runtime_error("PRINT: Operand stack underflow");
                }
                Value v = f.operand_stack.back();
                f.operand_stack.pop_back();

                std::visit([](auto&& x) {
                    using T = std::decay_t<decltype(x)>;

                    if constexpr (std::is_same_v<T, HeapRef>) {
                        std::cout << "<obj @" << x.id << ">";
                    }
                    else {
                        std::cout << x;
                    }
                }, v);

                break;
            }
            case OperationCode::RET: {
                CallFrame& f = rda_.GetStack().CurrentFrame();

                const Constant& ret_c =
                    rda_.GetMethodArea().GetConstant(f.function->return_type_index);
                std::string ret_type(ret_c.data.begin(), ret_c.data.end());

                std::optional<Value> ret_value;

                if (ret_type != "void") {
                    if (f.operand_stack.empty()) {
                        throw std::runtime_error("RET: missing return value");
                    }

                    ret_value = f.operand_stack.back();
                    f.operand_stack.pop_back();

                    if (!CheckReturnType(ret_type, *ret_value)) {
                        throw std::runtime_error("RET: return type mismatch");
                    }
                }

                rda_.GetStack().PopFrame();

                if (!rda_.GetStack().Empty() && ret_value.has_value()) {
                    rda_.GetStack().CurrentFrame()
                        .operand_stack.push_back(*ret_value);
                }

                break;
            }
            case OperationCode::HALT: {
                uint16_t idx = (op.arguments[0] << 8) | op.arguments[1];
                const Constant& c = rda_.GetMethodArea().GetConstant(idx);
                Value return_value = ConstantToValue(c);
                
                int exit_code = 0;
                switch (c.tag) {
                    case ConstantTag::U1: exit_code = c.data[0]; break;
                    case ConstantTag::U2: exit_code = (c.data[0] << 8) | c.data[1]; break;
                    case ConstantTag::U4: 
                        exit_code = (c.data[0] << 24) | (c.data[1] << 16) | (c.data[2] << 8) | c.data[3];
                        break;
                    case ConstantTag::I4: 
                        exit_code = int32_t((c.data[0] << 24) | (c.data[1] << 16) | (c.data[2] << 8) | c.data[3]);
                        break;
                    default:
                        throw std::runtime_error("HALT: unsupported constant type for exit code");
                }

                std::exit(exit_code);
            }
            case OperationCode::DUP: {
                CallFrame& f = rda_.GetStack().CurrentFrame();
                if (f.operand_stack.empty()) {
                    throw std::runtime_error("DUP: Operand stack underflow");
                }
                f.operand_stack.push_back(f.operand_stack.back());
                break;
            }
            case OperationCode::SWAP: {
                CallFrame& f = rda_.GetStack().CurrentFrame();
                if (f.operand_stack.empty()) {
                    throw std::runtime_error("SWAP: Operand stack underflow");
                }
                auto first = f.operand_stack.back();
                f.operand_stack.pop_back();
                if (f.operand_stack.empty()) {
                    throw std::runtime_error("SWAP: Operand stack underflow");
                }
                auto second = f.operand_stack.back();
                f.operand_stack.pop_back();
                
                f.operand_stack.push_back(first);
                f.operand_stack.push_back(second);
                break;
            }
            case OperationCode::NEWARR: {
                Value v_size = f.operand_stack.back();
                f.operand_stack.pop_back();

                uint32_t arr_size;
                if (auto p = std::get_if<uint8_t>(&v_size))      arr_size = *p;
                else if (auto p = std::get_if<uint16_t>(&v_size)) arr_size = *p;
                else if (auto p = std::get_if<uint32_t>(&v_size)) arr_size = *p;
                else if (auto p = std::get_if<int32_t>(&v_size))  arr_size = static_cast<uint32_t>(*p);
                else {
                    throw std::runtime_error("NEWARR: array size must be integer");
                }

                uint16_t type_idx = (op.arguments[0] << 8) | op.arguments[1];
                const Constant& type_c = rda_.GetMethodArea().GetConstant(type_idx);
                std::string type_str(type_c.data.begin(), type_c.data.end()); // пример: I; или [I;

                std::vector<Value> elements(arr_size);

                HeapRef ref = rda_.GetHeap().Allocate(type_str, std::move(elements));

                f.operand_stack.push_back(ref);
                break;
            }
            case OperationCode::STELEM: {
                Value v_value = f.operand_stack.back();
                f.operand_stack.pop_back();

                Value v_index = f.operand_stack.back();
                f.operand_stack.pop_back();

                uint32_t index;
                if (auto p = std::get_if<uint8_t>(&v_index))       index = *p;
                else if (auto p = std::get_if<uint16_t>(&v_index)) index = *p;
                else if (auto p = std::get_if<uint32_t>(&v_index)) index = *p;
                else if (auto p = std::get_if<int32_t>(&v_index))  index = *p;
                else {
                    throw std::runtime_error("STELEM: index must be integer");
                }

                Value v_arr = f.operand_stack.back();
                f.operand_stack.pop_back();

                auto* ref = std::get_if<HeapRef>(&v_arr);
                if (!ref) {
                    throw std::runtime_error("STELEM: not array reference");
                }

                HeapObject& obj = rda_.GetHeap().Get(*ref);

                if (obj.type.empty() || obj.type[0] != '[') {
                    throw std::runtime_error("STELEM: object is not array");
                }

                if (index >= obj.fields.size()) {
                    throw std::runtime_error("STELEM: index out of bounds");
                }

                obj.fields[index] = v_value;

                break;
            }
            case OperationCode::LDELEM: {
                Value v_index = f.operand_stack.back();
                f.operand_stack.pop_back();

                uint32_t index;
                if (auto p = std::get_if<uint8_t>(&v_index)) index = *p;
                else if (auto p = std::get_if<uint16_t>(&v_index)) index = *p;
                else if (auto p = std::get_if<uint32_t>(&v_index)) index = *p;
                else if (auto p = std::get_if<int32_t>(&v_index)) index = *p;
                else {
                    throw std::runtime_error("LDELEM: index must be integer");
                }

                Value v_arr = f.operand_stack.back();
                f.operand_stack.pop_back();

                auto* ref = std::get_if<HeapRef>(&v_arr);
                if (!ref) {
                    throw std::runtime_error("LDELEM: not an array reference");
                }

                HeapObject& obj = rda_.GetHeap().Get(*ref);

                if (obj.type.empty() || obj.type[0] != '[') {
                    throw std::runtime_error("LDELEM: object is not array");
                }

                if (index >= obj.fields.size()) {
                    throw std::runtime_error("LDELEM: index out of bounds");
                }

                f.operand_stack.push_back(obj.fields[index]);
                break;
            }
            case OperationCode::MUL:  {
                if (f.operand_stack.empty()) {
                    throw std::runtime_error("MUL: Operand stack underflow");
                }
                Value b = f.operand_stack.back(); f.operand_stack.pop_back();
                if (f.operand_stack.empty()) {
                    throw std::runtime_error("MUL: Operand stack underflow");
                }
                Value a = f.operand_stack.back(); f.operand_stack.pop_back();

                auto result = std::visit(
                    [](auto x, auto y) -> Value {
                        using X = std::decay_t<decltype(x)>;
                        using Y = std::decay_t<decltype(y)>;

                        if constexpr (std::is_same_v<X, Y> &&
                                    (std::is_integral_v<X>)) {
                            return x * y;
                        } else {
                            throw std::runtime_error("MUL: incompatible types");
                        }
                    },
                    a, b
                );

                f.operand_stack.push_back(result);
                break;
            }
            case OperationCode::MIN: {
                if (f.operand_stack.empty()) {
                    throw std::runtime_error("MIN: Operand stack underflow");
                }
                Value a = f.operand_stack.back(); f.operand_stack.pop_back();

                auto result = std::visit(
                    [](auto x) -> Value {
                        using X = std::decay_t<decltype(x)>;

                        if constexpr (std::is_integral_v<X>) {
                            return -x;
                        } else {
                            throw std::runtime_error("MIN: incompatible types");
                        }
                    },
                    a
                );

                f.operand_stack.push_back(result);
                break;
            }
            case OperationCode::SUB: {
                if (f.operand_stack.empty()) {
                    throw std::runtime_error("SUB: Operand stack underflow");
                }
                Value b = f.operand_stack.back(); f.operand_stack.pop_back();
                if (f.operand_stack.empty()) {
                    throw std::runtime_error("SUB: Operand stack underflow");
                }
                Value a = f.operand_stack.back(); f.operand_stack.pop_back();

                auto result = std::visit(
                    [](auto x, auto y) -> Value {
                        using X = std::decay_t<decltype(x)>;
                        using Y = std::decay_t<decltype(y)>;

                        if constexpr (std::is_same_v<X, Y> &&
                                    (std::is_integral_v<X>)) {
                            return x - y;
                        } else {
                            throw std::runtime_error("SUB: incompatible types");
                        }
                    },
                    a, b
                );

                f.operand_stack.push_back(result);
                break;
            }
            case OperationCode::DIV:  {
                if (f.operand_stack.empty()) {
                    throw std::runtime_error("DIV: Operand stack underflow");
                }
                Value b = f.operand_stack.back(); f.operand_stack.pop_back();
                if (f.operand_stack.empty()) {
                    throw std::runtime_error("DIV: Operand stack underflow");
                }
                Value a = f.operand_stack.back(); f.operand_stack.pop_back();

                auto result = std::visit(
                    [](auto x, auto y) -> Value {
                        using X = std::decay_t<decltype(x)>;
                        using Y = std::decay_t<decltype(y)>;

                        if constexpr (std::is_same_v<X, Y> &&
                                    (std::is_integral_v<X>)) {
                            return x / y;
                        } else {
                            throw std::runtime_error("DIV: incompatible types");
                        }
                    },
                    a, b
                );

                f.operand_stack.push_back(result);
                break;
            }
            case OperationCode::CALL:
                break;
            default:
                throw std::runtime_error("Unknown opcode");
        }
    }
}


}  // namespace czffvm
