# Bytecode

- [Structure](#structure)
- [Tags](#tag)
- [OpCodes List](#operation-codes-list)

## Structure

### The following objects are serialized into a .ball file in the order listed: 

1. **Header** includes:
    1.  **Magical number** — 62616c6c ("ball" in 0x format) — byte[4], 4 bytes
    2. **Version** of bytecode — byte[3], 3 bytes
    3. **Flags** of `debug`, `has_checksum`, maybe more will be later — byte, 1 byte
    
    **Summary**: $8\ \text{bytes}$.
2. **Constants pool** includes:
    1. **Length** of pool — uint_16, shorted to $n$
    2. **Array of elemnts** in view: (tag, data) — tag, 1 byte; data, $x$ bytes 

>[!IMPORTANT]
>All constants are stored here. In [Types](#types) section will be mentioned types for data, but constants for *Name*, *descriptors*, everything stored in *uint_16* as **index of constant**. For axmaple `Name, string` means that *uint_16*, staying in this position, is the index of constant for *Name* with type *string*. `Max stack used, uint_16` *uint_16* of length of max stack, **it isn't** constant in code. 

3. **Functions pool** includes"
    1. **Functions length**, uint_16
    2. **Functions**    

4. **Classes pool** includes:
    1. **Classes length**, uint_16
    2. **Classes**, class[x]

## Types

### Function
Function is a sequence of commands that is executed when called using parameters. The function returns a value.
1. **Name**, string
2. **Parameters**, string descriptor
3. **Return type link**, string descriptor
4. **Max stack used**, uint_16 *(Optimization, not for MVP)*
5. **Locals length**, uint_16 *(Optimization, not for MVP)*
6. **Code length**, uint_16
7. **Code**, op_code[]

### Class
1. **Name**, string
2. **Fields length**, uint_16 
3. **Fields**, field[]
4. **Methods length**, uint_16
5. **Methods**, method[]

### Field
Field is a variable inside a class, all fields are public

1. **Name**, string
2. **Type link**, string descriptor

### Method
Method is a function inside a class, all methods are public. It has access to fields of class object. Method is stored in function pool, class stores it index.

1. **Link to function**, uint_16

### Tag
Tag is byte primitive type signature

Tags list: 
- `u1` — `0x01` — 1 byte of uint
- `u2` — `0x02` — 2 bytes of uint 
- `u4` — `0x03` — 4 bytes of uint
- `i1` — `0x04` — 1 bytes of int
- `i2` — `0x04` — 2 bytes of int
- `i4` — `0x04` — 4 bytes of int
- `string` — `0x05` — *n = 2* bytes of length + *n* bytes of chars 
- `u8` — `0x06` — 8 bytes of uint
- `i8` — `0x07` — 8 bytes of int
- `u16` — `0x08` — 16 bytes of uint
- `i16` — `0x09` — 16 bytes of int
- `b` — `0x0A` — 1 byte of bool
- W.I.P.

### Operation Codes
- **Operation code**, 2 bytes
- **Arguments**, if mentioned

## String Descriptor
String Descriptor is a string constant, that provides you inforamtion about types.

- Primitives:
    - `int` : `I;`
    - `uint` : `U;`
    - `int8` : `I1;`
    - `int16` : `I2;`
    - `int64` : `I8;`
    - `int128` : `I16;`
    - `uint8` : `U1;`
    - `uint16` : `U2;`
    - `uint64` : `U8;`
    - `uint128` : `U16;`
    - `bool`: `B;`
    - `string` : `String;`
- Array is stated as `[` symbol, it doesn't have `;` on the end because it's a composite type and also no other type do have `[` in it. After `[` descriptor of inner type is stated.
    - `array<int>` : `[I;`
    - `array<array<int>>` : `[[I;`
- Classes:
    - W.I.P.
  
## Operation Codes List

- `ldc` — `0x0001 + u2` constant index
- `dup` — `0x0002`
- `swap` — `0x0003`
- `store` — `0x0004 + u2` variable index
- `ldv` — `0x0005 + u2` variable index
- `add` — `0x0006`
- `print` — `0x0007`
- `ret` — `0x0008`
- `halt` — `0x0009`
- `newarr` — `0x000A + u2` constant index, array type descriptor
- `stelem` — `0x000B`
- `ldelem` — `0x000C`
- `mul` — `0x000D`
- `min` — `0x000E`
- `sub` — `0x000F`
- `div` — `0x0010`
- `call` — `0x0011 + u2` function index
- `eq` — `0x0012`
- `lt` — `0x0013`
- `leq` — `0x0014`
- `jmp` — `0x0015 + u2` opcode index
- `jz` — `0x0016 + u2` opcode index
- `jnz` — `0x0017 + u2` opcode index
- W.I.P.
