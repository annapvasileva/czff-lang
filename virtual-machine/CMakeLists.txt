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
option(CZFF_ENABLE_SANITIZERS "Enable sanitizers (debug only)" ON)

# ===== Library =====
add_library(czff_virtual_machine_lib
    src/virtual_machine.cpp
    src/bytecode_loader.cpp
    src/call_frame.cpp
    src/interpreter.cpp
)

target_include_directories(czff_virtual_machine_lib
    PUBLIC
        ${PROJECT_SOURCE_DIR}/include
)

# ===== Warnings =====
if (CZFF_ENABLE_WARNINGS)
    if (MSVC)
        target_compile_options(czff_virtual_machine_lib PRIVATE /W4)
    else()
        target_compile_options(czff_virtual_machine_lib PRIVATE -Wall -Wextra -Wpedantic)
    endif()
endif()

# ===== Sanitizers (Debug only) =====
if (CZFF_ENABLE_SANITIZERS AND CMAKE_BUILD_TYPE STREQUAL "Debug")
    if (NOT MSVC)
        target_compile_options(czff_virtual_machine_lib PRIVATE -fsanitize=address,undefined)
        target_link_options(czff_virtual_machine_lib PRIVATE -fsanitize=address,undefined)
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
        src/jit/jit_${JIT_ARCH}.cpp
    )

    string(TOUPPER "${JIT_ARCH}" DEF_JIT_ARCH)
    
    target_compile_definitions(czff_jit_${JIT_ARCH}
        PUBLIC
            CZFF_JIT_${DEF_JIT_ARCH}
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
        src/jit/jit_dummy.cpp
    )
    
    target_compile_definitions(czff_jit_${JIT_ARCH}
        PUBLIC
            CZFF_JIT_DISABLED
    )

endif()

target_include_directories(czff_jit_${JIT_ARCH}
    PUBLIC
        ${PROJECT_SOURCE_DIR}/include
)


add_library(czff_garbage_collector
    src/garbage_collector/classic_garbage_collector.cpp
)

target_include_directories(czff_garbage_collector
    PUBLIC
        ${PROJECT_SOURCE_DIR}/include
)

# ===== Executable =====
add_executable(czff_virtual_machine
    src/main.cpp
)

target_link_libraries(czff_virtual_machine
    PRIVATE
        czff_jit_${JIT_ARCH}
        czff_virtual_machine_lib
        czff_garbage_collector
)
