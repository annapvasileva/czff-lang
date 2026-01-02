# Class Loader

The Class Loader is the first subsystem executed by the CVM (Czff Virtual Machine).

Its responsibility is to load `.ball` bytecode files into memory, validate their structure, create runtime class representations, and link references between them before program execution begins. The Class Loader operates in a single-file program model: all user-defined classes must be located in one `.ball` file. The standard library is provided as separate `.ball` files and is always loaded first.

## Class Loader Input

The Class Loader accepts one or more `.ball` files:

1. **Standard Library Files** (`stdlib.ball`, etc.)
2. **User Program** (e. g. `program.ball`)

The standard library is loaded before the user program and its classes are available for reference during user class loading.

## Loading Stages

Class loading is performed in the following stages:

1. **File Loading**
2. **Verification**
3. **Linking**
4. **Entry Point Resolution**

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
5. Loads the **global functions pool**:
    - Reads functions count
    - Parses each function definition
    - Registers functions by name in the global function table

Global class/function tables are shared across stdlib and user file.

No semantic checks are performed at this stage.

---

### 2. Verification

Verification ensures that bytecode is structurally and semantically valid before execution.

The loader verifies:

#### Class-Level Checks

- Class names are unique within the two files
- Referenced classes exist (either in `stdlib` or user file)

#### Field Checks

- Field names are unique within a class
- Field type descriptors are valid

#### Method Checks

- Method names are unique within a class (parameter-based overloading is not supported _for now_)
- Parameter and return type descriptors are valid
- `max_stack` and `locals_length` are non-zero where required
- `code_length` matches actual bytecode size

#### Function Checks

- Function names are unique within the two files (parameter-based overloading is not supported _for now_)
- Referenced functions exist (either in `stdlib` or user file)
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

- Resolving class, field, method, and function references
- Converting symbolic references into direct pointers or indices

#### Layout Preparation

- Calculating field offsets for each class
- Preparing method metadata

No code is executed during this stage.

---

### 4. Entry Point Resolution

After all classes are loaded, the Class Loader searches for the program entry point.

Requirements:

- Function name: `Main` (case-sensitive)
- Signature: `func void Main()`
- Must be a global function
- Exactly one entry point must exist

If no valid entry point is found, the VM terminates with an error.

---

### Error Handling

Any error during loading, verification, linking, or initialization causes the CVM to terminate before execution begins.

Errors must include:

- Stage name
- Class name (if applicable)
- Human-readable description

---
