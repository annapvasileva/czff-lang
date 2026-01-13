#pragma once
#include <cstdint>
#include <iostream>
#include <stdexcept>

namespace stdint128 {

struct uint128_t {
    uint64_t hi, lo;

    constexpr uint128_t(uint64_t v = 0) : hi(0), lo(v) {}
    constexpr uint128_t(uint64_t hi_, uint64_t lo_) : hi(hi_), lo(lo_) {}

    friend constexpr bool operator==(uint128_t a, uint128_t b) {
        return a.hi == b.hi && a.lo == b.lo;
    }
    friend constexpr bool operator!=(uint128_t a, uint128_t b) { return !(a==b); }
    friend constexpr bool operator<(uint128_t a, uint128_t b) {
        return (a.hi < b.hi) || (a.hi == b.hi && a.lo < b.lo);
    }
    friend constexpr bool operator>(uint128_t a, uint128_t b) { return b<a; }
    friend constexpr bool operator<=(uint128_t a, uint128_t b){return !(b<a);}
    friend constexpr bool operator>=(uint128_t a, uint128_t b){return !(a<b);}

    friend constexpr uint128_t operator+(uint128_t a, uint128_t b){
        uint64_t lo = a.lo + b.lo;
        return {a.hi + b.hi + (lo < a.lo), lo};
    }

    friend constexpr uint128_t operator-(uint128_t a, uint128_t b){
        uint64_t lo = a.lo - b.lo;
        return {a.hi - b.hi - (a.lo < b.lo), lo};
    }

    friend uint128_t operator*(uint128_t a, uint128_t b){
        uint64_t a_lo = (uint32_t)a.lo;
        uint64_t a_hi = a.lo >> 32;
        uint64_t b_lo = (uint32_t)b.lo;
        uint64_t b_hi = b.lo >> 32;

        uint64_t lo_lo = a_lo * b_lo;
        uint64_t lo_hi = a_lo * b_hi;
        uint64_t hi_lo = a_hi * b_lo;
        uint64_t hi_hi = a_hi * b_hi;

        uint64_t cross = (lo_hi & 0xFFFFFFFF) + (hi_lo & 0xFFFFFFFF) + (lo_lo >> 32);
        uint64_t lo = (cross << 32) | (lo_lo & 0xFFFFFFFF);
        uint64_t hi = a.hi * b.lo + a.lo * b.hi + hi_hi + (lo_hi >> 32) + (hi_lo >> 32) + (cross >> 32);

        return {hi, lo};
    }

    friend uint128_t operator/(uint128_t n, uint128_t d){
        if(d==0) throw std::runtime_error("div0");

        uint128_t q, r;
        for(int i=127;i>=0;i--){
            r = r<<1;
            if(n.bit(i)) r.lo |= 1;
            if(r>=d){
                r = r-d;
                q.setbit(i);
            }
        }
        return q;
    }

    friend uint128_t operator%(uint128_t a, uint128_t b){
        return a - (a/b)*b;
    }

    /* bit ops */
    friend constexpr uint128_t operator&(uint128_t a,uint128_t b){
        return {a.hi&b.hi, a.lo&b.lo};
    }
    friend constexpr uint128_t operator|(uint128_t a,uint128_t b){
        return {a.hi|b.hi, a.lo|b.lo};
    }
    friend constexpr uint128_t operator^(uint128_t a,uint128_t b){
        return {a.hi^b.hi, a.lo^b.lo};
    }

    friend constexpr uint128_t operator<<(uint128_t v,int s){
        if(s>=128) return 0;
        if(s>=64) return {v.lo<<(s-64),0};
        return { (v.hi<<s)|(v.lo>>(64-s)), v.lo<<s };
    }

    friend constexpr uint128_t operator>>(uint128_t v,int s){
        if(s>=128) return 0;
        if(s>=64) return {0, v.hi>>(s-64)};
        return { v.hi>>s, (v.lo>>s)|(v.hi<<(64-s)) };
    }

    friend constexpr uint128_t operator~(uint128_t v){
        return {~v.hi, ~v.lo};
    }

    /* helpers */
    constexpr bool bit(int i) const {
        return i<64 ? (lo>>i)&1 : (hi>>(i-64))&1;
    }
    constexpr void setbit(int i){
        if(i<64) lo |= (1ull<<i);
        else hi |= (1ull<<(i-64));
    }
};

} // namespace
