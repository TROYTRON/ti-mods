# C# build environment

Install the compiler and locate your own game/loader assemblies, then use the single set of [build and package commands](../examples/code/README.md#build). Unity Editor is unnecessary for code-only mods.

## Compiler and editor

The maintained projects use SDK **8.0.424**, pinned in [global.json](../examples/code/global.json). Install the **SDK**, not just a runtime, and verify that `dotnet --list-sdks` lists it. Follow [Microsoft's installation instructions](https://learn.microsoft.com/en-us/dotnet/core/install/windows); a user-local installation can be passed to the build script with `-DotnetPath`.

Visual Studio, VS Code or another C# editor can use the same projects. For Visual Studio, install the .NET desktop development workload and use a version that supports the selected SDK.

The mod target is **`net48`**, matching the installed Harmony dependency. The .NET 8 SDK compiles it; it does not turn the mod into a .NET 8 Unity plugin. The pinned `Microsoft.NETFramework.ReferenceAssemblies` package supplies framework reference assemblies during restore.

## Local game references

Find the game through Steam's **Browse local files** or the storefront equivalent. `TerraInvictaDir` must point to the directory containing `TerraInvicta_Data/Managed/Assembly-CSharp.dll`.

Install [Unity Mod Manager](https://github.com/newman55/unity-mod-manager) for Terra Invicta and confirm its menu starts. The maintained baseline is **1.0.53a / Unity 2020.3.49f1 / UMM 0.33 / Harmony 2.3.6**. UMM and Harmony default to `TerraInvicta_Data/Managed/UnityModManager/`; use `UnityModManagerDir` if the chosen installation differs.

All references must come from the installation you will test. The shared project files set game, Unity and loader references to `Private=false`, so their DLLs do not enter your package. Do not add a separate Harmony package or commit copied game assemblies.

## Resolve setup failures

| Failure | Check |
| --- | --- |
| `dotnet` exists but no SDK is found | Install the SDK; inspect `dotnet --list-sdks` and the pinned version |
| Missing `Assembly-CSharp.dll` | Point `TerraInvictaDir` at the game root, not its `Managed` subdirectory |
| Missing UMM/Harmony reference | Verify the loader installation and `UnityModManagerDir` |
| NuGet restore fails | Check configured sources/network access; the first build needs the pinned reference package |
| Existing mod DLL cannot be replaced | Close the game before deploying the new build |
| A UI type is unresolved | Add its owning installed assembly with `Private=false`; see [UI references](IntroToUI.md) |

Continue with [build, install and smoke test](../examples/code/README.md). Rebuild and repeat the relevant feature tests after a game or loader update. AssetBundle authoring uses a separate [Unity asset workflow](../docs/assets.md).
