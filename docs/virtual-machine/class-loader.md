# Class Loader

The Class Loader is the first subsystem executed by the CVM (Czff Virtual Machine).

Its responsibility is to load `.ball` bytecode files into memory, validate their structure, create runtime class representations, and link references between them before program execution begins. The Class Loader operates in a single-file program model: all user-defined classes must be located in one `.ball` file. The standard library is provided as separate `.ball` files and is always loaded first.

## Class Loader Input

The Class Loader accepts one or more `.ball` files:

1. **Standard Library Files** (`stdlib.ball`, etc.)
2. **User Program** (e. g. `program.ball`)

The standard library is loaded before the user program and its classes are available for reference during user class loading.

## Loading Stages

Loading is performed in the following stages:

1. **File Loading**
2. **Entry Point Resolution**

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

### 2. Entry Point Resolution

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
