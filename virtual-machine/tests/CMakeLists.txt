include(FetchContent)

FetchContent_Declare(
    googletest
    GIT_REPOSITORY https://github.com/google/googletest.git
    GIT_TAG release-1.12.1
)

set(gtest_force_shared_crt ON CACHE BOOL "" FORCE)
FetchContent_MakeAvailable(googletest)

enable_testing()

add_executable(
    tests
    class_loader_tests.cpp
    vm_tests.cpp
    byte_reader_tests.cpp
)

target_link_libraries(
    tests
    czff_virtual_machine_lib
    czff_garbage_collector
    GTest::gtest_main
)

target_include_directories(tests PUBLIC ${PROJECT_SOURCE_DIR})

include(GoogleTest)

gtest_discover_tests(tests)