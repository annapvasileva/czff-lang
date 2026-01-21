# Runtime Data Area

## Common Principles

When starting up, the CVM creates a set of specialized memory areas to store the data needed to execute the Czff program. Together with the rules for interacting with these areas, they constitute the _Runtime Data Area_.

## Areas

### Method Area

Method Area stores following types of objects:

* class structures (names, metadata, etc.),

* method bytecode,

* static variables,

* constants (Constant Pool),

* method references.

> If CVM cannot allocate space in the Method Area , an `out_of_memory_error : Metaspace` is thrown.

### Heap

Stores all value-typed objects. This is where [Garbage Collector](./execution-engine/garbage-collector.md) works.

> If CVM cannot allocate space in the Heap, an `out_of_memory_error : Heap` is thrown.

### Stack Memory

Stores _frames_ (call frames), or _method execution contexts_.

Each frame contains:

* local variables,

* operand stack (operands for calculations),

* runtime constant pool reference (reference to class constants).

> If the stack overflows (recursion is too deep) `stack_overflow_error : Function Call Stack` is thrown.

### PC Register

Program Counter register stores address of the currently executing bytecode instruction.

### Native Method Stack

Native Method Stack is used for calling native (non-Czff) methods.

It contains native frames and data needed to perform such methods.

> If Native Method Stack overflows `stack_overflow_error : Native Stack` is thrown.

## Runtime

When a program is executed:

* The CVM loads the class and its structure and bytecode are placed in the [Method Area](#method-area).

* When a method is called, a new frame is created on the [Stack](#stack-memory).

* When objects are created within the method ​​they are allocated on the [Heap](#heap).

* The [PC register](#pc-register) indicates which bytecode instruction is currently being executed.

* If the method calls native code, control is transferred to the [Native Method Stack](#native-method-stack).
