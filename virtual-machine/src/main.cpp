#include <iostream>

#include "virtual_machine.hpp"
#include "common.hpp"

struct CmdOptions {
    std::string ball_path;
    uint32_t max_heap_size = 0;
    bool is_set_max_heap_size = false;
};

CmdOptions parseArguments(int argc, char* argv[]) {
    CmdOptions options;

    if (argc < 3) {
        throw std::runtime_error("Missing arguments. Use -p <file> [-mhs <number>]");
    }

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
        }
        else {
            throw std::runtime_error("Unknown argument: " + arg);
        }
    }

    if (options.ball_path.empty()) {
        throw std::runtime_error("Parameter -p is required");
    }

    return options;
}

int main(int argc, char* argv[]) {
    try {
        CmdOptions opts = parseArguments(argc, argv);
        
        czffvm::VirtualMachine vm = 
            (opts.is_set_max_heap_size)
            ? czffvm::VirtualMachine(opts.max_heap_size)
            : czffvm::VirtualMachine();
        vm.LoadProgram(opts.ball_path);

        vm.Run();
    } catch (const std::exception& e) {
        std::cerr << "Exception: " << e.what() << std::endl;
    }
}
