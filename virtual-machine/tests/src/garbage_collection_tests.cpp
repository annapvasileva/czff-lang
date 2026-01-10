#include <gtest/gtest.h>

#include <stdexcept>
#include <vector>
#include <string>

#include "heap_data_area.hpp"
#include "stack_data_area.hpp"
#include "common.hpp"
#include "call_frame.hpp"
#include "runtime_data_area.hpp"

namespace czffvm {

class HeapTest : public ::testing::Test {
protected:
    StackDataArea stack_;
    Heap heap_;

    HeapTest() : heap_(stack_, 1000) {}

    void SetUp() override {
        
    }

    RuntimeFunction CreateDummyFunction(uint16_t locals_count = 5, uint16_t max_stack = 5) {
        RuntimeFunction rf;
        rf.locals_count = locals_count;
        rf.max_stack = max_stack;

        return rf;
    }

    void PushDummyFrame(uint16_t locals_count = 5, uint16_t max_stack = 5) {
        static thread_local std::vector<RuntimeFunction> dummy_functions;
        dummy_functions.emplace_back(CreateDummyFunction(locals_count, max_stack));
        stack_.PushFrame(&dummy_functions.back());
    }
};

TEST_F(HeapTest, UnreferencedObjectCollected) {
    HeapRef ref = heap_.Allocate("int;", {});
    heap_.Collect(0);
    EXPECT_THROW(heap_.Get(ref), std::runtime_error);
}

TEST_F(HeapTest, ReferencedFromLocalsSurvives) {
    PushDummyFrame();
    CallFrame& frame = stack_.CurrentFrame();

    HeapRef ref = heap_.Allocate("int;", {});
    frame.locals[0] = ref;

    heap_.Collect(0);

    EXPECT_NO_THROW(heap_.Get(ref));
    HeapObject& obj = heap_.Get(ref);
    EXPECT_FALSE(obj.marked);
    EXPECT_EQ(obj.generation, 1);
}

TEST_F(HeapTest, ReferencedFromOperandStackSurvives) {
    PushDummyFrame();
    CallFrame& frame = stack_.CurrentFrame();

    HeapRef ref = heap_.Allocate("int;", {});
    frame.operand_stack.push_back(ref);

    heap_.Collect(0);

    EXPECT_NO_THROW(heap_.Get(ref));
    HeapObject& obj = heap_.Get(ref);
    EXPECT_FALSE(obj.marked);
    EXPECT_EQ(obj.generation, 1);
}

TEST_F(HeapTest, ChainedReferencesSurvive) {
    PushDummyFrame();
    CallFrame& frame = stack_.CurrentFrame();

    HeapRef ref_b = heap_.Allocate("int;", {});
    HeapRef ref_a = heap_.Allocate("[int;", {ref_b});
    frame.locals[0] = ref_a;

    heap_.Collect(0);

    EXPECT_NO_THROW(heap_.Get(ref_a));
    EXPECT_NO_THROW(heap_.Get(ref_b));

    HeapObject& obj_a = heap_.Get(ref_a);
    HeapObject& obj_b = heap_.Get(ref_b);
    EXPECT_EQ(obj_a.generation, 1);
    EXPECT_EQ(obj_b.generation, 1);
}

TEST_F(HeapTest, OuterNotReferencedCollected) {
    PushDummyFrame();
    CallFrame& frame = stack_.CurrentFrame();

    HeapRef ref_b = heap_.Allocate("int;", {});
    HeapRef ref_a = heap_.Allocate("[int;", {ref_b});
    frame.locals[0] = ref_b;

    heap_.Collect(0);

    EXPECT_THROW(heap_.Get(ref_a), std::runtime_error);
    EXPECT_NO_THROW(heap_.Get(ref_b));

    HeapObject& obj_b = heap_.Get(ref_b);
    EXPECT_EQ(obj_b.generation, 1);
}

TEST_F(HeapTest, GenerationalCollection) {
    PushDummyFrame();
    CallFrame& frame = stack_.CurrentFrame();

    HeapRef ref = heap_.Allocate("int;", {});
    frame.locals[0] = ref;

    // Collect gen 0: survives, gen -> 1
    heap_.Collect(0);
    EXPECT_NO_THROW(heap_.Get(ref));
    EXPECT_EQ(heap_.Get(ref).generation, 1);

    // Collect gen 0 again: skipped since gen=1
    heap_.Collect(0);
    EXPECT_NO_THROW(heap_.Get(ref));
    EXPECT_EQ(heap_.Get(ref).generation, 1);  // Unchanged

    // Collect gen 1: survives, gen -> 2
    heap_.Collect(1);
    EXPECT_NO_THROW(heap_.Get(ref));
    EXPECT_EQ(heap_.Get(ref).generation, 2);

    // Collect gen 1 again: skipped since gen=2
    heap_.Collect(1);
    EXPECT_EQ(heap_.Get(ref).generation, 2);

    // Collect gen 2: survives, gen remains 2 (since <2 false)
    heap_.Collect(2);
    EXPECT_NO_THROW(heap_.Get(ref));
    EXPECT_EQ(heap_.Get(ref).generation, 2);

    // Remove reference and collect gen 2: collected
    frame.locals[0] = int32_t(0);  // Clear reference
    heap_.Collect(2);
    EXPECT_THROW(heap_.Get(ref), std::runtime_error);
}

TEST_F(HeapTest, MixedReferences) {
    PushDummyFrame();
    CallFrame& frame = stack_.CurrentFrame();

    HeapRef ref1 = heap_.Allocate("int;", {});  // Unreferenced
    HeapRef ref2 = heap_.Allocate("int;", {});  // Referenced
    HeapRef ref3 = heap_.Allocate("int;", {ref2});  // References ref2, but itself unreferenced

    frame.locals[0] = ref2;

    heap_.Collect(0);

    EXPECT_THROW(heap_.Get(ref1), std::runtime_error);
    EXPECT_NO_THROW(heap_.Get(ref2));
    EXPECT_THROW(heap_.Get(ref3), std::runtime_error);  // ref3 not reached from roots
}

TEST_F(HeapTest, CycleHandling) {
    PushDummyFrame();
    CallFrame& frame = stack_.CurrentFrame();

    HeapRef ref_a = heap_.Allocate("obj;", {});
    HeapRef ref_b = heap_.Allocate("obj;", {ref_a});
    // Create cycle: A -> B -> A
    heap_.Get(ref_a).fields = {ref_b};

    frame.locals[0] = ref_a;  // Root references A

    heap_.Collect(0);

    // Both survive despite cycle
    EXPECT_NO_THROW(heap_.Get(ref_a));
    EXPECT_NO_THROW(heap_.Get(ref_b));
    EXPECT_EQ(heap_.Get(ref_a).generation, 1);
    EXPECT_EQ(heap_.Get(ref_b).generation, 1);
}

TEST_F(HeapTest, NoInfiniteRecursionInCycles) {
    PushDummyFrame();
    CallFrame& frame = stack_.CurrentFrame();

    HeapRef ref_a = heap_.Allocate("obj;", {});
    HeapRef ref_b = heap_.Allocate("obj;", {ref_a});
    heap_.Get(ref_a).fields = {ref_b};  // Cycle

    frame.locals[0] = ref_a;

    heap_.Collect(0);

    EXPECT_NO_THROW(heap_.Get(ref_a));
    EXPECT_NO_THROW(heap_.Get(ref_b));
}

}  // namespace czffvm