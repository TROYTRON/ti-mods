# Global configuration: exposed fields and effective values

`TIGlobalConfig` exposes many game-wide settings through the native template loader. A field can be configurable even when it is absent from the shipped `TIGlobalConfig.json`. Patch the `globalConfig` record with just the settings you intend to change; use the [native data guide](native-data.md) for metadata, merging, and installation.

Field names and types below target **1.0.53a** with Dark Skies. Check [version notes](compatibility.md) when updating, and test the setting in the scenario that will use it.

## A minimal config mod

Install the contents of the [global config example directory](../tutorials/tutorial-files/template-json-mod-examples/global-config-example) as:

```text
Mods/Disabled/Handbook Config Example/
  ModInfo.json
  TIGlobalConfig.json
```

`ModInfo.json`:

```json
{
  "Title": "Handbook Config Example",
  "Author": "Your name",
  "Description": "Sets the notification input delay to one second.",
  "LoadOrder": 0
}
```

`TIGlobalConfig.json`:

```json
[
  {
    "dataName": "globalConfig",
    "notificationReceiveInputDelay": 1.0
  }
]
```

`notificationReceiveInputDelay` is a `float` initialized to `0.5` by the constructor and absent from the base JSON. Notification button/hotkey delay coroutines read it. This patch requests one second: enable the mod, restart, and check the input delay on a newly displayed notification.

## What “default” means

A constructor assignment supplies an initial value. Base JSON, DLC variants, mods, and later game code can supply different values. A saved state, player option, or cache may also retain a value separately from the template.

An omitted field in a sparse patch preserves the working JSON record's value; it does not reset it to the constructor default. To find the effective setting, follow the selected scenario's config and the code that reads it, then check the relevant UI or game action.

Dark Skies' `Broken_Earth_Scenario/Templates/TIGlobalConfig.json` contains another `globalConfig` record tagged `PostApoc`. It overrides settings including `occupationSpeed` and `controlPointIPFactor`. Check this variant when changing Broken Earth balance. The [scenario resolver and DLC merge context](native-data.md#scenarios-and-dlc) also apply to global config.

## Field names and types

Two names found in older config examples need special handling:

| Member | Where to look in 1.0.53a |
| --- | --- |
| `PCGDPToReduceUnrestBy1` | Absent from `TIGlobalConfig`. Trace the current unrest calculation instead of guessing a replacement from a similar name. |
| `techTreeUIStarters` | Absent from current `TIGlobalConfig`, but present as `string[]` in current **`TIStartTimeTemplate`** and in shipped start-time records. Patch the record selected by your scenario. |

Other useful fields to inspect:

| Field(s) | Current type | Authoring implication |
| --- | --- | --- |
| `defaultDisableFactionValue` | `bool` | Inspect campaign-option consumers before assigning it a gameplay meaning. |
| `uiScaleValues` | `int[]` | Array policy matters; inspect supported UI ranges. |
| `allowNegativeInfluenceBaseIncome` | `bool` | Inspect influence-income consumers and scenario data. |
| `numEcosForCoreEcoRegion` | `int` | Referenced in the Broken Earth config dataset. |
| `numEcosForCoreMiningRegion` | `int` | Referenced in the Broken Earth config dataset. |
| `numEcosForCoreOilRegion` | `int` | Referenced in the Broken Earth config dataset. |
| `numPrioritiesForLegitimize` | `int` | Referenced in the Broken Earth config dataset. |
| `controlPointIPFactor` | `float` | Base construction and scenario data can differ. |
| `occupationSpeed` | `float` | Base construction and scenario data can differ. |
| `maxValueFromAttackerAdjacentControlPoints` | `float` | Referenced in the Broken Earth config dataset. |
| `AI_AlienSurveillanceDelayModifier_C`, `_N`, `_V`, `_B` | Four `float` fields | Preserve exact difficulty suffixes; inspect the selected difficulty's calculation. |
| `AI_AlienQuiescence_C`, `_N`, `_V`, `_B` | Four `float` fields | Preserve exact difficulty suffixes; inspect the selected difficulty's calculation. |
| `AI_WormholeSetupSpeed_C`, `_N`, `_V`, `_B` | Four `float` fields | Preserve exact difficulty suffixes; inspect the selected difficulty's calculation. |
| `meaningfulTradeThreshold` | `float` | Trace trade consumers before interpreting a numeric threshold. |
| `pathSunStylized` | `string` | Resource path; it is not a gameplay scalar. |
| `illus_BSBE_preCrashIntro` | `string` | Illustration resource path. |

The grouped suffix rows denote full names such as `AI_AlienQuiescence_N`, not standalone `_N` properties.

## Finding the right setting

Start with these fields, then read their callers to determine units and behavior:

| Intended change | Where to start |
| --- | --- |
| Councilor progression and org limits | `XPToLevelUp`, `councilorMaxOrgs`, `maxCouncilorAttribute`, then the councilor callers |
| Mission pacing and notification behavior | `dontStopBimonthlyMissions`, `notificationReceiveInputDelay`, the mission/notification controllers |
| Country priorities and economic balance | `priority_*`, `controlPointIPScaling`, `controlPointIPFactor`, scenario configs, and priority-completion calculations |
| Difficulty-sensitive alien pacing | The relevant `AI_*` fields **and** the selected `TIStartTimeTemplate` pacing fields |
| UI option bounds | The corresponding `*SliderMax`, `uiScaleValues`, or `maxShipsAllowedInCombat` consumer |
| Starting resources, completed techs, research-tree starting nodes | **`TIStartTimeTemplate`**, not a guessed global setting |
| A specific drive, org, project, weapon, or event | Its own template file, not a global override |

These are discovery routes, not a guarantee of semantics based on the name alone. Exact capitalization and spelling matter, including historical misspellings in field names. A field may represent a percentage, fraction, probability threshold, divisor, UI bound, resource amount, or timer. Read the arithmetic and the call context before choosing values. Arrays, enum lists, and resource paths require different validation from scalar numbers.

### Example: combat slider ceiling

`maxShipsAllowedInCombat` is an `int` with a constructor assignment of **90**. `OptionsMenuController` assigns it to `maxShipsInCombatSlider.maxValue`; the selected slider value is loaded separately using the `MaxShipsInCombat` preference.

Therefore a patch to this field changes the **slider ceiling**. It does not by itself set the player's selected battle size or prove that a battle will deploy that many ships at once. Record the chosen option and test an actual battle/reinforcement situation when evaluating a combat-size change.

### Example: replacing a speed array

If you intend to define the complete combat speed list, use `TemplatesToReplaceArrays` for `TIGlobalConfig.json` and supply the complete desired `combatLayerSpeedSettings`. A short array under the default index-merge policy leaves untouched elements at later indices. Selecting a file-wide replacement-array policy affects other arrays you supply in that same file, too.

## Test a config change

1. Record the game version, branch, DLC, and chosen scenario. Preserve your mod's prior version for comparison.
2. Search the current base **and DLC** template files for the exact field and `globalConfig`. Record explicit values and tags separately.
3. Inspect the current `TIGlobalConfig` member type and the methods reading/writing it. Read assembly metadata/IL with a tool such as ILSpy or dnlib; you do not need to instantiate game classes to learn their declarations.
4. Make a sparse patch with one clear intended effect. Choose an array policy only if your patch supplies arrays.
5. Validate JSON, enable the mod and Use Mods, restart, and inspect both the selected template value and the actual consuming UI/action where possible.
6. Test a new campaign when initialization is involved; additionally test an existing disposable save if you claim support for it. Check saving/reloading and the relevant update/event.
7. Record the expected and observed result with the build, scenario, and relevant campaign options.

For additional search terms, see [johnnylump's developer field notes](https://discord.com/channels/462769550841348126/1457825534275883018/1457825534275883018). That older listing is not an installable config; confirm any additional field against your current assembly.
