# Execution Engine

## Purpose

The Execution Engine runs the bytecode contained in ".ball" files. It reads the bytecode instruction by instruction, utilizes data and information stored in various memory areas, and executes those instructions.

## Structure

Execution Engine consists of the following parts:

1) [Interpreter](interpreter.md) which translates bytecode into machine code;

2) [Garbage Collector](garbage-collector.md) which removes objects unreachable from the program;

3) [Profiler](profiler.md) which identifies areas of optimization for the JIT compiler;

4) [JIT Compiler](jit-compiler.md) which speeds up the execution by compiling frequently repeated parts of bytecode to machine code.
