#include <type_traits>
#include <iostream>
#include <optional>

#include "interpreter.hpp"
#include "call_frame.hpp"
#include "runtime_data_area.hpp"
#include "common.hpp"

namespace czffvm {

Interpreter::Interpreter(RuntimeDataArea& rda)
    : rda_(rda) {
    heapHelper_ = std::make_unique<czffvm_jit::X86JitHeapHelper>(rda_);
}

struct TypeDesc {
    enum Kind { INT, BOOL, ARRAY, STRING, VOID } kind;
    bool is_signed = true;
    int size_bytes = 0;
    std::unique_ptr<TypeDesc> element;
};

static TypeDesc ParseType(const std::string& s, size_t& i) {

    if (s.compare(i,7,"String;")==0) {
        i+=7;
        return {TypeDesc::STRING};
    }

    if (s[i]=='I' || s[i]=='U') {
        bool sign = s[i]=='I';
        i++;

        size_t start=i;
        while (isdigit(s[i])) i++;

        int bytes = start==i ? 4 : std::stoi(s.substr(start,i-start));

        if (s[i++]!=';')
            throw std::runtime_error("Bad descriptor");

        return {TypeDesc::INT, sign, bytes};
    }

    if (s[i]=='B') {
        i+=2;
        return {TypeDesc::BOOL};
    }

    if (s[i]=='[') {
        i++;
        auto inner = ParseType(s,i);
        return {TypeDesc::ARRAY,false,0,
                std::make_unique<TypeDesc>(std::move(inner))};
    }

    if (s.compare(i,5,"void;")==0) {
        i+=5;
        return {TypeDesc::VOID};
    }

    throw std::runtime_error("Bad descriptor");
}

static bool Match(const TypeDesc& t,const Value& v){
    return std::visit([&](auto&& x){
        using T = std::decay_t<decltype(x)>;

        if(t.kind==TypeDesc::BOOL)
            return std::is_same_v<T,bool>;

        if(t.kind==TypeDesc::ARRAY)
            return std::is_same_v<T,HeapRef>;

        if(t.kind==TypeDesc::STRING)
            return std::is_same_v<T,StringRef>;

        if(t.kind==TypeDesc::INT){
            if(t.is_signed){
                if(t.size_bytes==1) return std::is_same_v<T,int8_t>;
                if(t.size_bytes==2) return std::is_same_v<T,int16_t>;
                if(t.size_bytes==4) return std::is_same_v<T,int32_t>;
                if(t.size_bytes==8) return std::is_same_v<T,int64_t>;
                if(t.size_bytes==16) return std::is_same_v<T,stdint128::int128_t>;
            } else {
                if(t.size_bytes==1) return std::is_same_v<T,uint8_t>;
                if(t.size_bytes==2) return std::is_same_v<T,uint16_t>;
                if(t.size_bytes==4) return std::is_same_v<T,uint32_t>;
                if(t.size_bytes==8) return std::is_same_v<T,uint64_t>;
                if(t.size_bytes==16) return std::is_same_v<T,stdint128::uint128_t>;
            }
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

size_t CountParams(const std::string& s){
    size_t i=0,c=0;
    while(i<s.size()){
        ParseType(s,i);
        c++;
    }
    return c;
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
                f.locals.at(idx) = std::move(f.operand_stack.back());
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
                Value b = std::move(f.operand_stack.back()); f.operand_stack.pop_back();
                if (f.operand_stack.empty()) {
                    throw std::runtime_error("ADD: Operand stack underflow");
                }
                Value a = std::move(f.operand_stack.back()); f.operand_stack.pop_back();

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
                Value v = std::move(f.operand_stack.back());
                f.operand_stack.pop_back();

                std::visit([](auto&& x) {
                    using T = std::decay_t<decltype(x)>;

                    if constexpr (std::is_same_v<T, HeapRef>) {
                        std::cout << "<obj @" << x.id << ">";
                    } else if constexpr (std::is_same_v<T, StringRef>) {
                        std::cout << *x;
                    } else {
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

                    ret_value = std::move(f.operand_stack.back());
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
                auto first = std::move(f.operand_stack.back());
                f.operand_stack.pop_back();
                if (f.operand_stack.empty()) {
                    throw std::runtime_error("SWAP: Operand stack underflow");
                }
                auto second = std::move(f.operand_stack.back());
                f.operand_stack.pop_back();
                
                f.operand_stack.push_back(first);
                f.operand_stack.push_back(second);
                break;
            }
            case OperationCode::NEWARR: {
                Value v_size = std::move(f.operand_stack.back());
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
                    if (elem_type == "U1;") return uint8_t(0);
                    if (elem_type == "I1;") return int8_t(0);
                    if (elem_type == "U2;") return uint16_t(0);
                    if (elem_type == "I2;") return int16_t(0);
                    if (elem_type == "U;") return uint32_t(0);
                    if (elem_type == "I;") return int32_t(0);
                    if (elem_type == "U8;") return uint64_t(0);
                    if (elem_type == "I8;") return int64_t(0);
                    if (elem_type == "U16;") return stdint128::uint128_t(0);
                    if (elem_type == "I16;") return stdint128::int128_t(0);
                    if (elem_type == "B;") return false;
                    if (elem_type == "String;") return std::make_shared<std::string>(std::string());
                    throw std::runtime_error("NEWARR: unknown element type"); };

                Value def = make_default();

                std::vector<Value> elements(arr_size, def);

                HeapRef ref = rda_.GetHeap().Allocate(array_type, std::move(elements));

                f.operand_stack.push_back(ref);
                break;
            }
            case OperationCode::STELEM: {
                Value v_value = std::move(f.operand_stack.back());
                f.operand_stack.pop_back();

                Value v_index = std::move(f.operand_stack.back());
                f.operand_stack.pop_back();

                uint32_t index;
                if (auto p = std::get_if<uint8_t>(&v_index))       index = *p;
                else if (auto p = std::get_if<uint16_t>(&v_index)) index = *p;
                else if (auto p = std::get_if<uint32_t>(&v_index)) index = *p;
                else if (auto p = std::get_if<int32_t>(&v_index))  index = *p;
                else {
                    throw std::runtime_error("STELEM: index must be integer");
                }

                Value v_arr = std::move(f.operand_stack.back());
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
                Value v_index = std::move(f.operand_stack.back());
                f.operand_stack.pop_back();

                uint32_t index;
                if (auto p = std::get_if<uint8_t>(&v_index)) index = *p;
                else if (auto p = std::get_if<uint16_t>(&v_index)) index = *p;
                else if (auto p = std::get_if<uint32_t>(&v_index)) index = *p;
                else if (auto p = std::get_if<int32_t>(&v_index)) index = *p;
                else {
                    throw std::runtime_error("LDELEM: index must be integer");
                }

                Value v_arr = std::move(f.operand_stack.back());
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
                Value b = std::move(f.operand_stack.back()); f.operand_stack.pop_back();
                if (f.operand_stack.empty()) {
                    throw std::runtime_error("MUL: Operand stack underflow");
                }
                Value a = std::move(f.operand_stack.back()); f.operand_stack.pop_back();

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
                Value a = std::move(f.operand_stack.back()); f.operand_stack.pop_back();

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
                Value b = std::move(f.operand_stack.back()); f.operand_stack.pop_back();
                if (f.operand_stack.empty()) {
                    throw std::runtime_error("SUB: Operand stack underflow");
                }
                Value a = std::move(f.operand_stack.back()); f.operand_stack.pop_back();

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
                Value b = std::move(f.operand_stack.back()); f.operand_stack.pop_back();
                if (f.operand_stack.empty()) {
                    throw std::runtime_error("DIV: Operand stack underflow");
                }
                Value a = std::move(f.operand_stack.back()); f.operand_stack.pop_back();

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

                RuntimeFunction* callee =
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
                    args[i] = caller.operand_stack.back();
                    caller.operand_stack.pop_back();
                }

                if (!callee->jit_function && callee->call_count >= kJitThreshold && callee->compilable) {
                    if (!CanCompile(callee)) {
                        callee->compilable = false;
                    } else {
                        JitCompile(callee);
                    } 
                }

                if (callee->jit_function && callee->compilable) {
                    try {
                        ExecuteJitFunction(callee, caller, args);
                        break;
                        
                    } catch (const std::exception& e) {
                        std::cerr << "Error: " << e.what() << std::endl;
                        callee->compilable = false;
                    }
                }

                rda_.GetStack().PushFrame(const_cast<RuntimeFunction*>(callee));
                CallFrame& callee_frame =
                    rda_.GetStack().CurrentFrame();

                callee->call_count++;

                for (auto& v : args) {
                    callee_frame.operand_stack.push_back(v);
                }

                break;
            }
            case OperationCode::EQ: {
                CallFrame& f = rda_.GetStack().CurrentFrame();

                if (f.operand_stack.empty())
                    throw std::runtime_error("EQ: Operand stack underflow");

                Value b = std::move(f.operand_stack.back());
                f.operand_stack.pop_back();

                if (f.operand_stack.empty())
                    throw std::runtime_error("EQ: Operand stack underflow");

                Value a = std::move(f.operand_stack.back());
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
                    throw std::runtime_error("LT: Operand stack underflow");

                Value b = std::move(f.operand_stack.back());
                f.operand_stack.pop_back();

                if (f.operand_stack.empty())
                    throw std::runtime_error("LT: Operand stack underflow");

                Value a = std::move(f.operand_stack.back());
                f.operand_stack.pop_back();

                auto result = std::visit(
                    [](auto&& x, auto&& y) -> Value {
                        using X = std::decay_t<decltype(x)>;
                        using Y = std::decay_t<decltype(y)>;

                        if constexpr (std::is_same_v<X, Y>
                                    && std::is_integral_v<X>) {
                            return x < y;
                        } else {
                            throw std::runtime_error("LT: incompatible types");
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
                    throw std::runtime_error("LEQ: Operand stack underflow");

                Value b = std::move(f.operand_stack.back());
                f.operand_stack.pop_back();

                if (f.operand_stack.empty())
                    throw std::runtime_error("LEQ: Operand stack underflow");

                Value a = std::move(f.operand_stack.back());
                f.operand_stack.pop_back();

                auto result = std::visit(
                    [](auto&& x, auto&& y) -> Value {
                        using X = std::decay_t<decltype(x)>;
                        using Y = std::decay_t<decltype(y)>;

                        if constexpr (std::is_same_v<X, Y>
                                    && std::is_integral_v<X>) {
                            return x <= y;
                        } else {
                            throw std::runtime_error("LEQ: incompatible types");
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

                Value v = std::move(f.operand_stack.back());
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

                Value v = std::move(f.operand_stack.back());
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
            case OperationCode::NEG: {
                if (f.operand_stack.empty()) {
                    throw std::runtime_error("NEG: Operand stack underflow");
                }
                Value a = std::move(f.operand_stack.back()); f.operand_stack.pop_back();
                auto result = std::visit(
                    [](auto x) -> Value {
                        using X = std::decay_t<decltype(x)>;

                        if constexpr (std::is_same_v<X,bool>) {
                            return !x;
                        } else {
                            throw std::runtime_error("NEG: cannot apply logical negation to non-boolean types");
                        }
                    },
                    a
                );

                f.operand_stack.push_back(result);
                break;
            }
            case OperationCode::MOD: {
                if (f.operand_stack.empty()) {
                    throw std::runtime_error("MOD: Operand stack underflow");
                }
                Value b = std::move(f.operand_stack.back()); f.operand_stack.pop_back();
                if (f.operand_stack.empty()) {
                    throw std::runtime_error("MOD: Operand stack underflow");
                }
                Value a = std::move(f.operand_stack.back()); f.operand_stack.pop_back();

                auto result = std::visit(
                    [](auto x, auto y) -> Value {
                        using X = std::decay_t<decltype(x)>;
                        using Y = std::decay_t<decltype(y)>;

                        if constexpr (std::is_same_v<X, Y> &&
                                    (std::is_integral_v<X>)) {
                            return x % y;
                        } else {
                            throw std::runtime_error("MOD: incompatible types");
                        }
                    },
                    a, b
                );

                f.operand_stack.push_back(result);
                break;
            }
            case OperationCode::LOR: {
                if (f.operand_stack.empty()) {
                    throw std::runtime_error("LOR: Operand stack underflow");
                }
                Value b = std::move(f.operand_stack.back()); f.operand_stack.pop_back();
                if (f.operand_stack.empty()) {
                    throw std::runtime_error("LOR: Operand stack underflow");
                }
                Value a = std::move(f.operand_stack.back()); f.operand_stack.pop_back();
                auto result = std::visit(
                    [](auto x, auto y) -> Value {
                        using X = std::decay_t<decltype(x)>;
                        using Y = std::decay_t<decltype(y)>;

                        if constexpr (std::is_same_v<X, Y> &&
                                    (std::is_same_v<X,bool>)) {
                            return x || y;
                        } else {
                            throw std::runtime_error("LOR: incompatible types");
                        }
                    },
                    a, b
                );

                f.operand_stack.push_back(result);
                break;
            }
            case OperationCode::LAND: {
                if (f.operand_stack.empty()) {
                    throw std::runtime_error("LAND: Operand stack underflow");
                }
                Value b = std::move(f.operand_stack.back()); f.operand_stack.pop_back();
                if (f.operand_stack.empty()) {
                    throw std::runtime_error("LAND: Operand stack underflow");
                }
                Value a = std::move(f.operand_stack.back()); f.operand_stack.pop_back();
                auto result = std::visit(
                    [](auto x, auto y) -> Value {
                        using X = std::decay_t<decltype(x)>;
                        using Y = std::decay_t<decltype(y)>;

                        if constexpr (std::is_same_v<X, Y> &&
                                    (std::is_same_v<X,bool>)) {
                            return x && y;
                        } else {
                            throw std::runtime_error("LAND: incompatible types");
                        }
                    },
                    a, b
                );

                f.operand_stack.push_back(result);
                break;
            }
            default:
                throw std::runtime_error("Unknown opcode");
        }
    }
}

void Interpreter::JitCompile(RuntimeFunction* function) {
    function->jit_function = jit_compiler_->CompileFunction(*function, rda_);
}

void Interpreter::SetJitCompiler(std::unique_ptr<czffvm_jit::JitCompiler> jit) {
    jit_compiler_ = std::move(jit);
}

void Interpreter::ExecuteJitFunction(RuntimeFunction* function, CallFrame& caller_frame, std::vector<Value>& args) {
    
    int32_t stack[function->max_stack * 2];
    for (size_t i = 0; i < args.size(); ++i) {
        int32_t value;
        if (auto p = std::get_if<uint8_t>(&args[i]))      value = static_cast<int32_t>(*p);
        else if (auto p = std::get_if<uint16_t>(&args[i])) value = static_cast<int32_t>(*p);
        else if (auto p = std::get_if<uint32_t>(&args[i])) value = static_cast<int32_t>(*p);
        else if (auto p = std::get_if<uint64_t>(&args[i])) value = static_cast<int32_t>(*p);
        else if (auto p = std::get_if<int8_t>(&args[i]))  value = *p;
        else if (auto p = std::get_if<int16_t>(&args[i]))  value = *p;
        else if (auto p = std::get_if<int32_t>(&args[i]))  value = *p;
        else if (auto p = std::get_if<int64_t>(&args[i]))  value = *p;
        else if (auto p = std::get_if<bool>(&args[i]))  value = static_cast<bool>(*p);
        else if (auto p = std::get_if<HeapRef>(&args[i]))  value = static_cast<int32_t>(p->id);
        else {
            throw std::runtime_error("Wrong type for JIT-compilation, only integers supported");
        }
        stack[i] = value;
    }

    using VMFunc = void(*)(int32_t*, czffvm_jit::X86JitHeapHelper*);
    VMFunc func_ptr = function->jit_function->getFunction<VMFunc>();

    czffvm_jit::X86JitHeapHelper& hh = *heapHelper_;

    auto ret_c = rda_.GetMethodArea().GetConstant(function->return_type_index);

    func_ptr(stack, &hh);
    std::string ret_type(ret_c.data.begin(), ret_c.data.end());

    if (ret_type == "void;") {
        return;
    }
    
    size_t i = 0;
    auto type_kind = ParseType(ret_type, i);

    Constant c;
    switch (type_kind.kind) {
        case TypeDesc::Kind::INT:
        case TypeDesc::Kind::BOOL: {
            switch (type_kind.size_bytes) {
                case 1:
                case 0: {
                    c = {
                        type_kind.kind == TypeDesc::Kind::BOOL ? ConstantTag::BOOL : type_kind.is_signed ? ConstantTag::I1 : ConstantTag::U1, 
                        {stack[0] & 0xFF}
                    };
                    break;
                }
                case 2: {
                    c = {
                        type_kind.is_signed ? ConstantTag::I2 : ConstantTag::U2, 
                        {stack[0] >> 8, stack[0] & 0xFF}
                    };
                    break;
                }
                case 4: {
                    c = {
                        type_kind.is_signed ? ConstantTag::I4 : ConstantTag::U4, 
                        {stack[0] >> 24, (stack[0] >> 16) & 0xFF, (stack[0] >> 8) & 0xFF, stack[0] & 0xFF, }
                    };
                    break;
                }
                default: 
                    throw std::runtime_error("Wrong return type for JIT-compilation, only integers supported");
            }
            break;
        }
        default: 
            throw std::runtime_error("Wrong return type for JIT-compilation, only integers supported");
    }

    caller_frame.operand_stack.push_back(ConstantToValue(c));
}

bool Interpreter::CanCompile(const RuntimeFunction* function) {
    if (!jit_compiler_) {
        return false;
    }
    for (const auto& op : function->code) {
        if (!jit_compiler_->CanCompile(op)) {
            return false;
        }
    }

    return true;
}


}  // namespace czffvm
