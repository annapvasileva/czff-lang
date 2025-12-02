# Bytecode Generator

## Algorithm of work

1. Getting AST from parser
2. Creating object, that will be serialized into bytecode. Object called **Ball**
3. Going through AST
4. Adding source from it to **Ball**
5. Seriallizing **Ball**

## Ball

It is object, that will be serialized into [bytecode](./bytecode.md).

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