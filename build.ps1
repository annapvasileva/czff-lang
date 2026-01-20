param (
    [string]$Configuration = "Release",
    [string]$Generator = "Ninja"
)

Write-Host "=== Building C# Compiler ===" -ForegroundColor Cyan

dotnet restore Compiler/Compiler.csproj
if ($LASTEXITCODE -ne 0) { exit 1 }

dotnet publish Compiler/Compiler.csproj -c $Configuration -o build/compiler
if ($LASTEXITCODE -ne 0) { exit 1 }

Write-Host "=== Building C++ VM ===" -ForegroundColor Cyan
Write-Host "Configuration: $Configuration"
Write-Host "Generator: $Generator"

# Configure
cmake -G "$Generator" -S virtual-machine -B build/vm -DCMAKE_BUILD_TYPE=$Configuration
if ($LASTEXITCODE -ne 0) { exit 1 }

# Build
cmake --build build/vm --config $Configuration
if ($LASTEXITCODE -ne 0) { exit 1 }

Write-Host ""
Write-Host "Build completed successfully" -ForegroundColor Green
Write-Host "Compiler: build/compiler/Compiler.exe"
Write-Host "VM: build/vm/czffvm.exe"
