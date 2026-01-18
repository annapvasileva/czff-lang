#include <iostream>

#include "virtual_machine.hpp"
#include "common.hpp"
#include "ball_disassembler.hpp"

struct CmdOptions {
    std::string ball_path;
    std::string stdlib_path;
    uint32_t max_heap_size = 0;
    bool is_set_max_heap_size = false;
    bool is_set_stdlib = false;
    bool is_set_debug_mode = false;
    bool is_set_gc_off = false;
};

CmdOptions parseArguments(int argc, char* argv[]) {
    CmdOptions options;

    if (argc < 2) {
        throw std::runtime_error("Missing arguments. Use -p <file> [-mhs <number>] [--debug]");
    }

    bool debug = false;
    bool is_gc_off = false;

    for (int i = 1; i < argc; ++i) {
        std::string arg = argv[i];

        if (arg == "-p") {
            if (i + 1 >= argc) {
                throw std::runtime_error("-p parameter requires path");
            }
            options.ball_path = argv[++i];
            if (options.ball_path.size() < 5 || options.ball_path.substr(options.ball_path.size() - 5) != ".ball") {
                throw std::runtime_error("File extension must be `.ball`");
            }
        } else if (arg == "-s") {
            if (i + 1 >= argc) {
                throw std::runtime_error("-s parameter requires path");
            }
            options.stdlib_path = argv[++i];
            if (options.stdlib_path.size() < 5 || options.stdlib_path.substr(options.stdlib_path.size() - 5) != ".ball") {
                throw std::runtime_error("File extension must be `.ball`");
            }
            options.is_set_stdlib = true;
        }
        else if (arg == "-mhs") {
            if (i + 1 >= argc) {
                throw std::runtime_error("-mhs requires a number");
            }
            try {
                long long value = std::stoll(argv[++i]);
                if (value < 0 || value > UINT32_MAX) {
                    throw std::out_of_range("Max Heap Size is out of range (uint32_t)");
                }
                options.max_heap_size = static_cast<uint32_t>(value);
                options.is_set_max_heap_size = true;
            } catch (const std::exception& e) {
                throw std::runtime_error("Invalid -mhs value");
            }
        } else if (arg == "--debug") {
            debug = true;
        } else if (arg == "--gcoff") {
            is_gc_off = true;
        } else {
            throw std::runtime_error("Unknown argument: " + arg);
        }
    }

    if (options.ball_path.empty()) {
        throw std::runtime_error("Parameter -p is required");
    }
    options.is_set_debug_mode = debug;
    options.is_set_gc_off = is_gc_off;

    return options;
}

int main(int argc, char* argv[]) {
    try {
        CmdOptions opts = parseArguments(argc, argv);

        if (opts.is_set_debug_mode) {
            czffvm::BallDisassembler disasm(opts.ball_path);
            disasm.Disassemble();
        }

        czffvm::VirtualMachine vm = 
            (opts.is_set_max_heap_size)
            ? czffvm::VirtualMachine(opts.max_heap_size, opts.is_set_gc_off)
            : czffvm::VirtualMachine(opts.is_set_gc_off);
        if (opts.is_set_stdlib) {
            vm.LoadStdlib(opts.stdlib_path);
        }
        vm.LoadProgram(opts.ball_path);

        vm.Run();
    } catch (const std::exception& e) {
        std::cerr << "Exception: " << e.what() << std::endl;
    }
}
