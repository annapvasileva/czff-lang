# Garbage Collector

The Garbage Collector (GC) serves as an automatic memory manager. It provides reclaiming memory by deleting objects that the program can no longer reach. This prevents unused objects from occupying memory forever.

When no variable references an object, it becomes eligible for garbage collection.

Garbage collection starts in the generation when an allocation fails due to the lack of space.

## Generations

In Czff Garbage collector organizes objects in the heap into generations.

### Generation 0

In Generation 0 newly allocated objects are stored. Since most objects die quickly, GC frequently collects Generation 0, reclaiming memory immediately.

**Examples**: local variables, temporary objects like strings created inside a loop or lightweight helper objects.

**Promotion**: objects that endure one or more garbage collection cycles are moved to Generation 1.

### Generation 1

Generation 1 serves as an intermediate layer separating short-lived objects from long-lived ones.

**Exmaples**: objects that outlive a single method invocation but do not persist for the entire application's duration.

**Promotion**: objects that survive multiple garbage collection cycles are moved to Generation 2.

### Generation 2

**Examples**: long-lived objects, such as static data or large objects that persist for the lifetime of the program.

**Promotion**: objects in generation 2 that survive a collection remain in generation 2 until they're determined to be unreachable in a future collection.

## Detecting Garbage

During a garbage collection cycle, the collector frees memory occupied by objects the application no longer needs. It identifies these unused objects by starting from the application's _roots_ (e. g. class loaders, Metaspace/Method Area objects, root runtime structures). The collector queries the runtime to obtain this list of roots and builds a graph of all objects reachable from them. Objects absent from this graph are considered unreachable and thus treated as garbage, with their allocated memory reclaimed.

## Deleting Garbage

While scanning the managed heap, the collector locates memory blocks held by unreachable objects. For each one found, it compacts the remaining reachable objects by copying them together, thereby consolidating free space. After compaction, it updates all pointers in the roots to reflect the objects’ new addresses and adjusts the heap pointer to begin right after the last reachable object.
