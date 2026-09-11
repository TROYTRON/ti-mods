# Batch still-portrait conversion, adapted from Tayta's original waifu2vid script.
# Run in a folder of portrait PNGs, or supply -Path. Existing videos are preserved.
[CmdletBinding()]
param([string]$Path = '.')

$ErrorActionPreference = 'Stop'
$null = Get-Command ffmpeg -ErrorAction Stop
$portraitDirectory = (Get-Item -LiteralPath $Path -ErrorAction Stop)
if (-not $portraitDirectory.PSIsContainer) { throw 'Path must be a directory of PNG portraits.' }
$portraits = @(Get-ChildItem -LiteralPath $portraitDirectory.FullName -Filter '*.png' -File)
if ($portraits.Count -eq 0) { return }
$videoDirectory = Join-Path $portraitDirectory.FullName 'videos'
$null = New-Item -ItemType Directory -Path $videoDirectory -Force

foreach ($portrait in $portraits) {
    $outputPath = Join-Path $videoDirectory ($portrait.BaseName + '.webm')
    if (Test-Path -LiteralPath $outputPath) {
        Write-Host "Skipping existing output: $outputPath"
        continue
    }
    & ffmpeg -n -loop 1 -framerate 1 -i $portrait.FullName -frames:v 2 -an -c:v libvpx -pix_fmt yuva420p -auto-alt-ref 0 $outputPath
    if ($LASTEXITCODE -ne 0) { throw "Portrait conversion failed: $($portrait.Name) (exit $LASTEXITCODE). Inspect any incomplete output before retrying." }
}
