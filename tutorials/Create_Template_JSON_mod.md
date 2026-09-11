# Creating a native template / JSON mod

This tutorial changes the initial owner of Alaska to Canada in a new **2022 campaign**. It needs no DLL, Unity project, or Unity Mod Manager. It targets **Terra Invicta 1.0.53a**; see [version notes](../docs/compatibility.md) for other builds and the [native data reference](../docs/native-data.md) for merge rules and troubleshooting.

## Template structure

The installed game's `TerraInvicta_Data/StreamingAssets/Templates` directory contains the base template files. A file such as `TIBilateralTemplate.json` describes instances of the corresponding game template type. Each file contains a JSON array, including files with just one object.

`dataName` is the identifier the native JSON merger uses to match records. An existing record needs only that identifier and the fields you want to change. A new identifier needs enough fields to construct a valid new template. Referenced identifiers, enum values, property names, and filenames should retain their exact spelling and capitalization.

Keep the installed templates as reference material. Develop your files in a separate mod folder. A sparse patch inherits untouched values from the record being merged; it does not reset them to C# constructor values.

## Setting up the mod folder

Create this layout under the game installation:

```text
Mods/
  Disabled/
    Example Mod/
      ModInfo.json
      TIBilateralTemplate.json
```

Use a mod directory whose name matches `Title`. Keep `ModInfo.json` beside the JSON files: the loader looks for metadata in each template file's immediate directory, even though it discovers mod files recursively.

Save this complete, valid [ModInfo.json](tutorial-files/template-json-mod-examples/ModInfo.json):

```json
{
  "Title": "Example Mod",
  "Author": "Your name",
  "Description": "Starts the 2022 scenario with Alaska owned by Canada; preserves the US claim.",
  "LoadOrder": 0
}
```

`LoadOrder` and array policies belong in `ModInfo.json`. A native data mod does not need `AssemblyName` or `EntryMethod`; those belong to the separate [code mod workflow](code-mods-with-umm.md).

## Creating the JSON mod file

In the base `TIBilateralTemplate.json`, Alaska's initial ownership is represented by:

```json
[
  {
    "dataName": "ClaimUSAAlaska",
    "relationType": "Claim",
    "nation1": "USA",
    "region1": "Alaska",
    "initialOwner": true
  }
]
```

Our patch makes that claim cease to be the initial ownership claim. It adds a new claim whose initial owner is Canada. Save this as [TIBilateralTemplate.json](tutorial-files/template-json-mod-examples/TIBilateralTemplate.json):

```json
[
  {
    "dataName": "ClaimUSAAlaska",
    "initialOwner": false
  },
  {
    "dataName": "ExampleMod_ClaimCANAlaska",
    "relationType": "Claim",
    "nation1": "CAN",
    "region1": "Alaska",
    "initialOwner": true
  }
]
```

There are no trailing commas, comments, placeholder records, or copied unrelated claims in these example files. The `ExampleMod_` prefix reduces the risk of colliding with another author's new identifier.

This changes campaign initialization data. It does not order the transfer of Alaska in a campaign that has already created its nation and region states. Other start dates can use different region/nation records and scenario variants; test those separately if you intend to support them.

## Testing the mod

1. Validate both files before installation. For example, run `python -m json.tool ModInfo.json` and `python -m json.tool TIBilateralTemplate.json` from your mod's working directory. Validate the metadata again whenever you add array policies; a malformed concat list can cause symptoms that look like missing content.
2. Open the game's Mods menu, enable `Example Mod`, and enable **Use Mods**. Restart the game after changing the installed files or enabled set.
3. Check the log for the mod's filename and for JSON parsing, unknown-template, and missing-reference errors. A successful merge message confirms discovery and merging, not the final campaign result.
4. Start a new 2022 campaign. Check that Alaska belongs to Canada and that the US claim still exists.
5. Save, quit, and reload that disposable campaign. Check the same facts and the logs again.
6. Test with other enabled mods that touch bilateral relations or the same scenario. Record the game build, scenario, DLC, and enabled mod list with your results.

The loader merges JSON **in memory**. It does not overwrite the vanilla template or create a backup file. An unchanged file under `StreamingAssets/Templates` is expected; check the game result and logs.

## Upload your mod

Package just your mod directory and its required files. Follow the [Workshop upload and update guide](Uploading%20and%20Updating%20Workshop%20Mod.md). State the tested game build and start scenario, describe the two modified claim identifiers, and say that this example requires a new campaign to demonstrate its intended behavior.

## Common problems

| Symptom | First thing to inspect |
| --- | --- |
| Nothing changes in an existing save | Test a new campaign; initial ownership is not a live transfer instruction. |
| Mod appears, but arrays do surprising things | Default array handling merges by index. Choose an explicit [array policy](../docs/native-data.md#array-policies) where necessary. |
| Adding concat produces a crash or missing names/flags | Validate the metadata syntax, then check for duplicate starting-content entries in appended arrays. See [concat troubleshooting](../docs/native-data.md#diagnosing-concatenation-and-replacement-conflicts). |
| A nested template ignores `LoadOrder` | Put its `ModInfo.json` in the same immediate directory. |
| The log cannot find a template type | Preserve the exact supported template filename. A filename does not define a new C# type. |
| A patch appears to work only in one scenario | Follow `TIMetaTemplate.json` to the actual region/nation IDs, including the DLC's separate template directory. Check tags and variants; a prefix used by one scenario is not a rule for every start. |
| The vanilla file does not change | Expected for the current in-memory merge path. |

Use a supported template filename: it must resolve to a game template type. Keep unrelated `.json` settings, backup files, and alternative examples outside the installed mod directory.

Continue with [global configuration](../docs/global-config.md), [content authoring](../docs/content-authoring.md), or [custom organizations](Custom%20Orgs.md).
