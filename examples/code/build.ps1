[CmdletBinding()]
param(
    [string]$TerraInvictaDir = $env:TerraInvictaDir,
    [string]$UnityModManagerDir = $env:UnityModManagerDir,
    [string]$DotnetPath = 'dotnet',
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release'
)

$ErrorActionPreference = 'Stop'
if (-not $TerraInvictaDir) {
    throw 'Supply -TerraInvictaDir with the path to your own Terra Invicta installation.'
}
$TerraInvictaDir = (Resolve-Path -LiteralPath $TerraInvictaDir).Path
$projects = @('TiMods.Starter', 'TiMods.Console', 'TiMods.TemplatePatch', 'TiMods.SaveState', 'TiMods.Diagnostics')

Push-Location $PSScriptRoot
try {
    foreach ($project in $projects) {
        $buildArguments = @('build', "$project/$project.csproj", '-c', $Configuration,
            "-p:TerraInvictaDir=$TerraInvictaDir", '--nologo')
        if ($UnityModManagerDir) {
            $buildArguments += "-p:UnityModManagerDir=$UnityModManagerDir"
        }
        & $DotnetPath @buildArguments
        if ($LASTEXITCODE -ne 0) {
            throw "Build failed: $project (exit $LASTEXITCODE)."
        }
        $package = Join-Path $PSScriptRoot "$project/bin/$Configuration/net48/package/$project"
        $unexpected = Get-ChildItem -LiteralPath $package -File |
            Where-Object { $_.Name -notin @('ModFile.json', "$project.dll") }
        if ($unexpected) {
            throw "Unexpected files in $package. Distribute only the mod DLL and ModFile.json."
        }
        Write-Host "Package: $package"
    }
}
finally {
    Pop-Location
}
