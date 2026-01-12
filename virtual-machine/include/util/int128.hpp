#pragma once

#include <cstdint>
#include <iostream>
#include <stdexcept>

#include "uint128.hpp"

namespace stdint128 {

struct int128_t {
    uint128_t u;

    constexpr int128_t(int64_t v=0){
        if(v<0){
            u = uint128_t(0, -v);
            u = ~u + 1;
        }else u = uint128_t(0,v);
    }

    constexpr bool neg() const { return u.hi>>63; }

    friend bool operator==(int128_t a,int128_t b){return a.u==b.u;}
    friend bool operator!=(int128_t a,int128_t b){return !(a==b);}

    friend bool operator<(int128_t a,int128_t b){
        if(a.neg()!=b.neg()) return a.neg();
        return a.u<b.u;
    }

    friend int128_t operator+(int128_t a,int128_t b){
        int128_t r; r.u = a.u + b.u; return r;
    }
    friend int128_t operator-(int128_t a,int128_t b){
        int128_t r; r.u = a.u - b.u; return r;
    }
    friend constexpr int128_t operator-(int128_t v){
        int128_t r;
        r.u = ~v.u + 1;

        return r;
    }
    friend int128_t operator*(int128_t a,int128_t b){
        bool n = a.neg()^b.neg();
        uint128_t x = a.abs().u;
        uint128_t y = b.abs().u;
        uint128_t r = x*y;
        int128_t out; out.u = n? (~r+1):r; return out;
    }

    friend int128_t operator/(int128_t a,int128_t b){
        bool n = a.neg()^b.neg();
        uint128_t r = a.abs().u / b.abs().u;
        int128_t out; out.u = n?(~r+1):r; return out;
    }

    int128_t abs() const {
        if(!neg()) return *this;
        int128_t r; r.u = ~u+1; return r;
    }

    friend constexpr int128_t operator<<(int128_t v, int s){
        int128_t r;
        r.u = v.u << s;
        return r;
    }

    friend constexpr int128_t operator>>(int128_t v, int s){
        int128_t r;

        if(!v.neg()){
            r.u = v.u >> s;
        }else{
            // арифметический сдвиг
            uint128_t tmp = v.u >> s;

            if(s < 128){
                uint64_t mask_hi = (~0ull) << (64 - s);
                tmp.hi |= mask_hi;
            }
            r.u = tmp;
        }
        return r;
    }
};

/* ostream */

inline std::ostream& operator<<(std::ostream& o, stdint128::uint128_t v){
    if(v == 0) return o << '0';

    std::string s;
    while(v != 0){
        auto d = v % 10;
        s.push_back(char('0' + d.lo));
        v = v / 10;
    }

    for(auto it = s.rbegin(); it != s.rend(); ++it)
        o << *it;

    return o;
}

inline std::ostream& operator<<(std::ostream& o, stdint128::int128_t v){
    if(v.neg()){
        o << '-';
        v = v.abs();
    }

    return o << v.u;
}

} // namespace
