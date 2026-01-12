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
        uint64_t a0 = a.lo, a1 = a.hi;
        uint64_t b0 = b.lo, b1 = b.hi;

        __uint128_t p = ( __uint128_t)a0 * b0;
        uint64_t lo = (uint64_t)p;
        uint64_t hi = (uint64_t)(p >> 64);

        hi += a0*b1 + a1*b0;
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
