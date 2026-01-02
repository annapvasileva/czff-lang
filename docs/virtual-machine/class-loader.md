# Class Loader

The Class Loader is the first subsystem executed by the CVM (Czff Virtual Machine).

Its responsibility is to load `.ball` bytecode files into memory, validate their structure, create runtime class representations, link references between them, and initialize static data before program execution begins. The Class Loader operates in a single-file program model: all user-defined classes must be located in one `.ball` file. The standard library is provided as a separate `.ball` file and is always loaded first.

## Class Loader Input

The Class Loader accepts one or more `.ball` files:

1. **Standard Library** (`stdlib.ball`)
2. **User Program** (e. g. `program.ball`)

The standard library is loaded before the user program and its classes are available for reference during user class loading.

## Loading Stages

Class loading is performed in the following stages:

1. **File Loading**
2. **Verification**
3. **Linking**
4. **Initialization**
5. **Entry Point Resolution**

Each stage must complete successfully before the next one begins.

### 1. File Loading

During this stage the loader:

1. Reads the `.ball` file into memory.
2. Validates the file header:
   - Checks the magic number (`0x62616c6c`)
   - Verifies supported bytecode version
   - Reads and stores flags
3. Loads the **constant pool**:
   - Reads pool length
   - Parses each constant according to its tag
4. Loads the **classes pool**:
   - Reads class count
   - Parses each class definition
   - Registers classes by name in the global class table

No semantic checks are performed at this stage.

---

### 2. Verification

Verification ensures that bytecode is structurally and semantically valid before execution.

The loader verifies:

#### Class-Level Checks

- Class names are unique within the file
- Referenced classes exist (either in `stdlib` or user file)

#### Field Checks

- Field names are unique within a class
- Field type descriptors are valid

#### Method Checks

- Method names are unique within a class (parameter-based overloading is not supported _for now_)
- Parameter and return type descriptors are valid
- `max_stack` and `locals_length` are non-zero where required
- `code_length` matches actual bytecode size

#### Bytecode Checks

- All opcodes are known
- Instruction arguments match opcode descriptor
- Constant pool references are in bounds
- Stack usage does not exceed `max_stack`
- Local variable access is within `locals_length`

If any verification step fails, the Class Loader aborts execution with a descriptive error.

---

### 3. Linking

Linking resolves symbolic references and prepares classes for execution.

This stage includes:

#### Constant Pool Resolution

- Resolving class, field, and method references
- Converting symbolic references into direct pointers or indices

#### Layout Preparation

- Calculating field offsets for each class
- Preparing method metadata
- Allocating space for static fields

No code is executed during this stage.

---

### 4. Initialization

During the initialization stage, the Class Loader prepares class-level runtime state.

Initialization is performed **per class** and applies only to **static runtime members**. Instance data is not allocated during this stage.

Static members are a runtime-level concept and are not directly expressible in the CZFF source language. They are primarily used by the standard library and internal VM facilities.

#### Static Storage Allocation

For each loaded class, the Class Loader allocates static storage based on the class bytecode metadata.

Each class has exactly one static storage area associated with its runtime class representation. This storage is shared across all uses of the class and exists independently of class instances.

#### Default Value Initialization

All static fields are initialized with default values according to their type:

- Numeric types → `0`
- Boolean → `false`
- Reference types (`string`, `array`, class references) → `UNINITIALIZED`

The `UNINITIALIZED` value is a runtime-only sentinel that cannot be produced by user code.
Any attempt to read a static field in the `UNINITIALIZED` state results in a runtime error.

#### Initialization Completion

After this stage completes, all classes are considered initialized and ready for execution. Control is then transferred to the VM entry point.

---

### 5. Entry Point Resolution

After all classes are loaded and initialized, the Class Loader searches for the program entry point.

Requirements:

- Method name: `Main`
- Signature: `func void Main()`
- Must belong to exactly one class

If no valid entry point is found, the VM terminates with an error.

---

### Error Handling

Any error during loading, verification, linking, or initialization causes the CVM to terminate before execution begins.

Errors must include:

- Stage name
- Class name (if applicable)
- Human-readable description

---
