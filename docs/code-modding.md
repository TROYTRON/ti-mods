# Code modding

Use **Unity Mod Manager (UMM) and its bundled Harmony** for the examples in this handbook. Start with [TiMods.Starter](../examples/code/TiMods.Starter/Main.cs), then follow the [build, install and smoke-test instructions](../examples/code/README.md). The projects compile for **stable 1.0.53a**, Unity **2020.3.49f1**, UMM **0.33** and Harmony **2.3.6**. In-game behavior and save round trips still need testing on your installation; **1.0.57** requires a fresh build and feature test.

## Choose the right extension

| Change | Starting point |
| --- | --- |
| Static balance or content data | [Native JSON](native-data.md) |
| Method behavior or returned display text | [Harmony starter](../examples/code/TiMods.Starter/Main.cs) |
| User preferences | Starter's `ModSettings`, `OnGUI` and `OnSaveGUI` |
| Console command | [Console recipe](../cookbook/add_console_command/index.md) |
| Programmatic template edits | [Template recipe](../cookbook/patching_data_templates_in_code/index.md) |
| New campaign data | [Save-state recipe](../cookbook/save_mod_state_to_save_file/index.md) |
| A new game-screen control | [Unity UI guide](../tutorials/IntroToUI.md) |

BepInEx plugins and MonoMod patch assemblies have different entry points and deployment rules. Use the [alternate-loader guide](../tutorials/MonoMod%20Guide.md) when maintaining one of those mods; a UMM DLL is not automatically a BepInEx plugin.

## Inspect the target before patching

Open your installed `TerraInvicta_Data/Managed/Assembly-CSharp.dll` read-only in [dnSpyEx or ILSpy](tools.md). Find the declaring type, exact overload, parameter and return types, visibility, and callers. Some template and UI types are in the global namespace; many game-state types are in `PavonisInteractive.TerraInvicta`.

Check **when** the target runs. `Load` can precede campaign creation, while some templates and UI controllers can predate the mod. Avoid retaining scene or campaign objects across a load. A string-named Harmony target or private field can compile even when the target no longer exists.

Prefer the game's existing operation/update methods when changing state. For example, investigate fleet orbit/transfer operations such as `AssumeOrbit()` instead of assigning a convenience reference such as `ref_orbit`: movement can require related state changes. Inspect the actual overload and callers before using it.

## Manage the patch lifecycle

The starter loads settings first, registers UMM callbacks, then patches on enable. Its main-menu probe displays `patched` or `original`; the optional councilor patch replaces the project-contribution display with a number without changing income.

Use a unique Harmony ID, normally `modEntry.Info.Id`. On disable or unload, remove only your own patches:

```csharp
harmony.UnpatchAll(modEntry.Info.Id);
```

The parameterless form can remove other mods' patches. Unpatching stops future interceptions; it does not undo values already written into templates, cached objects or saves. Return success from `OnToggle` only after the requested transition succeeds, and log failures through `modEntry.Logger`.

## Replace only the cases you handle

A postfix can adjust the original result. A **boolean prefix** can instead supply the result and skip the original for selected cases: return `false` after assigning `__result`, or `true` to preserve the original path. Other patches may still affect the call, so keep the replacement narrow. See [Harmony prefixes](https://harmony.pardeike.net/articles/patching-prefix.html).

This illustrative example targets a method owned by the mod, so it needs no campaign or game API. It is separate from the maintained starter source:

```csharp
using System.Runtime.CompilerServices;
using HarmonyLib;

internal static class PreviewLabel
{
    internal static bool UseShortLabel;

    [MethodImpl(MethodImplOptions.NoInlining)]
    internal static string GetText() => "Full diagnostic label";
}

[HarmonyPatch(typeof(PreviewLabel), nameof(PreviewLabel.GetText))]
internal static class PreviewLabelPatch
{
    private static bool Prefix(ref string __result)
    {
        if (!PreviewLabel.UseShortLabel)
            return true;

        __result = "Short";
        return false;
    }
}
```

Apply it through the loader's normal `PatchAll` setup. With the flag false, `GetText()` returns the original text; with it true, the prefix supplies `Short`. `NoInlining` keeps this small demonstration target available for patching. The conditional-replacement lesson comes from [NotSoLoneWolf's IncomePerTurn tutorial](https://github.com/TROYTRON/ti-mods/pull/1); this example is independently written.

Do not copy an entire decompiled game method just to change one value. That preserves old branches and can discard later game fixes. Also inspect targets that derive keys from their own method names: even logging patches can disturb them. See [notification instrumentation](debugging.md#notification-keys-and-harmony-instrumentation).

## Keep data in the right place

| Data | Storage |
| --- | --- |
| A player's preferences | `UnityModManager.ModSettings`; separate from campaign saves |
| Static configuration | Templates, usually patched through native JSON |
| Dynamic campaign data | A registered `TIGameState` subtype |
| Temporary lookup/index | An in-memory cache rebuilt for the current campaign |

Give saved types stable names and a schema version. Plan migrations before changing their shape. Use `GameStateID` to associate extra state with an existing object, and resolve it again after loading; the [save recipe](../cookbook/save_mod_state_to_save_file/index.md) covers indexing, removal and missing-mod tests.

The [template and state API reference](template-and-state-apis.md) covers template inheritance, localized names, `Find`, `IterateByClass`, `GetAllTemplates`, `GetMyTemplate`, and the state initialization stages.

## Updating from 1.0.53a to 1.0.57

Rebuild against the target installation, then retest the behavior affected by your patches. These intervening changes deserve particular attention:

- **1.0.54:** cache changes around army reachability, faction hab lists, councilor maximum stats and tech connections. Trace the current invalidation/update path when mutating their inputs. Also retest new template names and DLC scenarios.
- **1.0.55:** the `CartesianState.ChangeReferenceFrame` optimization was reversed; recheck trajectory patches. Revisit trade and AI mutation timing around prompts.
- **1.0.57:** the full-tech-tree crash introduced in 1.0.54 was fixed, and AI repeatable-project selection changed. Test large tech trees and repeatable projects against the target build.

See [compatibility](compatibility.md) for release details. Check both an existing disposable save and a new campaign in each supported scenario. A successful compile cannot establish patch timing, UI behavior, saved-type compatibility or interactions with another mod.

For release, test enable/disable, restart, the actual feature and any required save/reload cycle. Package only your mod DLL and required metadata/assets; use the [example packaging instructions](../examples/code/README.md#package-and-install-locally).
