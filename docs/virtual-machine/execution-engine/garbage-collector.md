# Garbage Collector

The Garbage Collector (GC) in CzffVM is implemented as a **mark-and-sweep** memory management system.  
It automatically reclaims memory occupied by objects that are no longer reachable by the program.

Unlike generational collectors, this implementation **does not use generations** and **does not perform heap compaction**.  
Freed memory slots are reused through a free list mechanism.

Garbage collection is triggered **only when memory allocation fails** due to heap size limits (unless GC is disabled).

---

## Allocation Policy

Before allocating a new object, the heap:

1. Estimates the required memory size.
2. Checks if the allocation would exceed the configured heap limit.
3. If the limit would be exceeded and GC is enabled → **GC is executed**.
4. If memory is still insufficient after GC → throws  
   `Heap memory limit exceeded`.

Freed object slots are stored in a **free list** and reused for future allocations.

---

## Memory Model

- All objects are stored in a contiguous vector `objects_`
- Deleted objects leave empty slots (`std::optional`)
- These empty slots are tracked in `free_list_`
- New objects reuse these slots before expanding the heap

---

## Garbage Collection Process

GC consists of two main phases:

### 1. Mark Phase

The collector starts from **root references**:

- Local variables in stack frames
- Operand stack values

For each root reference:

- Recursively traverses all referenced heap objects
- Marks every reachable object (`marked = true`)
- Follows all nested `HeapRef` fields

This builds a graph of **reachable objects**.

---

### 2. Sweep Phase

The heap is scanned linearly:

- **Unmarked objects**
  - Considered garbage
  - Memory size is subtracted from `used_bytes_`
  - Slot is cleared
  - Index is added to `free_list_`

- **Marked objects**
  - Survive collection
  - Mark flag is reset for the next GC cycle

No object relocation or compaction is performed.

---

## Roots Detection

Roots are extracted from the runtime stack:

- Frame local variables
- Operand stacks

Only values of type `HeapRef` are treated as references.
All others are ignored.

---

## Memory Reclamation

When an object is collected:

- Its slot in `objects_` becomes empty
- The index is added to `free_list_`
- Future allocations reuse these slots

This avoids heap fragmentation growth but does **not move objects**.

---

## Size Estimation

Memory usage is approximated using:

- Base object size
- Type string size
- Number of fields
- Size of stored values
- Actual size of string contents

This estimation is used to:
- Track used heap size
- Decide when to trigger GC

---

## Summary

The GC in CzffVM is a **simple, deterministic mark-and-sweep collector**:

- Triggered only on allocation failure
- Traverses references from stack roots
- Reclaims unreachable objects
- Reuses freed memory slots
- Does not relocate objects
- Does not implement generational optimization

This design prioritizes **simplicity and predictability** over performance optimizations.
