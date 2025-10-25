# CZFF-lang specification

1. [Purpose, concepts](#purpose-concepts)

2. [Syntax and semantics](#syntax-and-semantics)

    * [Characters](#characters)

    * [Comments](#comments)

    * [Tokens](#tokens)

    * [Literals](#literals)

    * [Constants](#constants)

3. [Types](#types)

    * [Primitive types](#primitive-types)

    * [Composite types](#composite-types)

    * (?) [User-defined data types](#user-defined-data-types)

4. [Expressions and operators](#expressions-and-operators)

    * [Function calls](#function-calls)

    * [Arithmetic operators](#arithmetic-operators)

    * [Comparison operators](#comparison-operators)

    * [Logical operators](#logical-operators)

    * [Assignment](#assignment)

    * (?) [Ternary conditional operator](#-ternary-conditional-operator)

    * [Conversions](#conversions)

    * [Priority and order of operations](#priority-and-order-of-operations)

5. [Blocks, scopes, declarations](#blocks-scopes-declarations)

    * [Constant declarations](#constant-declarations)

    * [Variable declarations](#variable-declarations)

    * [Function declarations](#function-declarations)

    * (?) [Class structure declarations](#class-structure-declarations)

6. [Control Flow](#control-flow)

    * [Conditions](#conditions)

    * [Loops](#loops)

    * [Exceptions](#exceptions)

7. [Memory model and management](#memory-model-and-management)

8. [Standard library](#standard-library)

9. [Pipeline](#pipeline)

10. [Code examples](#code-examples)

11. [Other](#other)

<hr>

## Purpose, concepts

* Imperative programming language

* Automatic memory management

* Interpreter + JIT-compiler for optimizing hot spots

* Statically typed

* Single-threaded

* Entry point - точка входа - (например, функция int main();)


## Syntax and semantics

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

## Types

### Primitive types

* Bool

* Integer

### Composite types

* String

* Array

### (?) User-defined data types

* Classes

* Structures

## Expressions and operators

### Function calls

### Arithmetic operators

### Comparison operators

### Logical operators

### Assignment

### (?) Ternary conditional operator

### Conversions

* неявные (типа из int в bool)

* явные (возможно, нам это не надо)

### Priority and order of operations

## Blocks, scopes, declarations

### Constant declarations

### Variable declarations

### Function declarations

### (?) Class structure declarations

## Control Flow

### Conditions

if, else, elif...

### Loops

for, while + break, continue, return(?), (?) do-while, (?) foreach

### Exceptions

## Memory model and management

Memory model and management + how to pass data (by value/reference)

## Standard library

* input, output

* built-in functions

    * min, max

    * ...

* ...

## Pipeline

* Lexer

* Parser

* Semantic analysis

* Intermediate code generation

* Optimizations

* Virtual machine 

* Garbage Collector

* JIT-compiler (and Profiler)

## Code examples

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

## Other

* Brackets: “()”, “{}”, “[]”

* Quotation marks (“”, ‘’)
