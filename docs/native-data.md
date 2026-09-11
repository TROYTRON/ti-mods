# Native data mods: files, merging, and scenarios

Use a native data mod when the behavior you want is already described by a game template, localization entry, or supported resource reference. Start with the [two-file JSON tutorial](../tutorials/Create_Template_JSON_mod.md), then use this page to choose the right merge policy and campaign scope.

This guide targets Windows **1.0.53a** with Dark Skies. Check the [version notes](compatibility.md) when targeting **1.0.57**; test your selected scenario and mod combination before release.

## Where the data lives

Paths below are relative to the Terra Invicta installation.

| Location | Purpose |
| --- | --- |
| `TerraInvicta_Data/StreamingAssets/Templates/*.json` | Base template definitions and `TIGlobalConfig.json` |
| `TerraInvicta_Data/StreamingAssets/Localization/<language>` | Base text, typically `key=value` files with the language extension |
| `TerraInvicta_Data/StreamingAssets/Namelists` | Shipped name-list references |
| `DLC_Content/DarkSkies` | Installed DLC assets, text, and scenario template variants |
| `Mods/Disabled/<Title>` | A local mod before enabling it |
| `Mods/Enabled/<Title>` | Native discovery location for enabled mods |

Copy the records you need into your working mod; do not develop by overwriting the installed template directory. Keep your authoring project, alternative examples, and backups outside the deployed mod. The current loader recursively enumerates files under each enabled mod directory.

The simplest deployment has `ModInfo.json`, template JSON, and localization files directly in `<Title>`. Nested files can be discovered, but **JSON settings are read from the template file's immediate containing directory**. A root `ModInfo.json` is not a reliable inherited configuration for a JSON file under `Templates/`. Keep the flat layout unless you have a specific reason and have tested the nested layout.

## Metadata and load order

This complete metadata example appends meta-template arrays and replaces tech arrays:

```json
{
  "Title": "My Content Mod",
  "Author": "Your name",
  "Description": "Adds content and adjusts selected prerequisites.",
  "LoadOrder": 10,
  "TemplatesToConcatArrays": [
    "TIMetaTemplate.json"
  ],
  "TemplatesToReplaceArrays": [
    "TITechTemplate.json"
  ]
}
```

Keep the folder name equal to `Title` for the native menu workflow. `Title`, `Author`, `ModURL`, and `Description` supply menu information. The same `ModInfo.json` also supplies these native JSON merge settings:

| Setting | Type and behavior |
| --- | --- |
| `LoadOrder` | Integer; default `0`. Template patches are sorted in ascending order, so a higher number is applied later. |
| `TemplatesToConcatArrays` | List of exact filenames whose arrays should append. |
| `TemplatesToReplaceArrays` | List of exact filenames whose arrays should be replaced by the supplied arrays. |
| `TemplatesToReplace` | List of exact filenames whose entire working JSON record list should be replaced by this mod's file. |

Include the `.json` suffix and exact case: `TIMetaTemplate.json`, not `TIMetaTemplate` or `timetaTemplate.json`. These settings apply at the **file level**, including nested arrays, not to named individual properties. If a filename is in both array lists, replacement wins. Use just the intended list.

Choose explicit distinct orders when two mods must overwrite the same field. Equal-order precedence has not been established; do not rely on alphabetical folder names as a compatibility contract. Load order can resolve which value wins; it cannot make conflicting redesigns logically compatible. This JSON ordering does not establish ordering for localization, DLLs, or asset bundles.

Native data mods do not need Unity Mod Manager. If a package also contains managed code, see [the UMM guide](../tutorials/code-mods-with-umm.md) for its separate entry point and metadata requirements. Adding a dummy DLL solely to silence a code-loader warning is unnecessary for a data-only package.

## How a sparse patch merges

Template files have an outer array of objects. For each patch record, the native merger looks for a matching **`dataName`**, with exact string matching. For ordinary base template processing, a new name appends a new record; a matching name merges into the existing object.

```json
[
  {
    "dataName": "AdAstra",
    "researchCost": 2500
  }
]
```

This patch to `TITechTemplate.json` changes the specified field and preserves the technology's prerequisites, effects, and other existing properties. Reusing a new record's `dataName` accidentally means editing that record, not creating a second one. Use an author/mod prefix for your additions.

Keep patches sparse to avoid restoring stale values over other mods' edits or later game updates.

Property names are case-sensitive, and object merging ignores incoming nulls:

- Omit properties you do not want to change.
- Use `false` or `0` when those are the intended typed values.
- Do not use `null` as a general delete/reset instruction.
- Use an empty string only for a field whose game semantics accept it.
- A new record must satisfy its actual type's validation and references; syntax alone cannot prove this.

The current loader builds the merged representation **in memory**, deserializes it, and registers templates. It does not rewrite vanilla JSON or create `.bak` files during this path. Fields omitted from an existing-record patch retain the working record's values; fields omitted from a genuinely new record depend on construction/deserialization and later game processing. See [global config](global-config.md) for why a constructor assignment is not the same thing as the effective campaign value.

## Array policies

The game's default is Json.NET **Merge**, which combines array entries by index. This differs from its special `dataName` matching for the outer template record list. Arrays inside a record do not automatically match their members by name or ID. The enum meanings are also documented in the [Json.NET API reference](https://www.newtonsoft.com/json/help/html/T_Newtonsoft_Json_Linq_MergeArrayHandling.htm).

For a starting scalar array `["A", "B"]` and supplied array `["C"]`:

| Policy | Result | Typical use |
| --- | --- | --- |
| Default Merge | `["C", "B"]` | Deliberately change an element at a known index |
| `TemplatesToConcatArrays` | `["A", "B", "C"]` | Add an org to a scenario list without copying the existing list |
| `TemplatesToReplaceArrays` | `["C"]` | Provide the complete desired prerequisites/options/list |

Under the default, `[ ]` does not clear an existing array. With replacement selected, an explicit empty array does clear it. Concatenation appends duplicates too; the native settings expose no Union option. For arrays of objects, index merging can leave fields from the old object mixed into the new one. It is especially hazardous for narrative conditions and event options.

For example, appending to `ModernOrgTemplates` uses metadata containing `"TemplatesToConcatArrays": ["TIMetaTemplate.json"]`, then this `TIMetaTemplate.json` patch:

```json
[
  {
    "dataName": "ModernOrgTemplates",
    "templateNames": [
      "Handbook_OpenResearchGroup"
    ]
  }
]
```

The complete associated org is in the [org example](../tutorials/Custom%20Orgs.md). The metadata applies concatenation to every supplied array in this template file, so review additional records before combining unrelated changes into the same mod.

`TemplatesToReplace` is a stronger operation: the mod's complete file becomes the working record list at that step. Sparse files used this way remove all the other records from that working list. Later mods can still modify the resulting data. Reserve full-file replacement for a deliberately maintained replacement dataset, document the conflict surface, and rebase it when game data changes.

### Diagnosing concatenation and replacement conflicts

| Symptom | Check |
| --- | --- |
| A combination loses names, flags, or other content | Parse `ModInfo.json` and every template file first. A missing comma around an array policy can obscure the intended setup. Then check localization keys and resource references. |
| Duplicate-key exception during campaign creation | Append only new list entries. Copying existing starting stations or other starting content into a concatenated meta list can create duplicates. Check the key named in the log. |
| Another mod's portraits disappear | Check for `TemplatesToReplace` on `TICouncilorAppearanceTemplate.json`. A later whole-file replacement discards earlier additions. Prefer sparse edits or agree explicit load orders for a maintained replacement dataset. |
| Array policy has no effect on localization | Remove text files from the JSON policy lists; use the separate [localization loader and key format](content-authoring.md#localization-files). |

A duplicate nested starting-content entry is different from intentional top-level `dataName` matching. Do not remove the `dataName` needed to identify an existing-record patch.

## Scenarios and DLC

A scenario is a network of templates. `TIMetaTemplate` groups references using `templateType` and `templateNames`; groups can contain other meta-template groups. The 2022 scenario root is `ModernScenario`, which references `ModernStartTime`, `ModernRegions`, `ModernNations`, `ModernArmies`, `StandardCouncilors`, `ModernStartFleets`, `ModernOrgTemplates`, `ModernHabs`, and `StandardRepeatingEvents`. `ModernStartTime` then selects `TIStartTimeTemplate` record `ModernDayStart`.

The 2026 and 2070 scenarios select their own start, nation, region, and other groups. Dark Skies installs `2003_Scenario` and `Broken_Earth_Scenario` data, among other content. Having a base record with a familiar name does not prove it is the active record in every campaign.

To locate a scenario's nation or region, follow its root through `TIMetaTemplate.json` to the selected records. Search the separate `DLC_Content/DarkSkies` directory for DLC templates. Follow exact references instead of generating names from a presumed scenario prefix.

Current scenario authoring uses these concepts:

| Member / record | What to trace |
| --- | --- |
| `TIMetaTemplate.isNewCampaignOption` and `newCampaignOptionCategory` | Whether a root is offered as a campaign choice; shipped scenario roots use category `Scenario`. |
| `templateType`, `templateNames` | The reference graph that creates selected campaign game states. |
| `requiredDLC` | Required DLC IDs on the scenario/meta choice; the installed DLC uses `DarkSkies`. |
| `scenarioTags` | Tags used by `ResolveTaggedTemplates` and `ResolveDuplicateTemplates` to filter/select variants. Untagged and scenario-specific records must be considered together. |
| `dataName` | JSON merge identity; it remains important before scenario resolution. |
| `referenceAlias`, `localizationAlias` | Separate reference/localization names exposed by `TIDataTemplate`; they fall back to `dataName` when empty. They are not a general JSON “inherit from this record” directive. |
| `scenarioLocalizationPostfix`, `templatesToUseDefaultLocalization` | Scenario text-selection settings; follow the relevant current scenario's lookup behavior and files. |
| `TIStartTimeTemplate` | Date, starting resources/research, and other campaign initialization settings. `techTreeUIStarters` is a field here in 1.0.53a. |

The tag filter requires a template's tags to be present in the chosen scenario's tags; tags are not simply an OR-list. Duplicate template resolution follows this filtering. Do not assume the native JSON merger is scenario-aware just because the final template resolver is.

`scenarioPrefix` remains in some shipped JSON but is absent from the current `TIMetaTemplate` type. Use the supported members above and follow their callers when adapting a scenario.

During processing of a file whose path contains `DLC_Content`, `RegisterFileBasedTemplate` supplies a restrict-to-existing flag to the JSON merge. That flag suppresses unmatched additions **during that file's processing**. It is not a global rule that data mods cannot add records whenever DLC is installed; ordinary base-file processing has a different context. Full-file replacement is also a separate branch. Test the resulting template and scenario state, especially when the same template type has base and DLC variants.

When targeting **1.0.57**, retest DLC-template edits and new record additions: that branch includes fixes introduced after 1.0.53a for those operations. Keep packages tested on each branch separately; see [version notes](compatibility.md).

To build a new scenario, first copy the smallest current scenario root appropriate to your start, assign a unique root ID, and keep its working groups. Then introduce your own groups/records incrementally. Trace every referenced start time, faction, nation, region, army, hab, fleet, and recurring event. Keep references acyclic, keep required DLC explicit, and test selecting the scenario, creating a campaign, advancing through its first scheduled updates, saving, and reloading. A changed start date alone is not a complete scenario conversion.

## Validation and compatibility

Use strict JSON for your authored files even though shipped files contain comments and occasional trailing commas accepted by the game's parser. A strict parser rejecting those in vanilla data does not establish that the game cannot load or mod that file.

Validate four different things:

1. **Syntax and packaging:** metadata object, template outer arrays, exact filenames, unique patch IDs, no accidental extra deployed examples.
2. **Schema and references:** existing fields/types/enum values, referenced template IDs, resource paths, localization keys, scenario group membership.
3. **Merged data:** expected scalar and array results under the selected policies and mod order, including DLC variants.
4. **Runtime behavior:** the chosen scenario, relevant UI/game action, next update/event, save/reload, and logs.

The loader attempts to resolve a template type by filename and logs errors for unknown types. A `.json` file cannot invent executable logic, a condition subclass, or a new enum member; new code-defined types need a code mod with an appropriate loading strategy. Keep auxiliary settings under a different extension and load them through your own code where appropriate.

Start a disposable base campaign and each Dark Skies scenario you intend to support. Check effective template values, localized text, and the actual game action. Save and reload when the change creates or modifies persistent state.

Then enable another mod that touches the same record: check scalar conflicts with distinct `LoadOrder` values and verify the intended default/concat/replace array results. For whole-file replacement, also check that every required record remains available. Test the combination after restarting, not just each mod separately.

For a release, state the tested build/branch, DLC, scenarios, new-game requirement, other required mods, overlapping template IDs, and any chosen load-order relationship. Repeat targeted validation when updates change those parts. A successful launch alone does not establish save compatibility or content reachability.

Continue with [global configuration](global-config.md), [content authoring](content-authoring.md), and [custom orgs](../tutorials/Custom%20Orgs.md). The [Json.NET merge settings reference](https://www.newtonsoft.com/json/help/html/T_Newtonsoft_Json_Linq_JsonMergeSettings.htm) explains the underlying library options.
