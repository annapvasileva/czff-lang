include(FetchContent)

FetchContent_Declare(
    googletest
    GIT_REPOSITORY https://github.com/google/googletest.git
    GIT_TAG v1.14.0
)

set(gtest_force_shared_crt ON CACHE BOOL "" FORCE)
FetchContent_MakeAvailable(googletest)

enable_testing()

add_executable(
    tests
    src/class_loader_tests.cpp
    src/vm_tests.cpp
    src/byte_reader_tests.cpp
    src/interpreter_tests.cpp
    src/garbage_collection_tests.cpp
    src/int128_tests.cpp
)

add_library(
    test_lib
    src/class_loader_test_util.cpp
)

target_include_directories(test_lib
    PUBLIC
        ${PROJECT_SOURCE_DIR}/tests/include
)

target_link_libraries(
    tests
    test_lib
    czff_virtual_machine_lib
    GTest::gtest_main
)

target_include_directories(tests PUBLIC ${PROJECT_SOURCE_DIR})


if(JIT_ARCH STREQUAL "x86_64") # supported architectures
    add_executable(
        jit_tests
        src/jit/jit_${JIT_ARCH}_tests.cpp
        src/jit/jit_dummy_tests.cpp
    )
else()
    add_library(
        jit_tests
        src/jit/jit_dummy_tests.cpp
    )
endif()

target_link_libraries(
    jit_tests
    czff_virtual_machine_lib
    czff_jit_${JIT_ARCH}
    GTest::gtest_main
)

target_include_directories(jit_tests PUBLIC ${PROJECT_SOURCE_DIR})


include(GoogleTest)

gtest_discover_tests(tests)