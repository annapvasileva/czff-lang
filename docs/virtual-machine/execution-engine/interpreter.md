# Interpreter

## Purpose

The purpose of the CZFF Virtual Machine Interpreter (CVM Interpreter) is to execute compiled ".ball" bytecode files by decoding their structure, loading constants, classes and methods into the runtime environment, and performing operations defined by the bytecode instruction set.
The interpreter provides deterministic execution of CZFF programs, including stack-based evaluation, method invocation, field access, and control-flow operations.

Its responsibilities include:

* Parsing and validating bytecode structure.

* Managing program state: call stack, operand stacks, local variables.

* Resolving references to constants, classes, fields, and methods.

* Executing bytecode instructions in the correct order.

* Handling runtime errors and exceptional conditions.

## Architecture

The СVM uses a stack-based virtual machine architecture. Program execution occurs in an infinite execution loop until a termination instruction or an unhandled exception is encountered.

All arithmetic, logical operations and parameter passing are performed through the operand stack of the current frame.

### Components

#### Runtime Environment

Contains all loaded entities extracted from the bytecode file:

**Constants Pool** — raw constants parsed from the bytecode (u1, u2, u4, i4, strings, class references, etc.)

**Classes Pool** — classes, their fields, and method structures

**Method Area** — a mapping of method descriptors to executable units

#### Bytecode Executor

Responsible for interpreting opcodes one by one.
Includes:

* Instruction decoder

* Instruction dispatcher

* Operand stack operations

* Arithmetic and logical execution unit

* Branch execution logic

* Invocation subsystem (creating new frames)

#### Frame Structure

Each frame is created on method invocation and destroyed on method return. It includes:

* Operand Stack — used for all evaluations and storing intermediate results.

* Locals Table — fixed-size array of variables (parameters + declared locals).

* PC Register — instruction pointer for the method’s bytecode array.

* Return Address — host VM mechanism to resume when return instruction is executed.

#### Call Stack

A stack of frames, where each frame contains:

* Operand stack

* Local variables array

* Reference to the constant pool

* Reference to declaring class

* Program counter (PC)

* Method metadata (parameters count, return type, code length, etc.)

* Only the top frame is active during execution.

## Execution Algorithm

The interpreter processes bytecode using the following steps:

### 1. Initialization Phase

1. Read and validate the bytecode header:

    * magic number `0x62616c6c` (meaning that this is a ".ball" file)

    * version

    * flags

2. Load the constants pool:

    * allocate internal structures for each entry

    * decode data by tag types

3. Load classes into the runtime environment:

    * parse fields and methods

    * store method metadata, including parameters, max stack, locals count, and code

4. Locate the entry point (e.g., class Main, method main or other predefined entry).

### 2. Frame Creation

To begin execution:

1. Create the initial frame for the entry method.

2. Initialize:

    * operand stack (empty, but capacity = max stack)

    * locals table (pre-filled with parameters if any)

    * PC = 0

3. Push this frame onto the call stack.

### 3. Execution Loop

The VM enters the main loop:

```pseudocode
while (true):
    opcode = currentFrame.code[PC]
    PC += 1
    execute(opcode)
```

The high-level algorithm for each cycle:

1. **Fetch**

    Read the byte at PC as the opcode.

2. **Decode**

    Determine the number of arguments for this instruction (from incoming bytecode).

    Parse arguments according to their string descriptors (e.g., constant ref, index, branch offset).

3. **Execute** instruction:

    Operations include:

    * **Stack Manipulation**: push, pop, dup, swap

    * **Arithmetic**: iadd, isub, imul, idiv, irem

    * **Logical**: and, or, not, comparisons

    * **Constant Loading**: load constant from pool and push to stack

    * **Local Variable Ops**: load/store local index

    * **Object Ops**: get_field, put_field, new_object

    * **Control Flow**: if_xxx, goto, return

    * **Method Invocation**: invoke_virtual, invoke_static

    * **Termination**: halt, return from last frame

4. **Update PC**

    Jump instructions adjust PC relative or absolute based on operand values.

5. Error Handling

    If an invalid opcode, index out of bounds, or other violation occurs, the VM throws an unhandled exception and terminates execution.

### 4. Method Invocation

When calling a method:

1. Pop required parameters from operand stack.

2. Create a new frame:

    * move parameters into its local variables

    * allocate new operand stack

    * PC = 0

3. Push the new frame onto the call stack.

4. Continue execution from the new frame’s PC.

### 5. Method Return

When a return instruction is executed:

1. If a value must be returned:

    * pop it from operand stack of the current frame

2. Pop the current frame from the call stack

3. If a caller frame exists:

    * push the return value onto its operand stack

4. If the call stack is empty:

    * halt execution

### 6. Program Termination

Execution stops when:

1. A return instruction eliminates the last frame.

2. A dedicated halt instruction is executed.

3. An unhandled exception occurs.
