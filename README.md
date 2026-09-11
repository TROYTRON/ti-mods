# Terra Invicta Modding Handbook

Practical guides and source examples for creating Terra Invicta data mods, code mods, scenarios, and assets.

Start with the guide for your task, and check [compatibility](docs/compatibility.md) for the recorded game/tool versions, testing limits, and update notes.

## Start here

1. [Find your installation, logs, and saves](docs/getting-started.md).
2. [Create a native JSON mod](tutorials/Create_Template_JSON_mod.md), then learn [loading, arrays, and scenarios](docs/native-data.md).
3. For behavior changes, [set up UMM/Harmony](tutorials/code-mods-with-umm.md) and build the [C# examples](examples/code/README.md).
4. Test in a disposable campaign, then [package and publish your mod](tutorials/Uploading%20and%20Updating%20Workshop%20Mod.md).

## Find a capability

| Goal | Guide |
|---|---|
| Change values, add records, control arrays and conflicts | [Native data](docs/native-data.md) |
| Configure fields absent from vanilla JSON | [GlobalConfig](docs/global-config.md) |
| Add organizations, flags, technology, events, or localization | [Content authoring](docs/content-authoring.md), [organizations](tutorials/Custom%20Orgs.md) |
| Target base-game or Dark Skies scenarios | [Scenarios](docs/native-data.md) |
| Build asset bundles or work with ships | [Assets](docs/assets.md) |
| Make councilor portraits and badges | [Portraits](tutorials/Councillor%20Portraits.md) |
| Edit maps and region geometry | [Maps](tutorials/MapCreation.md) |
| Add audio | [Audio](tutorials/Audio%20Modding%20Guide.md) |
| Patch code, add settings, or persist state | [Code modding](docs/code-modding.md), [recipes](cookbook/cookbook.md) |
| Look up templates and understand campaign state | [Template and state APIs](docs/template-and-state-apis.md) |
| Modify a game screen | [Unity UI](tutorials/IntroToUI.md) |
| Work with BepInEx/MonoMod | [Alternative loaders](tutorials/MonoMod%20Guide.md) |
| Diagnose a crash or conflict | [Debugging](docs/debugging.md) |
| Choose a compiler, decompiler, or editor | [Tools](docs/tools.md) |

## Resources

- **[Terra Invicta AI mod template](https://github.com/Laurentiu-Andronache/ti-mod-template)**: Windows scaffolding for creating mods with AI agents, including agent instructions, MCP, ILSpy, and build, test, and packaging workflows.
- **TerraInvictaMCP**: tools for interacting with the game during mod development. Compare [Laurentiu-Andronache's fork](https://github.com/Laurentiu-Andronache/TerraInvictaMCP) and [MeatBunny's upstream](https://github.com/MeatBunny/TerraInvictaMCP), and use whichever was updated most recently. Follow the chosen repository's setup instructions.
- [TIShipModdingFramework by UNNRazorback](https://github.com/UNNRazorback/TIShipModdingFramework): custom ship support; follow the project's installation and version requirements.
- [MonoMod adaptation by Tayta](tutorials/tutorial-files/TIShipModdingFramework_MonoMod.cs): source for existing MonoMod ship projects; see [adaptation notes](docs/assets.md#inspect-assets-and-ship-examples).
- [Narrative event reference](https://docs.google.com/document/d/1s3x96SyjvKFwx3pRSaMS7Zjo3FLwVVSLzSWzidT4CEo/edit): use alongside your installed templates and field signatures.
- [Sarah's navigable tech tree](https://sarahwatt.ca/terra-invicta/techtree/?lang=en): browse prerequisites and unlocks; check its data version against your game.
- [Tutorial resources](tutorials/tutorial-files/tutorial-files.md): bundle helper, badge templates, and JSON examples.

## Modding Wishlist

- Native support for DLL patching, similar to how Rimworld does it.
  - This would alleviate the need for players to download and install third party patching tools (like Unity Mod Manager or BepInEx/MonoMod) to be able to play code mods.
