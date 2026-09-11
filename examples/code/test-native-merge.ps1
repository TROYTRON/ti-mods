[CmdletBinding()]
param(
    [string]$TerraInvictaDir = $env:TerraInvictaDir,
    [string]$DotnetPath = 'dotnet'
)

$ErrorActionPreference = 'Stop'
if (-not $TerraInvictaDir) {
    throw 'Supply -TerraInvictaDir with the path to your own Terra Invicta installation.'
}
$TerraInvictaDir = (Resolve-Path -LiteralPath $TerraInvictaDir).Path
Push-Location $PSScriptRoot
try {
    & $DotnetPath build 'TiMods.MergeChecks/TiMods.MergeChecks.csproj' -c Release "-p:TerraInvictaDir=$TerraInvictaDir" --nologo
    if ($LASTEXITCODE -ne 0) { throw "Merge-check build failed (exit $LASTEXITCODE)." }
    & $DotnetPath './TiMods.MergeChecks/bin/Release/net8.0/TiMods.MergeChecks.dll' (Join-Path $TerraInvictaDir 'TerraInvicta_Data/Managed')
    if ($LASTEXITCODE -ne 0) { throw "Merge checks failed (exit $LASTEXITCODE)." }
}
finally {
    Pop-Location
}
