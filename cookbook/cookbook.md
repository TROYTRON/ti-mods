# Code modding cookbook

Each recipe links to a maintained project with a manifest and shared build configuration. Start with the [UMM/Harmony starter](../examples/code/README.md) if you have not built a mod yet.

| Task | Recipe | Source |
| --- | --- | --- |
| Change templates after loading | [Patching data templates](patching_data_templates_in_code/index.md) | [TiMods.TemplatePatch](../examples/code/TiMods.TemplatePatch/Main.cs) |
| Preserve dynamic campaign data | [Saving mod state](save_mod_state_to_save_file/index.md) | [TiMods.SaveState](../examples/code/TiMods.SaveState/Main.cs) |
| Add a debug-console command | [Console commands](add_console_command/index.md) | [TiMods.Console](../examples/code/TiMods.Console/Main.cs) |

The projects compile against stable **1.0.53a**, targeting **net48** with UMM **0.33** and bundled Harmony **2.3.6**. In-game feature and save tests remain necessary. For **1.0.57**, rebuild against that installation and follow the [update checklist](../docs/code-modding.md#updating-from-1053a-to-1057).

## Adding a recipe

Keep the explanation focused on the task, patch timing, state ownership and cleanup. Link a complete project rather than duplicating source in Markdown. Specify how to observe success, what disabling does, and which tests require a disposable campaign. Use local game references with Copy Local disabled; never include game or loader DLLs.

The template, persistence and console recipe concepts originated with dkoiman. Their maintained implementations and current build instructions are linked above.
