#include <iostream>
#include <optional>

#include "interpreter.hpp"
#include "call_frame.hpp"
#include "runtime_data_area.hpp"
#include "common.hpp"

namespace czffvm {

Interpreter::Interpreter(RuntimeDataArea& rda)
    : rda_(rda) {}

struct TypeDesc {
    enum Kind {
        INT,
        BOOL,
        ARRAY,
        VOID
    } kind;

    std::unique_ptr<TypeDesc> element = nullptr;
    std::string class_name = "";
};

static TypeDesc ParseType(const std::string& s, size_t& i) {

    if (s[i] == 'I') {
        i+=2;
        return {TypeDesc::INT, nullptr, ""};
    }

    if (s[i] == 'B') {
        i+=2;
        return {TypeDesc::BOOL, nullptr, ""};
    }

    if (s[i] == '[') {
        i++;
        TypeDesc inner = ParseType(s,i);
        return {
            TypeDesc::ARRAY,
            std::make_unique<TypeDesc>(std::move(inner)),
            ""
        };
    }

    if (s.compare(i,5,"void;")==0) {
        i+=5;
        return {TypeDesc::VOID,nullptr,""};
    }

    throw std::runtime_error("Bad type descriptor");
}

static bool Match(const TypeDesc& t, const Value& v) {
    return std::visit([&](auto&& x)->bool{
        using T = std::decay_t<decltype(x)>;

        switch(t.kind) {
            case TypeDesc::INT:
                return std::is_same_v<T,int32_t>;

            case TypeDesc::BOOL:
                return std::is_same_v<T,bool>;

            case TypeDesc::ARRAY:
                return std::is_same_v<T,HeapRef>;

            case TypeDesc::VOID:
                return false;
        }
        return false;
    },v);
}

static bool CheckReturnType(const std::string& expected, const Value& v) {
    size_t i=0;
    TypeDesc t = ParseType(expected,i);

    if (t.kind == TypeDesc::VOID)
        return false;

    return Match(t,v);
}

static size_t CountParams(const std::string& s) {
    size_t i = 0;
    size_t count = 0;

    while (i < s.size()) {
        if (s[i] == '[') {
            while (s[i] == '[') i++;

            if (s[i] == 'I' || s[i] == 'B') {
                i++;
                if (s[i] != ';')
                    throw std::runtime_error("Bad descriptor");
                i++;
            }
            else {
                throw std::runtime_error("Unknown type");
            }

            count++;
            continue;
        }

        if (s[i] == 'I' || s[i] == 'B') {
            i++;
            if (s[i] != ';')
                throw std::runtime_error("Bad descriptor");
            i++;
            count++;
            continue;
        }

        throw std::runtime_error("Bad descriptor");
    }

    return count;
}

void Interpreter::Execute(RuntimeFunction* entry) {
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

                if (ret_type != "void;") {
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
                std::string elem_type(type_c.data.begin(), type_c.data.end()); // пример: I; или [I;
                std::string array_type = "[" + elem_type;

                auto make_default = [&]() -> Value {
                    if (elem_type == "U1;")  return uint8_t(0);
                    if (elem_type == "U2;")  return uint16_t(0);
                    if (elem_type == "U4;")  return uint32_t(0);
                    if (elem_type == "I;")  return int32_t(0);
                    if (elem_type == "U8;")  return uint64_t(0);
                    if (elem_type == "I8;")  return int64_t(0);
                    if (elem_type == "U16;") return stdint128::uint128_t(0);
                    if (elem_type == "I16;") return stdint128::int128_t(0);
                    if (elem_type == "B;") return false;
                    throw std::runtime_error("NEWARR: unknown element type");
                };

                Value def = make_default();

                std::vector<Value> elements(arr_size, def);

                HeapRef ref = rda_.GetHeap().Allocate(array_type, std::move(elements));

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
            case OperationCode::CALL: {
                uint16_t fn_idx = (op.arguments[0] << 8) | op.arguments[1];

                const RuntimeFunction* callee =
                    rda_.GetMethodArea().GetFunction(fn_idx);

                CallFrame& caller =
                    rda_.GetStack().CurrentFrame();

                const Constant& params_c =
                    rda_.GetMethodArea().GetConstant(
                        callee->params_descriptor_index
                    );

                std::string sig(
                    params_c.data.begin(),
                    params_c.data.end()
                );

                size_t argc = CountParams(sig);

                if (caller.operand_stack.size() < argc)
                    throw std::runtime_error("CALL: not enough arguments");

                std::vector<Value> args(argc);
                for (size_t i = 0; i < argc; ++i) {
                    args[argc - 1 - i] = caller.operand_stack.back();
                    caller.operand_stack.pop_back();
                }

                rda_.GetStack().PushFrame(const_cast<RuntimeFunction*>(callee));
                CallFrame& callee_frame =
                    rda_.GetStack().CurrentFrame();

                for (auto& v : args) {
                    callee_frame.operand_stack.push_back(v);
                }

                break;
            }
            case OperationCode::EQ: {
                CallFrame& f = rda_.GetStack().CurrentFrame();

                if (f.operand_stack.empty())
                    throw std::runtime_error("EQ: Operand stack underflow");

                Value b = f.operand_stack.back();
                f.operand_stack.pop_back();

                if (f.operand_stack.empty())
                    throw std::runtime_error("EQ: Operand stack underflow");

                Value a = f.operand_stack.back();
                f.operand_stack.pop_back();

                auto result = std::visit(
                    [](auto&& x, auto&& y) -> Value {
                        using X = std::decay_t<decltype(x)>;
                        using Y = std::decay_t<decltype(y)>;

                        if constexpr (std::is_same_v<X, Y>) {
                            return x == y;
                        } else {
                            throw std::runtime_error("EQ: incompatible types");
                        }
                    },
                    a, b
                );

                f.operand_stack.push_back(result);
                break;
            }
            case OperationCode::LT: {
                CallFrame& f = rda_.GetStack().CurrentFrame();

                if (f.operand_stack.empty())
                    throw std::runtime_error("EQ: Operand stack underflow");

                Value b = f.operand_stack.back();
                f.operand_stack.pop_back();

                if (f.operand_stack.empty())
                    throw std::runtime_error("EQ: Operand stack underflow");

                Value a = f.operand_stack.back();
                f.operand_stack.pop_back();

                auto result = std::visit(
                    [](auto&& x, auto&& y) -> Value {
                        using X = std::decay_t<decltype(x)>;
                        using Y = std::decay_t<decltype(y)>;

                        if constexpr (std::is_same_v<X, Y>
                                    && std::is_integral_v<X>) {
                            return x < y;
                        } else {
                            throw std::runtime_error("EQ: incompatible types");
                        }
                    },
                    a, b
                );

                f.operand_stack.push_back(result);
                break;
            }
            case OperationCode::LEQ: {
                CallFrame& f = rda_.GetStack().CurrentFrame();

                if (f.operand_stack.empty())
                    throw std::runtime_error("EQ: Operand stack underflow");

                Value b = f.operand_stack.back();
                f.operand_stack.pop_back();

                if (f.operand_stack.empty())
                    throw std::runtime_error("EQ: Operand stack underflow");

                Value a = f.operand_stack.back();
                f.operand_stack.pop_back();

                auto result = std::visit(
                    [](auto&& x, auto&& y) -> Value {
                        using X = std::decay_t<decltype(x)>;
                        using Y = std::decay_t<decltype(y)>;

                        if constexpr (std::is_same_v<X, Y>
                                    && std::is_integral_v<X>) {
                            return x <= y;
                        } else {
                            throw std::runtime_error("EQ: incompatible types");
                        }
                    },
                    a, b
                );

                f.operand_stack.push_back(result);
                break;
            }
            case OperationCode::JMP: {
                CallFrame& f = rda_.GetStack().CurrentFrame();

                uint16_t target =
                    (op.arguments[0] << 8) | op.arguments[1];

                if (target >= f.function->code.size()) {
                    throw std::runtime_error("JMP: target out of bounds");
                }

                f.pc = target;
                break;
            }
            case OperationCode::JZ: {
                CallFrame& f = rda_.GetStack().CurrentFrame();

                if (f.operand_stack.empty()) {
                    throw std::runtime_error("JZ: Operand stack underflow");
                }

                Value v = f.operand_stack.back();
                f.operand_stack.pop_back();

                bool cond = std::visit([](auto&& x) -> bool {
                    using T = std::decay_t<decltype(x)>;

                    if constexpr (std::is_integral_v<T>)
                        return x == 0;
                    else if constexpr (std::is_same_v<T,bool>)
                        return x == false;
                    else {
                        throw std::runtime_error("JZ: invalid type");
                    }
                }, v);

                if (cond) {
                    uint16_t target =
                        (op.arguments[0] << 8) | op.arguments[1];

                    if (target >= f.function->code.size())
                        throw std::runtime_error("JZ: target out of bounds");

                    f.pc = target;
                }

                break;
            }
            case OperationCode::JNZ: {
                CallFrame& f = rda_.GetStack().CurrentFrame();

                if (f.operand_stack.empty()) {
                    throw std::runtime_error("JNZ: Operand stack underflow");
                }

                Value v = f.operand_stack.back();
                f.operand_stack.pop_back();

                bool cond = std::visit([](auto&& x) -> bool {
                    using T = std::decay_t<decltype(x)>;

                    if constexpr (std::is_integral_v<T>)
                        return x != 0;
                    else if constexpr (std::is_same_v<T,bool>)
                        return x == true;
                    else {
                        throw std::runtime_error("JNZ: invalid type");
                    }
                }, v);

                if (cond) {
                    uint16_t target =
                        (op.arguments[0] << 8) | op.arguments[1];

                    if (target >= f.function->code.size())
                        throw std::runtime_error("JNZ: target out of bounds");

                    f.pc = target;
                }

                break;
            }
            default:
                throw std::runtime_error("Unknown opcode");
        }
    }
}


}  // namespace czffvm
