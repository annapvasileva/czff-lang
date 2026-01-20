cmake_minimum_required(VERSION 3.20)

project(czff_virtual_machine
    VERSION 0.1.0
    DESCRIPTION "Czff Virtual Machine"
    LANGUAGES CXX
)

# ===== C++ standard =====
set(CMAKE_CXX_STANDARD 20)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
set(CMAKE_CXX_EXTENSIONS OFF)

# ===== Options =====
option(CZFF_ENABLE_WARNINGS "Enable compiler warnings" ON)

if(NOT CMAKE_BUILD_TYPE)
    message(STATUS "[CZFF] CMAKE_BUILD_TYPE is not set! Setting it to Release")
    set(CMAKE_BUILD_TYPE "Release" CACHE STRING "Build type" FORCE)
endif()

if(CMAKE_BUILD_TYPE STREQUAL "Debug" OR CMAKE_CONFIGURATION_TYPES MATCHES "Debug")
    message(STATUS "[CZFF] Debug build configuration detected")

    add_compile_definitions(DEBUG_BUILD)

    if(MSVC)
        add_compile_options(/Zi /EHsc /W4 /DEBUG)
    else()
        add_compile_options(-g3 -O0 -Wall -Wextra -pedantic)
    endif()
endif()

# ===== Library =====
add_library(czff_virtual_machine_lib
    src/virtual_machine.cpp
    src/interpreter.cpp
    src/class_loader.cpp
    src/common.cpp
    src/runtime_data_area/call_frame.cpp
    src/runtime_data_area/runtime_data_area.cpp
    src/runtime_data_area/method_area.cpp
    src/runtime_data_area/heap_data_area.cpp
    src/runtime_data_area/stack_data_area.cpp
    src/util/int128.cpp
    src/util/uint128.cpp
    src/util/ball_disassembler.cpp
)

# ===== AsmJit Library =====
set(ASMJIT_STATIC ON CACHE BOOL "Build AsmJit as static library" FORCE)
set(ASMJIT_EMBED OFF CACHE BOOL "Don't embed AsmJit" FORCE)

if(WIN32)
    add_definitions(-DASMJIT_STATIC)
endif()

add_subdirectory(extern/asmjit)

target_include_directories(czff_virtual_machine_lib
    PUBLIC
        ${PROJECT_SOURCE_DIR}/include
        ${PROJECT_SOURCE_DIR}/include/runtime_data_area
        ${PROJECT_SOURCE_DIR}/include/util
        ${PROJECT_SOURCE_DIR}/extern/asmjit
)

# ===== Warnings =====
if (CZFF_ENABLE_WARNINGS)
    if (MSVC)
        target_compile_options(czff_virtual_machine_lib PRIVATE /W4)
    else()
        target_compile_options(czff_virtual_machine_lib PRIVATE -Wall -Wextra -Wpedantic)
    endif()
endif()

# ===== JIT compiler architecture detection =====
set(CZFF_JIT_ARCH "auto"
    CACHE STRING "JIT architecture (`auto`, `x64` (or `x86_64`, `amd64`), `arm64`, `riscv64`, `none`)"
)
if (CZFF_JIT_ARCH STREQUAL "auto")
    set(DETECTED_JIT_ARCH ${CMAKE_SYSTEM_PROCESSOR})
else()
    set(DETECTED_JIT_ARCH ${CZFF_JIT_ARCH})
endif()

string(TOLOWER "${DETECTED_JIT_ARCH}" DETECTED_JIT_ARCH)

if (DETECTED_JIT_ARCH MATCHES "x86_64|amd64|x64")
    set(JIT_ARCH "x86_64")
elseif (DETECTED_JIT_ARCH MATCHES "aarch64|arm64")
    set(JIT_ARCH "arm64")
elseif (DETECTED_JIT_ARCH MATCHES "riscv64")
    set(JIT_ARCH "riscv64")
elseif (DETECTED_JIT_ARCH STREQUAL "none")
    set(JIT_ARCH "none")
else()
    set(JIT_ARCH "unknown")
endif()

if(JIT_ARCH STREQUAL "x86_64") # supported architectures
    message(STATUS "[CZFF] Selected JIT architecture: ${JIT_ARCH}")

    add_library(czff_jit_${JIT_ARCH}
        src/jit/jit_compiler.cpp
        src/jit/generic_jit_optimizer.cpp
        src/jit/jit_${JIT_ARCH}.cpp
        src/jit/jit_dummy.cpp
    )

    string(TOUPPER "${JIT_ARCH}" DEF_JIT_ARCH)
    
    target_compile_definitions(czff_jit_${JIT_ARCH}
        PUBLIC
            CZFF_JIT_${DEF_JIT_ARCH}
    )

    target_include_directories(czff_jit_${JIT_ARCH}
        PUBLIC
            ${PROJECT_SOURCE_DIR}/include
            ${PROJECT_SOURCE_DIR}/extern/asmjit
    )

    target_link_libraries(czff_jit_${JIT_ARCH}
        PRIVATE
            asmjit
            czff_virtual_machine_lib
    )
else()
    if (JIT_ARCH STREQUAL "none")
        message(STATUS "[CZFF] JIT compilation is disabled")
    elseif (JIT_ARCH STREQUAL "unknown")
        message(WARNING "[CZFF] Unknown architecture was selected for JIT compiler (${DETECTED_JIT_ARCH}). JIT compilation is disabled")
    else()
        message(WARNING "[CZFF] JIT-compilation is not supported for ${JIT_ARCH} architecture. JIT compilation is disabled")
    endif()

    add_library(czff_jit_${JIT_ARCH}
        src/jit/jit_compiler.cpp
        src/jit/generic_jit_optimizer.cpp
        src/jit/jit_dummy.cpp
    )
    
    target_compile_definitions(czff_jit_${JIT_ARCH}
        PUBLIC
            CZFF_JIT_DISABLED
    )
    
    target_include_directories(czff_jit_${JIT_ARCH}
        PUBLIC
            ${PROJECT_SOURCE_DIR}/include
    )

    target_link_libraries(czff_jit_${JIT_ARCH}
        PRIVATE
            czff_virtual_machine_lib
    )
endif()

# ===== Executable =====
add_executable(czffvm
    src/main.cpp
)

target_link_libraries(czffvm
    PRIVATE
        czff_virtual_machine_lib
        czff_jit_${JIT_ARCH}
)

# ===== Tests =====
enable_testing()
add_subdirectory(tests)
