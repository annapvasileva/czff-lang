param (
    [Parameter(Mandatory = $true)]
    [string]$Source,

    [Parameter(Mandatory = $false)]
    [string]$Config = "config.json"
)

$compilerPath = ".\build\compiler\Compiler.exe"
$vmPath = ".\build\vm\czffvm.exe"

# -----------------------
# Check
# -----------------------

if (!(Test-Path $compilerPath)) {
    Write-Host "Compiler not found: $compilerPath" -ForegroundColor Red
    exit 1
}

if (!(Test-Path $vmPath)) {
    Write-Host "VM not found: $vmPath" -ForegroundColor Red
    exit 1
}

if (!(Test-Path $Source)) {
    Write-Host "Source file not found: $Source" -ForegroundColor Red
    exit 1
}

if (!(Test-Path $Config)) {
    Write-Host "Config file not found: $Config" -ForegroundColor Red
    exit 1
}

try {
    $runtimeConfig = Get-Content $Config -Raw | ConvertFrom-Json
}
catch {
    Write-Host "Failed to parse config.json" -ForegroundColor Red
    exit 1
}

# -----------------------
# Compiler config
# -----------------------

$compilerArgs = @()

$temporaryBall = $false
if (-not $runtimeConfig.BallFilePath -or $runtimeConfig.BallFilePath.Trim() -eq "") {
    $target = ".\temp.ball"
    $temporaryBall = $true
} else {
    $target = $runtimeConfig.BallFilePath
}

if ($runtimeConfig.ConstantFolding) {
    $compilerArgs += "--use-cf"
}

if ($runtimeConfig.DeadCodeElimination) {
    $compilerArgs += "--use-dce"
}

# -----------------------
# VM config
# -----------------------

$vmArgs = @("-p", $target)

$memoryLimit = $runtimeConfig.MemoryLimit
$enableGC = $runtimeConfig.EnableGC
$enableJIT = $runtimeConfig.EnableJIT

if ($memoryLimit -and $memoryLimit -gt 0) {
    $vmArgs += "-mhs"
    $vmArgs += $memoryLimit
}

if (-not $enableGC) {
    $vmArgs += "--gcoff"
}

if (-not $enableJIT) {
    $vmArgs += "--no-jit"
}

# -----------------------
# Compile
# -----------------------

Write-Host "=== Compiling ===" -ForegroundColor Cyan
Write-Host "Compiler args: -s $Source -t $target $($compilerArgs -join ' ')" -ForegroundColor DarkGray

$compileTimer = [System.Diagnostics.Stopwatch]::StartNew()

& $compilerPath -s $Source -t $target @compilerArgs

$compileTimer.Stop()

if ($LASTEXITCODE -ne 0) {
    Write-Host "Compilation failed" -ForegroundColor Red
    exit 1
}

if (!(Test-Path $target)) {
    Write-Host "Target file was not created: $target" -ForegroundColor Red
    exit 1
}

# -----------------------
# Run VM
# -----------------------

Write-Host "=== Running VM ===" -ForegroundColor Cyan
Write-Host "VM args: $($vmArgs -join ' ')" -ForegroundColor DarkGray

$runTimer = [System.Diagnostics.Stopwatch]::StartNew()
& $vmPath @vmArgs
$runTimer.Stop()

if ($LASTEXITCODE -ne 0) {
    Write-Host "VM execution failed" -ForegroundColor Red
    exit 1
}

# -----------------------
# Cleanup
# -----------------------

if ($temporaryBall) {
    Remove-Item $target -Force
}

# -----------------------
# Stats
# -----------------------

Write-Host ""
Write-Host "Compile time: $([math]::Round($compileTimer.Elapsed.TotalMilliseconds, 2)) ms" -ForegroundColor Yellow
Write-Host "Run time: $([math]::Round($runTimer.Elapsed.TotalMilliseconds, 2)) ms" -ForegroundColor Yellow
