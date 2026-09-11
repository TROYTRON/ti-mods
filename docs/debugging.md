# Debugging a Terra Invicta mod

Reproduce the problem with the smallest mod set that still fails. Record the game version/branch, DLC, loader version, mod version and exact action. Keep development and Workshop copies from being enabled together.

## Capture the first useful failure

On Windows, collect:

```text
%USERPROFILE%/AppData/LocalLow/Pavonis Interactive/TerraInvicta/Player.log
```

Copy it before another launch replaces it; check timestamps on `Player-prev.log` too. For BepInEx, also collect `BepInEx/LogOutput.txt`. For UMM, use its log panel and the path reported by the installed manager.

Find the **first related exception**, full stack trace and preceding loader messages. Later null references may be consequences of an earlier missing template or dependency. State what you did, what you expected and what happened: “open the ship designer after loading this save” is a useful reproduction step.

## Find the failure stage

| Symptom | Next checks |
| --- | --- |
| JSON does not parse | Commas, quotes, brackets and field value types; run the repository validator |
| Mod is absent | Correct enabled folder and metadata filename; accidental wrapper folders |
| Mod is listed but has no effect | Enabled state, duplicate copies, exact template identity, patch target and timing |
| DLL load or patch exception | Matching dependencies, exact overload/private field, first inner exception |
| Campaign/UI fails after startup | Missing objects/assets, scenario-specific data, initialization order |
| A loaded save fails | Saved type identity, missing mod/DLC, schema migration and stale references |
| Bundle or bank is found but content is missing | Actual asset/event reference, dependencies, platform/import settings and consuming code |

Consult [native data](native-data.md), [code mods](code-modding.md), [assets](assets.md), or [audio](../tutorials/Audio%20Modding%20Guide.md) for the relevant loading rules. Compare deployed files with the intended package: copying over a directory does not remove obsolete files.

## Inspect installed code without modifying it

Open `TerraInvicta_Data/Managed/Assembly-CSharp.dll` and its matching dependencies in dnSpyEx or ILSpy; the [local code-inspection workflow](tools.md#inspect-the-installed-game-code) covers the installation path, scripted tools, and evidence limits. Follow the stack trace to the target and its callers. Check the declaring type, overload, accessibility, parameter types and return value before changing a patch. Use the installed assembly read-only and make changes in your mod project.

Decompiler output reconstructs C# from IL. Inspect IL when an iterator, lambda, unusual control flow or method-name lookup matters. A familiar filename does not mean that two game versions expose the same API.

Log through the loader's logger at meaningful branches in your own patch. Include enough state to identify the object; avoid per-frame spam. Keep the matching DLL/PDB pair when debugging a build. After an update, check [the 1.0.53a → 1.0.57 review points](code-modding.md#updating-from-1053a-to-1057).

### Notification keys and Harmony instrumentation

If adding a patch to `TINotificationQueueState.LogProjectTriggered` makes notifications disappear, inspect how the notification key reaches `InitItem`. A method can derive an identifier from its own name; patching that method can change the lookup even when your callback only logs. Check the first missing-template exception and compare with the patch disabled.

Prefer an observation point that does not derive its identity from its own method name. If you need a replacement prefix, confirm the intended key and preserve the current method's required behavior before skipping the original. The [resolved notification example](https://discord.com/channels/462769550841348126/780213497028018207/1523095754338598912) motivates this check; it does not imply that every postfix suppresses notifications.

## Explore live objects with UnityExplorer

Use [yukieiji/UnityExplorer](https://github.com/yukieiji/UnityExplorer) for live scene/component inspection. Select the package matching your **Mono** runtime and existing loader. A standalone build needs its dependencies and explicit `ExplorerStandalone.CreateInstance()` initialization; copying a standalone DLL into a UMM folder does not initialize it.

Open the relevant game screen, locate its root or a visible control, and record parent/child relationships, component types and active/inactive state. Inspect values before calling methods or setters, which may modify the campaign. The [UI workflow](../tutorials/IntroToUI.md#inspect-a-screen-before-patching-it) turns those observations into a patch plan.

For startup or input failures, inspect the inner exception, dependency loading and the tool's startup-delay/EventSystem settings. Follow the selected release's [troubleshooting instructions](https://github.com/yukieiji/UnityExplorer#common-issues-and-solutions) one change at a time.

### RuntimeUnityEditor alternative

[RuntimeUnityEditor](https://github.com/ManlyMarco/RuntimeUnityEditor#how-to-use) provides a dedicated UMM package and separate BepInEx builds; its documented opening key is **F12**. Use it when that package better matches your setup. The handbook has not tested either explorer in game.

## Optional development player for managed debugging

For breakpoints and locals, use a matching Unity development player in a **separate recoverable installation**. Protect user saves/profile files separately: a copied game directory can still use the same user-data locations. This setup is optional and has not been run for the handbook.

1. Obtain the **Unity 2020.3.49f1 Windows x64 Mono development player** for the 1.0.53a baseline. The Editor path is `Editor/Data/PlaybackEngines/windowsstandalonesupport/Variations/win64_development_mono`.
2. Back up the test installation's runtime files. Merge the matching development player's `Data` into `TerraInvicta_Data`; use its `WindowsPlayer.exe` renamed to `TerraInvicta.exe`, `UnityPlayer.dll` and required companion files such as `WinPixEventRuntime.dll`. Keep a list of replacements and do not mix player versions.
3. Enable the development player's managed debugging, including `player-connection-debug=1` in `TerraInvicta_Data/boot.config`, preserving other entries.
4. If using Doorstop, follow that installed version's configuration format. Older setups use `debug_enabled`, `debug_address=127.0.0.1:55555` and optionally `debug_suspend`; those keys are not universal. Keep local debugging on loopback.
5. Launch/connect through dnSpyEx's Unity debugging engine. If attaching fails, try launching through the debugger. Verify the loaded module and a breakpoint in the intended method.
6. Restore the test installation afterward and repeat the feature test with the ordinary release player.

See [Dienes's development-player notes](https://github.com/Dienes/ti-mods/blob/2197ef0/tutorials/Build%20Environment.md) and [dnSpy's Unity debugging instructions](https://github.com/dnSpy/dnSpy/wiki/Debugging-Unity-Games) for the underlying workflow. For another game version, first determine its actual Unity player version.

## Confirm the fix

Retest the original failure, enable/disable, a second screen opening and campaign reload as relevant. For persistent state, also restart the application and test a second campaign. Report exactly which steps passed; compilation or reaching the menu does not replace the feature test.
