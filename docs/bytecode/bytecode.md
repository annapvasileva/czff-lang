# Bytecode

- [Structure](#structure)
- [Operation Codes List](#operation-codes-list)

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
>All constants are stored here. In [Types](#types) section will be mentioned types for data, everything excluding *length* is stored here. 
3. **Classes pool** includes:
    1. **Classes length**, uint_16
    2. **Classes**, class[x]

## Types

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
Method is a function inside a class, all methods are public

1. **Name**, string
2. **Parameters number**, uint_16
3. **Parameters**, string descriptor
4. **Return type link**, string descriptor
6. **Max stack used**, uint_16
7. **Locals length**, uint_16 
6. **Code length**, uint_16
7. **Code**, op_code[]

### Tag
Tag is byte primitive type signature

Tags list: 
- `u1` — `0x1`
- `u2` — `0x2` 
- `u4` — `0x3` 
- `i4` — `0x4`
- `string` — `0x5`
- W.I.P
- `class` — `0xF`

### Operation Codes
- **Operation code**, byte
- **Argument length**, uint_16
- **Arguments**, string descriptor

## String Descriptor
To do

## Operation Codes List
To do
