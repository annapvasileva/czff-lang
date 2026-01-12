
#include "common.hpp"

namespace czffvm {
Value ConstantToValue(const Constant& c) {
    switch (c.tag) {
        case ConstantTag::U1:
            return uint8_t(c.data[0]);

        case ConstantTag::U2:
            return uint16_t((c.data[0] << 8) | c.data[1]);

        case ConstantTag::U4:
            return uint32_t(
                (c.data[0] << 24) |
                (c.data[1] << 16) |
                (c.data[2] << 8)  |
                 c.data[3]
            );
        
        case ConstantTag::I4:
            return int32_t(
                (c.data[0] << 24) |
                (c.data[1] << 16) |
                (c.data[2] << 8)  |
                 c.data[3]
            );

        case ConstantTag::STRING:
            return std::string(c.data.begin(), c.data.end());

        case ConstantTag::U8: {
            uint64_t v = 0;
            for (int i = 0; i < 8; ++i)
                v = (v << 8) | c.data[i];
            return v;
        }

        case ConstantTag::I8: {
            int64_t v = 0;
            for (int i = 0; i < 8; ++i)
                v = (v << 8) | c.data[i];
            return v;
        }

        case ConstantTag::U16: {
            stdint128::uint128_t v = 0;
            for (int i = 0; i < 16; ++i)
                v = (v << 8) | c.data[i];
            return v;
        }

        case ConstantTag::I16: {
            stdint128::uint128_t tmp = 0;
            for (int i = 0; i < 16; ++i)
                tmp = (tmp << 8) | c.data[i];

            stdint128::int128_t v;
            v.u = tmp;

            return v;
        }

        case ConstantTag::BOOL:
            return bool(c.data[0]);

        default:
            throw std::runtime_error("Unsupported constant tag");
    }
}

bool HeapRef::operator==(const HeapRef& other) const {
    return id == other.id;
}

bool HeapRef::operator!=(const HeapRef& other) const {
    return !(*this == other);
}

std::string dump(const Value& v){
    return std::visit([](auto&& x){
        using T = std::decay_t<decltype(x)>;
        if constexpr (std::is_same_v<T,uint8_t>)   return "U1";
        if constexpr (std::is_same_v<T,uint16_t>)  return "U2";
        if constexpr (std::is_same_v<T,uint32_t>)  return "U4";
        if constexpr (std::is_same_v<T,int32_t>)   return "I4";
        if constexpr (std::is_same_v<T,std::string>) return "STRING";
        if constexpr (std::is_same_v<T,uint64_t>)  return "U8";
        if constexpr (std::is_same_v<T,int64_t>)   return "I8";
        if constexpr (std::is_same_v<T,stdint128::int128_t>)  return "I16";
        if constexpr (std::is_same_v<T,stdint128::uint128_t>) return "U16";
        if constexpr (std::is_same_v<T,bool>)      return "BOOL";
        if constexpr (std::is_same_v<T,HeapRef>)   return "REF";
        return "unknown";
    }, v);
};
}

namespace std {

size_t hash<czffvm::HeapRef>::operator()(const czffvm::HeapRef& r) const noexcept {
    return std::hash<uint32_t>{}(r.id);
}

}
