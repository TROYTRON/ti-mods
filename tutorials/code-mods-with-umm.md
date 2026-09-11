# Your first UMM code mod

Start with [TiMods.Starter](../examples/code/TiMods.Starter/Main.cs). It supplies a static UMM entry point, settings UI, enable/disable handling, owner-scoped Harmony cleanup, and a probe you can test from the main menu.

1. Prepare the [build environment](Build%20Environment.md).
2. Follow the [build and installation instructions](../examples/code/README.md#build).
3. Run the [starter smoke test](../examples/code/README.md#first-smoke-test).
4. Copy the project and shared build files for your own mod; rename its assembly, namespace, manifest ID and entry method together.
5. Use the [code-modding guide](../docs/code-modding.md) to inspect and patch the behavior you want to change.

UMM reads `ModFile.json` from `Mods/Enabled/<ModId>/` and invokes its `EntryMethod`. The starter's optional **PreciseProjects** setting patches `TICouncilorState.projectContributionString`: it replaces the engineering-project `+` display with a numeric monthly contribution, preserving the `"-"` case. It changes display text only and defaults to off.

The main-menu probe is the first test because it needs no campaign objects. After that works, inspect the actual councilor screen to test the game getter. The examples compile against stable **1.0.53a**; game behavior, settings persistence and patch interoperability still need in-game testing.

Next: [console commands](../cookbook/add_console_command/index.md), [template patches](../cookbook/patching_data_templates_in_code/index.md), [campaign state](../cookbook/save_mod_state_to_save_file/index.md), or [game-screen UI](IntroToUI.md).

References: [UMM authoring](https://github.com/newman55/unity-mod-manager/wiki/How-to-create-a-mod-for-unity-game), [Harmony](https://harmony.pardeike.net/articles/intro.html), [C# learning materials](https://learn.microsoft.com/en-us/dotnet/csharp/).

UMM also supports attribute-driven options: a settings class can implement `IDrawable`, annotate fields with `[Draw("Label")]`, supply `OnChange()`, and call `settings.Draw(modEntry)` from `OnGUI`. The starter uses explicit controls; see [UMM's options guide](https://github.com/newman55/unity-mod-manager/wiki/How-to-render-mod-options) for the alternative. For update notifications and distribution metadata such as `HomePage`, `Repository`, and `Requirements`, follow [UMM's information-file reference](https://github.com/newman55/unity-mod-manager/wiki/How-to-create-a-mod-for-unity-game#information-file), retaining Terra Invicta's `ModFile.json` filename.
