# Bytecode Generator

- [Generator](#generator)
- [Serializer](#serizalizer)

## Algorithm of operation

1. Receiving the AST from the parser
2. Recieving Symbol Table from Semantic Analyzer
2. Creating an intermediate object of type **Ball** that will be serialized into bytecode.
3. Going through the AST
4. Translating AST nodes into bytecode instructions and storing them in the **Ball** object
5. Seriallizing the **Ball** object into bytecode

## Generator

### Settings

Generator writes version to bytecode, so it's being built with `Version` parameter.

### Generate

Generator works with `Generator.Generate(AST, Symbol Table)`. Use AST from Parser and Symbol Table from analyzer. Generator uses `Visitor` pattern in it to go through the tree.

## Serizalizer

Serializer works with `Serializer.Serialize(Ball, Target)`. It writes bytes in linear way, starting with header, then constant pool etc. It uses `Visitor` patter to serialize `Operations` into `OpCodes`. 

## Ball

Ball object is an object that will be serialized into the final [bytecode](./bytecode.md).

## Parsing

Instruction, how to parse .ball:

```
Ball  {
    Header {
        Magical number: 0x62616c6c;
        
        Version: [0-255].[0-255].[0-255];
        
        Flags: 0b[0-1]{8}; 
    };
    
    Constants pool {
        Length: uint_16;

        Array of elements: (tag, data)[Length];
    };

    Functions {
        Name: uint_16; // link to name from consts
                
        Parameters number: uint_16;
        
        Parameters: uint_16; // link to descriptor from consts 
        
        Return type link: uint_16; // link to descriptor from consts 
        
        Max stack used: uint_16;
        
        Locals count: uint_16;
        
        Code length: uint_16;
        
        Code: OperationCode {
            Operation code: uint_8

            Argument count: uint_16

            Arguments: uint_16 // link to descriptor from consts
        }[Code length];
    }

    Classes {
        Classes count: uint_16;

        Classes: Class {
            Name: uint_16; // link to name from consts 
            
            Fields count: uint_16; 

            Fields: Field {
                Name: uint_16; // link to name from consts
                
                Type link: uint_16; // link to descriptor from consts 
            }[Fields count];

            Methods count: uint_16

            Methods: Method {
                Name: uint_16; // link to name from consts
                
                Parameters number: uint_16;
                
                Parameters: uint_16; // link to descriptor from consts 
                
                Return type link: uint_16; // link to descriptor from consts 
                
                Max stack used: uint_16;
                
                Locals count: uint_16;
                
                Code length: uint_16;
                
                Code: OperationCode {
                    Operation code: uint_8

                    Argument count: uint_16

                    Arguments: uint_16 // link to descriptor from consts
                }[Code length];
            }[Methods count];
        }[Classes count];
    };
};
```

## Element into Bytecode table

To do