# Creating a custom organization

A new org needs a template, localization, an icon reference, and a route into the campaign. This example adds a small Research org to `ModernOrgTemplates` and reuses a shipped logo for its first test. It requires no Unity project or code DLL. Add your own sprite after the data works.

This example targets **Terra Invicta 1.0.53a** with Dark Skies. Use the [native data guide](../docs/native-data.md) for merge behavior and scenario scope, and [version notes](../docs/compatibility.md) when targeting 1.0.57.

## Files and metadata

Copy the contents of the [org example directory](tutorial-files/template-json-mod-examples/org-example) into:

```text
Mods/Disabled/Handbook Org Example/
  ModInfo.json
  TIOrgTemplate.json
  TIMetaTemplate.json
  TIOrgTemplate.en
```

Use the matching directory name and metadata title. Keep the JSON files beside `ModInfo.json`; the loader reads settings from each JSON file's immediate directory.

```json
{
  "Title": "Handbook Org Example",
  "Author": "Your name",
  "Description": "Adds a small Research org to ModernOrgTemplates using an existing game logo.",
  "LoadOrder": 0,
  "TemplatesToConcatArrays": [
    "TIMetaTemplate.json"
  ]
}
```

The array policy is important: we will append our org ID to the existing scenario list. The default merge would instead modify its first array element.

## Define the org

Save this as `TIOrgTemplate.json`:

```json
[
  {
    "dataName": "Handbook_OpenResearchGroup",
    "friendlyName": "Open Research Group",
    "orgType": "Research",
    "tier": 1,
    "randomized": false,
    "allowedOnMarket": true,
    "requiresNationality": false,
    "costMoney": 20,
    "costInfluence": 10,
    "chanceIncomeResearch": 100,
    "incomeResearch": 5,
    "chanceScience": 100,
    "science": 1,
    "iconResource": "orglogos/JapanSocietyforthePromotionofScience"
  }
]
```

This is a new `dataName`, so its supplied values define a new template rather than patching an existing organization. `orgType` must be a supported non-`Any` value; `TIOrgTemplate.IsValid` rejects an unset type. The example uses `Research`.

The chance/value pairs follow the shipped non-randomized Research org pattern. Treat `incomeResearch: 5` as the configured base amount and test the resulting org state/UI; campaign scaling can matter. The logo is a temporary reference to an existing game resource.

The constructor initializes relevant trait/affinity/mission/tech-bonus collections to empty, so the example does not need null-filled placeholders. When you add restrictions or bonuses, inspect the current members and copy a record with matching behavior. Common fields include:

| Intent | Fields to inspect |
| --- | --- |
| Home-country / councilor restrictions | `homeRegionMapTemplateName`, `requiresNationality`, `requiredOwnerTraits`, `prohibitedOwnerTraits` |
| Ideological availability | `affinities`, `restricted` |
| Research gating | `requiredTechName`, plus the current `CanSpawn` implementation |
| Market presence | `allowedOnMarket`, campaign meta lists, and the org state/market selection |
| Mission access | `missionsGrantedNames` with exact mission IDs |
| Research-category bonus | `techBonuses` using the current structured entry format |
| Research projects | `projectsGranted`, `projectGrantedName`, with the appropriate supported semantics |
| Acquisition and income | Cost/income fields, associated chance/randomization fields, and org-state initialization |
| Visual identity | `iconResource` |

`homeRegionMapTemplateName` points to a **map-region** template such as `map_NorthHonshu`, not a nation's three-letter ID. An org being valid, being instantiated, qualifying for the market, and being purchasable by a specific councilor are separate checks.

## Register campaign availability

Save this as `TIMetaTemplate.json`:

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

Together with the metadata's concatenation policy, this appends the new ID without copying or replacing the existing org list. `ModernOrgTemplates` has `templateType: "TIOrgTemplate"`; the 2022 root selects it. Other scenario roots reuse it too, so this example can affect several scenarios.

For a different scenario, trace the active root and its org group. If it uses a separate group, extend that group or create/select your own. Adding `allowedOnMarket: true` to a template is not a substitute for arranging campaign creation/availability of a fixed org. Conversely, a debug command that grants an org does not prove its natural market route works.

The concat policy is file-wide: if you later add other `TIMetaTemplate` edits containing arrays, those arrays will append too. Keep the intended policy explicit and do not list this filename under `TemplatesToReplaceArrays` at the same time.

## Add localization

Save these lines in your mod's `TIOrgTemplate.en`:

```text
TIOrgTemplate.displayName.Handbook_OpenResearchGroup=Open Research Group
TIOrgTemplate.displayNameWithArticle.Handbook_OpenResearchGroup=the Open Research Group
```

Add equivalent files/keys for other languages you support. Keep each entry on one physical line and preserve any required markup/placeholders. Do not modify `StreamingAssets/Localization/en/TIOrgTemplate.en` in the installed game. See [localization details](../docs/content-authoring.md#localization-files) for parser behavior and language discovery.

## Test before adding artwork

1. Validate all three JSON files. Enable the mod and Use Mods in the game menu, then restart.
2. Start a disposable 2022 campaign. Check logs for the org, meta-template, localization, and missing-resource errors.
3. Check that the org template resolves and that the campaign creates the intended fixed-org state. Inspect its configured stats, icon, and localized name.
4. Check the ordinary market and councilor restrictions. Random market selection means “not offered immediately” is not enough to conclude the mod failed.
5. If you use the game's developer terminal, enter `giveorg ResistCouncil, Handbook_OpenResearchGroup`. It accepts a faction template ID or councilor display name before the comma, then the org template ID. Use a campaign with an existing councilor. The command can grant a new org to a councilor or move an existing org, so inspect both councilor inventories and faction pools when checking the result.
6. Save, quit, reload, and verify the org and its effects. Test another claimed scenario and any other enabled mods that edit the same lists.

Granting the org is a diagnostic shortcut; also test its normal market availability.

## Add a custom logo

Once the template works, replace the temporary `iconResource` with your own sprite resource. Use the current [asset workflow](../docs/assets.md) to import a Sprite (2D and UI), assign a uniquely named bundle, build for the game's platform, and deploy the bundle and its matching manifest inside your mod.

The asset workflow uses Unity **2020.3.49f1** as its build profile for this game's Windows baseline. Follow its import and platform settings, then check the resulting sprite in the game.

The native `iconResource` reference must resolve to the bundle/asset combination expected by the loader. Importing a PNG into Unity or naming a bundle is not enough by itself. Validate the exact resource string, sprite type, bundle discovery, and resulting UI image. Keep the working data-only version available so you can distinguish a template problem from an asset-loading problem.

For other content types, continue with [content authoring](../docs/content-authoring.md).
