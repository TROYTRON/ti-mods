# Build and test the C# examples

The five mod projects compile against **Terra Invicta stable 1.0.53a**, Unity **2020.3.49f1**, UMM **0.33** and bundled Harmony **2.3.6**. They target **`net48`** to match the Harmony dependency and use SDK **8.0.424**. In-game feature and save tests remain necessary; rebuild against **1.0.57** before testing that version.

| Project | What it demonstrates |
| --- | --- |
| [TiMods.Starter](TiMods.Starter/Main.cs) | UMM settings, toggle, owner-scoped Harmony cleanup, main-menu probe and optional numeric councilor project display |
| [TiMods.Console](TiMods.Console/Main.cs) | `timods_echo`, argument validation, registration and cleanup |
| [TiMods.TemplatePatch](TiMods.TemplatePatch/Main.cs) | Change `PointDefenseLaserTurret.targetingRange_km` to `1337`; restart required to disable |
| [TiMods.SaveState](TiMods.SaveState/Main.cs) | Explicitly create, increment and remove a campaign counter |
| [TiMods.Diagnostics](TiMods.Diagnostics/Main.cs) | Log effective template/localization values and faction state |

## Build

Install the SDK in [global.json](global.json); see [environment setup](../../tutorials/Build%20Environment.md) if needed. From the repository root in PowerShell:

```powershell
$env:TerraInvictaDir = 'D:\SteamLibrary\steamapps\common\Terra Invicta'
./examples/code/build.ps1
```

Use your actual game directory, containing `TerraInvicta_Data/Managed/Assembly-CSharp.dll`. The script builds all five mods and prints their package paths. It does not deploy files or launch the game.

For a single project, run inside `examples/code`:

```powershell
dotnet build ./TiMods.Starter/TiMods.Starter.csproj -c Release "-p:TerraInvictaDir=$env:TerraInvictaDir"
```

| Override | Use |
| --- | --- |
| `-DotnetPath 'D:\Tools\dotnet\dotnet.exe'` | Select a user-local SDK when running `build.ps1` |
| `-UnityModManagerDir 'D:\Path\To\UnityModManager'` | Override the default `TerraInvicta_Data/Managed/UnityModManager` reference directory |
| `-Configuration Debug` | Build with the script's Debug configuration instead of Release |

`TerraInvictaDir` and `UnityModManagerDir` also work as environment variables or MSBuild properties. The shared [props](Directory.Build.props) and [targets](Directory.Build.targets) keep game, Unity and loader references `Private=false`. Restore uses the pinned `Microsoft.NETFramework.ReferenceAssemblies` **1.0.3** package; the first restore needs NuGet access. No game DLLs are supplied or downloaded.

When starting your own project, copy the shared build files too. Rename the project/assembly, namespace, manifest ID and entry method together. Keep local absolute paths, build output and game DLLs out of Git.

## Package and install locally

A Release build stages:

```text
TiMods.Starter/bin/Release/net48/package/TiMods.Starter/
  ModFile.json
  TiMods.Starter.dll
```

With the game closed, copy that **inner `TiMods.Starter` folder** to **`<TerraInvictaDir>/Mods/Enabled/TiMods.Starter/`**, or zip it for UMM installation. The loader scans `Mods/Enabled` and uses **`ModFile.json`**; native data metadata is the separate `ModInfo.json` format.

Keep the DLL and manifest at the mod folder root. Distribute your mod and its required authored assets, not Unity/game/UMM/Harmony/framework DLLs or an entire build directory. Restart after replacing an assembly.

## First smoke test

1. Start with `TiMods.Starter` and remain at the main menu. Open UMM and enable the mod.
2. Check `Enabled=True; Probe: patched` in the log and `Harmony probe: patched` in its options.
3. Clear **Enable probe postfix**: the label becomes `original`. Restore it: the label becomes `patched`. **Write probe to log** captures either result.
4. Save settings in UMM, restart and check the persisted checkbox. Settings live in the mod folder, separately from campaign saves.
5. Disable the mod: expect `Enabled=False; Probe: original`. Re-enable and check that the patch works again.
6. For the optional numeric-projects setting, load a disposable campaign and inspect a councilor's engineering contribution. The setting changes display text only and defaults to off.

Close the game and remove the starter folder to uninstall. A restart releases its loaded assembly.

## Other examples

- **Console:** follow the [command recipe](../../cookbook/add_console_command/index.md), including bare-input validation and toggle cleanup.
- **Template patch:** follow the [template recipe](../../cookbook/patching_data_templates_in_code/index.md). This changes gameplay data; disable and restart before ordinary play.
- **Save state:** follow the [complete save/reload and removal test](../../cookbook/save_mod_state_to_save_file/index.md) with a disposable campaign. Disabling the mod does not remove saved data.
- **Diagnostics:** enable the [native org example](../../tutorials/tutorial-files/template-json-mod-examples/org-example/) and select **Log probes**. Defaults target `Handbook_OpenResearchGroup` and `TIOrgTemplate.displayName.Handbook_OpenResearchGroup`; change them for your fixtures. The log also shows global delay/speed settings, `ModernOrgTemplates`, active player and factions. An unavailable probe is a reason to inspect loading, not a passed test.

## Standalone native-merge checks

[TiMods.MergeChecks](TiMods.MergeChecks/Program.cs) is a **.NET 8 command-line harness**, not a game mod. It calls the installed `JsonController.CombineJson` using valid in-memory fixtures and resolves game DLLs in place. Do not install it into `Mods/Enabled`.

```powershell
./examples/code/test-native-merge.ps1 -TerraInvictaDir $env:TerraInvictaDir
```

Use `-DotnetPath` for a local SDK if needed. Nine assertions passed against the 1.0.53a helper: default array merge, omitted properties, concat, array replacement, null handling, `dataName` matching, addition/suppression of new names and property-name comparison. This tests the managed helper under .NET 8, not Unity mod discovery or gameplay. Malformed-input cases are excluded because the helper's exception path invokes Unity logging.

## Checks before release

Run these game-independent checks from the repository root:

```sh
python scripts/validate.py
python -m unittest discover -s tests
```

Build against your own game installation and complete the relevant feature tests above. Public CI cannot compile or run game-dependent checks without local proprietary references; skip those checks explicitly when the files are absent. Repeat them after game or loader updates, using the [code update checklist](../../docs/code-modding.md#updating-from-1053a-to-1057).
