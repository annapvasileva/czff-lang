param (
    [Parameter(Mandatory=$true)]
    [string]$Source,

    [Parameter(Mandatory=$true)]
    [string]$Config,

    [Parameter(Mandatory=$true)]
    [string]$Target,
    
    [Parameter(Mandatory=$false)]
    [int]$Memory,

    [Parameter(Mandatory=$false)]
    [switch]$NoJit
)

$compilerPath = ".\build\Compiler\Compiler.exe"
$vmPath = ".\build\vm\czffvm.exe"

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

# -----------------------
# Compile
# -----------------------

$compileTimer = [System.Diagnostics.Stopwatch]::StartNew()

& $compilerPath -s $Source -c $Config -t $Target

$compileTimer.Stop()

if ($LASTEXITCODE -ne 0) {
    Write-Host "Compilation failed" -ForegroundColor Red
    exit 1
}

if (!(Test-Path $Target)) {
    Write-Host "Target file was not created: $Target" -ForegroundColor Red
    exit 1
}

# -----------------------
# Run VM
# -----------------------

$runTimer = [System.Diagnostics.Stopwatch]::StartNew()

$vmArgs = @("-p", $Target)

if ($PSBoundParameters.ContainsKey("Memory")) {
    Write-Host "Running VM with memory limit: $Memory bytes"
    $vmArgs += @("-mhs", $Memory)
}
else {
    Write-Host "Running VM with auto memory limit"
}

if ($NoJit) {
    Write-Host "Running VM with JIT disabled"
    $vmArgs += "--no-jit"
}

& $vmPath $vmArgs

$runTimer.Stop()

# -----------------------
# Cleanup
# -----------------------

Remove-Item $Target -Force

# -----------------------
# Stats
# -----------------------

Write-Host ""
Write-Host "Compile time: $([math]::Round($compileTimer.Elapsed.TotalMilliseconds, 2)) ms" -ForegroundColor Yellow
Write-Host "Run time: $([math]::Round($runTimer.Elapsed.TotalMilliseconds, 2)) ms" -ForegroundColor Yellow
