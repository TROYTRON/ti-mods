# BepInEx and MonoMod

Use this route when maintaining a mod that already requires it. New handbook examples use [UMM/Harmony](../docs/code-modding.md). BepInEx plugins and MonoMod patch assemblies have different entry points and loading rules; neither is installed by treating it as a UMM DLL.

The following mechanisms are useful for existing projects, but this handbook has not tested a BepInEx/MonoMod combination on **1.0.53a or 1.0.57**. Match the chosen mod's loader versions before changing the installation.

## Set up the required loader

1. Select a **Windows x64 Mono** BepInEx package required by your mod. [BepInEx 5.4.23.5](https://github.com/BepInEx/BepInEx/releases/tag/v5.4.23.5) is a 5.x reference; 6.x has a different package/dependency matrix. Follow the selected release's installation instructions.
2. Use a recoverable test installation, retaining existing proxy/configuration files. Start it once and confirm BepInEx initialization and its log.
3. Put ordinary BepInEx plugins and their declared dependencies in the layout specified by the plugin, normally under `BepInEx/plugins`. Plugins can have supporting files and subdirectories.
4. If the mod requires assembly patching, install its compatible MonoMod loader. The [BepInEx.MonoMod.Loader 1.0.0.0](https://github.com/BepInEx/BepInEx.MonoMod.Loader/releases/tag/v1.0.0.0) route uses `BepInEx/monomod`; it is a legacy loader, not a universal companion for newer BepInEx releases.

A BepInEx plugin commonly derives from `BaseUnityPlugin`, identifies itself with `[BepInPlugin]`, and initializes in `Awake`. `[BepInProcess]` can restrict the target executable. UMM instead invokes the static method in `ModFile.json`. Keep those bootstraps separate.

## Build and map an assembly patch

Reference the target game's assemblies and the exact loader/patcher dependencies from the chosen installation, with Copy Local disabled. Keep those DLLs outside the distributable. The conventional patch assembly name is `Assembly-CSharp.<ModName>.mm.dll` for changes to `Assembly-CSharp.dll`.

| Mechanism | Purpose | Check before use |
| --- | --- | --- |
| `patch_TargetType : TargetType` | Describes members mapped onto an existing type | Exact namespace, type name and patcher conventions |
| `orig_Method` declared `extern` | Calls the preserved original from the replacement method | Matching signature and modifiers; avoid unintentionally skipping required behavior |
| `get_Property` / `set_Property` | Targets property accessor methods | Actual accessor signature, visibility and side effects |
| `[MonoModIgnore]` | Gives the compiler a declaration that the patcher should map rather than add | Existing member name/type; the attribute is interpreted by the patcher |
| `[MonoModPublic]` | Changes access to a member when deliberately required | Scope of the resulting API change and other callers |
| `[MonoModConstructor]` / original-constructor mapping | Adds initialization while retaining original construction | Constructor execution and separate save-deserialization lifecycle |

These conventions belong to the selected assembly patcher. Modern [MonoMod](https://github.com/MonoMod/MonoMod) also contains runtime-detour libraries; a similarly named package is not necessarily the same workflow.

For concrete C# examples of these conventions, see [method, private-member, enum, and constructor patches](MonoMod%20Examples.md). Those examples retain the useful mechanics from Tayta/TROYTRON's tutorial; validate the target signatures and serialization behavior before using them in a current mod.

## Adding fields and enum values

Adding a declaration is only part of a feature. Initialize new fields on every relevant construction/load path. A C# field initializer lives in constructor code, so adding a field alone does not ensure the original class executes it. A serialized field also needs a stable identity and migration plan.

For enum additions, match the actual underlying type and choose explicit values that do not collide with the game or other mods. Audit switches, array indexing, icons/localization, serialization and deserialization. Replacing an entire enum or globally changing FullSerializer behavior is usually too broad for one feature; first reproduce the specific missing case and fix it narrowly.

Compile, confirm the patch was applied, then test the changed behavior and a disposable save/load cycle. Restore and retest with the ordinary release player before publishing.

## Combining PVC with UMM

For the Project Valkyrie Chronicles setup, [Tayta's coexistence recipe](https://discord.com/channels/462769550841348126/780213497028018207/1542103083365048420) uses this sequence:

1. Confirm PVC works in the test installation.
2. Temporarily set aside its `doorstop_config.ini` and `winhttp.dll`.
3. Install UMM using the **Assembly** method.
4. Restore the PVC files and place UMM mods in `Mods/Enabled`.

The loaders still operate separately, and individual mods can conflict. Recheck this setup after game updates; the original instructions say to repeat installation after updates. This is a PVC-specific option, not the default installation method for the handbook starter.

Based on Tayta/TROYTRON's MonoMod tutorial and NotSoLoneWolf's BepInEx introduction. Keep the [upstream patcher documentation](https://github.com/MonoMod/MonoMod) and [debugging guide](../docs/debugging.md) at hand when adapting an existing patch.
