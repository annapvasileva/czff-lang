param (
    [string]$Configuration = "Release"
)

$mingwPath = $env:MINGW_PREFIX
if (-not $mingwPath) {
    $mingwPath = "D:/mingw64"
}

Write-Host "Using MinGW: $mingwPath" -ForegroundColor Cyan
$env:PATH = "$mingwPath/bin;$env:PATH"

# --- C# Compiler ---
Write-Host "=== Building C# Compiler ===" -ForegroundColor Cyan
dotnet restore Compiler/Compiler.csproj
if ($LASTEXITCODE -ne 0) { exit 1 }

dotnet publish Compiler/Compiler.csproj -c $Configuration -o build/compiler
if ($LASTEXITCODE -ne 0) { exit 1 }

# --- C++ VM ---
Write-Host "=== Building C++ VM ===" -ForegroundColor Cyan
if (Test-Path "build/vm") { Remove-Item -Recurse -Force build/vm }

cmake -S virtual-machine -B build/vm `
      -G Ninja `
      -DCMAKE_BUILD_TYPE=$Configuration `
      -DCMAKE_C_COMPILER="$mingwPath/bin/cc.exe" `
      -DCMAKE_CXX_COMPILER="$mingwPath/bin/c++.exe"

if ($LASTEXITCODE -ne 0) { exit 1 }

cmake --build build/vm --config $Configuration
if ($LASTEXITCODE -ne 0) { exit 1 }

Write-Host ""
Write-Host "Build completed successfully" -ForegroundColor Green
Write-Host "Compiler: build/compiler/Compiler.exe"
Write-Host "VM: build/vm/czffvm.exe"
