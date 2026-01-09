
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
}

namespace std {

size_t hash<czffvm::HeapRef>::operator()(const czffvm::HeapRef& r) const noexcept {
    return std::hash<uint32_t>{}(r.id);
}

}
