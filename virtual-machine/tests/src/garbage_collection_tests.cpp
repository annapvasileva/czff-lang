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

    RuntimeFunction CreateDummyFunction(uint16_t locals_count = 5,
                                        uint16_t max_stack = 5) {
        RuntimeFunction rf;
        rf.locals_count = locals_count;
        rf.max_stack = max_stack;
        return rf;
    }

    void PushDummyFrame(uint16_t locals_count = 5,
                        uint16_t max_stack = 5) {
        static thread_local std::vector<RuntimeFunction> dummy_functions;
        dummy_functions.emplace_back(
            CreateDummyFunction(locals_count, max_stack)
        );
        stack_.PushFrame(&dummy_functions.back());
    }
};

TEST_F(HeapTest, UnreferencedObjectCollected) {
    HeapRef ref = heap_.Allocate("int;", {});
    heap_.Collect();

    EXPECT_THROW(heap_.Get(ref), std::runtime_error);
}

TEST_F(HeapTest, ReferencedFromLocalsSurvives) {
    PushDummyFrame();
    CallFrame& frame = stack_.CurrentFrame();

    HeapRef ref = heap_.Allocate("int;", {});
    frame.locals[0] = ref;

    heap_.Collect();

    EXPECT_NO_THROW(heap_.Get(ref));
}

TEST_F(HeapTest, ReferencedFromOperandStackSurvives) {
    PushDummyFrame();
    CallFrame& frame = stack_.CurrentFrame();

    HeapRef ref = heap_.Allocate("int;", {});
    frame.operand_stack.push_back(ref);

    heap_.Collect();

    EXPECT_NO_THROW(heap_.Get(ref));
}

TEST_F(HeapTest, ChainedReferencesSurvive) {
    PushDummyFrame();
    CallFrame& frame = stack_.CurrentFrame();

    HeapRef ref_b = heap_.Allocate("int;", {});
    HeapRef ref_a = heap_.Allocate("[int;", {ref_b});

    frame.locals[0] = ref_a;

    heap_.Collect();

    EXPECT_NO_THROW(heap_.Get(ref_a));
    EXPECT_NO_THROW(heap_.Get(ref_b));
}

TEST_F(HeapTest, OuterNotReferencedCollected) {
    PushDummyFrame();
    CallFrame& frame = stack_.CurrentFrame();

    HeapRef ref_b = heap_.Allocate("int;", {});
    HeapRef ref_a = heap_.Allocate("[int;", {ref_b});

    frame.locals[0] = ref_b;

    heap_.Collect();

    EXPECT_THROW(heap_.Get(ref_a), std::runtime_error);
    EXPECT_NO_THROW(heap_.Get(ref_b));
}

TEST_F(HeapTest, MixedReferences) {
    PushDummyFrame();
    CallFrame& frame = stack_.CurrentFrame();

    HeapRef ref1 = heap_.Allocate("int;", {});
    HeapRef ref2 = heap_.Allocate("int;", {});
    HeapRef ref3 = heap_.Allocate("int;", {ref2});

    frame.locals[0] = ref2;

    heap_.Collect();

    EXPECT_THROW(heap_.Get(ref1), std::runtime_error);
    EXPECT_NO_THROW(heap_.Get(ref2));
    EXPECT_THROW(heap_.Get(ref3), std::runtime_error);
}

TEST_F(HeapTest, CycleHandling) {
    PushDummyFrame();
    CallFrame& frame = stack_.CurrentFrame();

    HeapRef ref_a = heap_.Allocate("obj;", {});
    HeapRef ref_b = heap_.Allocate("obj;", {ref_a});

    heap_.Get(ref_a).fields = {ref_b};

    frame.locals[0] = ref_a;

    heap_.Collect();

    EXPECT_NO_THROW(heap_.Get(ref_a));
    EXPECT_NO_THROW(heap_.Get(ref_b));
}

TEST_F(HeapTest, CycleNotReferencedCollected) {
    HeapRef ref_a = heap_.Allocate("obj;", {});
    HeapRef ref_b = heap_.Allocate("obj;", {ref_a});
    heap_.Get(ref_a).fields = {ref_b};

    heap_.Collect();

    EXPECT_THROW(heap_.Get(ref_a), std::runtime_error);
    EXPECT_THROW(heap_.Get(ref_b), std::runtime_error);
}

TEST_F(HeapTest, FreeListReused) {
    HeapRef ref1 = heap_.Allocate("int;", {});
    heap_.Collect();

    EXPECT_THROW(heap_.Get(ref1), std::runtime_error);

    HeapRef ref2 = heap_.Allocate("int;", {});

    EXPECT_EQ(ref1.id, ref2.id);
}

TEST_F(HeapTest, DeepGraphSurvives) {
    PushDummyFrame();
    CallFrame& frame = stack_.CurrentFrame();

    HeapRef a = heap_.Allocate("obj;", {});
    HeapRef b = heap_.Allocate("obj;", {a});
    HeapRef c = heap_.Allocate("obj;", {b});
    HeapRef d = heap_.Allocate("obj;", {c});

    frame.locals[0] = d;

    heap_.Collect();

    EXPECT_NO_THROW(heap_.Get(a));
    EXPECT_NO_THROW(heap_.Get(b));
    EXPECT_NO_THROW(heap_.Get(c));
    EXPECT_NO_THROW(heap_.Get(d));
}

} // namespace czffvm
