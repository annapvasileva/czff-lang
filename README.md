# CZFF-lang specification

1. [Overview](#overview)

    * [Purpose and Concepts](#purpose-and-concepts)

    * [System Architecture](#system-architecture)

        * [System Pipeline](#pipeline)

    * [Implementation Languages](#implementation-languages)

2. [Language Syntax and Semantics](#language-syntax-and-semantics)

    * [Characters](#characters)

    * [Comments](#comments)

    * [Tokens](#tokens)

    * [Literals](#literals)

    * [Constants](#constants)

    * [Types](#types)

        * [Primitive Types](#primitive-types)

        * [Composite Types](#composite-types)

        * [User-defined Data Types](#user-defined-data-types)

    * [Expressions and Operators](#expressions-and-operators)

        * [Function Calls](#function-calls)

        * [Arithmetic Operators](#arithmetic-operators)

        * [Comparison Operators](#comparison-operators)

        * [Logical Operators](#logical-operators)

        * [Assignment](#assignment)

        * [Ternary Conditional Operator](#ternary-conditional-operator)

        * [Conversions](#conversions)

        * [Priority and Order of Operations](#priority-and-order-of-operations)

    * [Blocks, Scopes and Declarations](#blocks-scopes-and-declarations)

        * [Constant Declarations](#constant-declarations)

        * [Variable Declarations](#variable-declarations)

        * [Function Declarations](#function-declarations)

        * [Class Declarations](#class-declarations)

    * [Control Flow](#control-flow)

        * [Conditions](#conditions)

        * [Loops](#loops)

        * [Exceptions](#exceptions)

3. [System Components](#system-components)

    * [Lexer and Parser](#lexer-and-parser)

    * [Bytecode Compiler](#bytecode-compiler)

    * [Virtual Machine (Runtime)](#virtual-machine-runtime)

        * [VM Architecuture](#vm-architecture)

        * [Memory Model and Management](#memory-model-and-management)

        * [Just-In-Time (JIT) Compiler](#jit-compiler)

4. [Standard Library](#standard-library)

5. [Code Examples](#code-examples)

## Overview

### Purpose and Concepts

* Imperative programming language

* Automatic memory management

* Interpreter + JIT-compiler for optimizing hot spots

* Statically typed

* Single-threaded

* Entry point - точка входа - (например, функция int main();)

### System Architecture

#### Pipeline

* Lexer

* Parser

* Semantic analysis

* Intermediate Code Generation

* Optimizations

* Virtual machine

* Garbage Collector

* JIT-compiler (and Profiler)

### Implementation Languages

| Component | Language | Justification |
| --------- | -------- | ------- |
| `Lexer/Parser` |  |  |
| `Compiler` | |  |
| `Virtual Machine` |  |  |

## Language Syntax and Semantics

### Characters

Newline, whitespace…, decimal digits, letters

### Comments

* One line

* Multiple lines

### Tokens

* Identifiers (названия переменных и типов)

* Keywords (if, for, while…)

* Operators (+, -, %...)

### Literals

* numeric

* string

### Constants

### Types

#### Primitive Types

* Bool

* Integer

#### Composite Types

* String

* Array

#### User-Defined Data Types

* Classes

* Structures

### Expressions and Operators

#### Function Calls

#### Arithmetic Operators

#### Comparison Operators

#### Logical Operators

#### Assignment

#### Ternary Conditional Operator

### Conversions

* Implicit

* Explicit

### Priority and Order of Operations

### Blocks, Scopes and Declarations

#### Constant Declarations

#### Variable Declarations

#### Function Declarations

#### Class Declarations

### Control Flow

#### Conditions

if, else, elif...

#### Loops

for, while + break, continue, return(?), (?) do-while, (?) foreach

#### Exceptions

## System Components

### Lexer and Parser

### Bytecode Compiler

### Virtual Machine (Runtime)

#### VM Architecture

#### Memory Model and Management

Memory model and management + how to pass data (by value/reference)

#### JIT Compiler

## Standard Library

* input, output

* built-in functions

    * min, max

    * ...

* ...

## Code Examples

* Factorial calculation

        var n = 10;
        print(factorial(n));

* Array Sorting

        array<int> arr
        merge_sort(arr)

* Prime Number Generation

        var n = 10;
        var arr := sieve(n);

* Maybe smth else
