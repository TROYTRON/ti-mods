# Authoring content: localization, research, events, and campaign data

Content becomes usable when its templates, references, text, resources, and campaign entry points all agree. Start with one small change, verify it in the relevant game flow, then expand. The [native data guide](native-data.md) explains packaging and merge rules; [custom orgs](../tutorials/Custom%20Orgs.md) provides a complete small addition.

Examples target **1.0.53a** with Dark Skies. Test each new content type in a disposable campaign; see [version notes](compatibility.md) for actionable **1.0.57** differences.

## Choose the right template family

| Content | Start with | Also trace |
| --- | --- | --- |
| Organizations | `TIOrgTemplate.json` | `TIMetaTemplate`, nationality/traits, research requirements, localization, icon |
| Councilors | `TICouncilorTemplate.json`, `TICouncilorTypeTemplate.json` | Traits, appearance/voice, orgs, scenario groups, generation rules |
| Traits and missions | `TITraitTemplate.json`, `TIMissionTemplate.json` | Conditions, effects, allowed actor/target states, UI text |
| Global technologies and faction projects | `TITechTemplate.json`, `TIProjectTemplate.json` | Prerequisites, effects, unlock-bearing content, objectives, research UI |
| Narrative events | `TINarrativeEventTemplate.json` | Condition classes, target types, options/outcomes, effects, text, illustration |
| Repeated or scheduled campaign actions | `TITimeEventTemplate.json` | Scenario recurring-event groups and their existing game implementations |
| National starting state | `TINationTemplate.json`, `TIRegionTemplate.json`, `TIBilateralTemplate.json`, `TIArmyTemplate.json` | Scenario groups, map references, claims, ownership, diplomacy |
| Space economy and habs | `TIHabModuleTemplate.json`, `TIHabTemplate.json`, `TIHabSiteTemplate.json` | Tech/project requirements, effects, resources, starting hab groups |
| Ships and parts | Hull/drive/power plant/weapon/utility templates, `TISpaceShipTemplate.json` | Designs, fleets, part slots, resource models, research unlocks |
| Text-only changes | `*.en`, `*.fr`, or the actual language extension | Exact lookup keys and runtime placeholders |
| New scenario | `TIMetaTemplate.json`, `TIStartTimeTemplate.json` | All selected campaign groups, tags, DLC, and their references |

The existence of a template field does not make every behavior data-driven. Conditions, enum values, formulas, actions, and UI controllers are implemented in code. Reuse supported implementations; a new arbitrary `$type`, enum string, or property name cannot create a mechanic. See the [code mod guide](../tutorials/code-mods-with-umm.md) when the intended behavior requires code changes.

## Localization files

Shipped English text is under `TerraInvicta_Data/StreamingAssets/Localization/en`. For a mod, put a file such as `TIOrgTemplate.en` beside `ModInfo.json` in your mod directory. It contains **one `key=value` entry per physical line**, not JSON:

```text
TIOrgTemplate.displayName.Handbook_OpenResearchGroup=Open Research Group
TIOrgTemplate.displayNameWithArticle.Handbook_OpenResearchGroup=the Open Research Group
```

Only include the keys you add or override. Do not edit the game's original localization file. Use the actual installed language code, such as `.en` or `.fr`; filename extensions participate in language discovery. A JSON field named `friendlyName` is useful reference text but is not a substitute for the keys the UI asks the localization manager to resolve.

`LocalizationManager` processes base language files, then DLC language files, then mod language files recursively. Mod entries can replace existing keys. This loader does not use the JSON `LoadOrder` sort, so do not rely on that setting to resolve competing localization edits.

Keep `.en`, `.fr`, and other localization files out of `TemplatesToConcatArrays`; these are text entries, not template arrays. If names disappear when combining mods, validate metadata syntax first, then check exact localization keys and conflicts.

Parser and formatting rules:

- The first `=` separates the key from the value. Keep the exact key, without added spaces.
- A physical line break ends the entry. Use `<br/>` for a displayed line break; do not assume a literal backslash-plus-`n` escape will be decoded.
- `//` starts a comment inside a value. A literal `https://...` in localization text can therefore be cut off by the parser; do not assume this format behaves like a general-purpose text file.
- Preserve positional placeholders such as `{0}` and `{1}`, and keep their meaning in translations.
- Named replacements such as `{targetRegionName}` belong to particular consumers, including narrative events. Verify that the target/secondary state used by the content supplies them.
- Preserve supported rich-text markup and its closing tags. Test actual UI wrapping, font glyphs, and tooltips in the chosen language.

When updating translations for **1.0.57**, check `TIOfficerTemplate.InternalDamageTaken`: the intervening update changes its placeholder from `{4}` to `{1}`. Preserve the target build's argument contract when overriding that key. See [version notes](compatibility.md).

Adding a new language also involves `TILocalizationTemplate` and potentially font assets. Its settings include `active`, `core`, `requiresFontChange`, `headlineFontPath`, and `bodyTextFontPath`. Follow an installed language template and test its language-menu selection and fonts; adding a new file extension alone is insufficient.

Localization aliases and scenario postfixes are separate from JSON identity. For content intended to share another record's text, inspect its `localizationAlias` and the consuming lookup. For a scenario-specific translation, follow the selected scenario's current `scenarioLocalizationPostfix` and `templatesToUseDefaultLocalization` behavior. Do not perform a global prefix substitution based on an old scenario tutorial.

## Technology and project changes

Global technologies use `TITechTemplate`; faction projects use `TIProjectTemplate`. Both inherit common members from `TIGenericTechTemplate`, including `techCategory`, `researchCost`, `prereqs`, `altPrereq0`, `altPrereq1`, `effects`, and resource-path fields. Projects add availability/unlock chances, faction/objective/milestone restrictions, uniqueness/repeatability, and possible org/resource grants.

The [tech example](../tutorials/tutorial-files/template-json-mod-examples/tech-example) makes a deliberately small change:

```json
[
  {
    "dataName": "AdAstra",
    "researchCost": 2500
  }
]
```

In the base file, `AdAstra` has `researchCost: 5000` and prerequisites `MissiontotheMoon` and `OrbitalShipbuilding`. The example preserves those requirements. Campaign speed options, bonuses, or other calculations can affect displayed/effective costs and progress, so compare the underlying template and the UI under recorded options.

For a new technology or project:

1. Copy one current record with the same role and prerequisite style. Give it a unique `dataName`, and deliberately set cost, category, AI role, and any special restrictions.
2. Resolve every prerequisite by exact ID. Check for cycles, unreachable branches, accidental self-references, and requirements unavailable to the intended faction/scenario.
3. Decide how projects become available. `factionAvailableChance`, `initialUnlockChance`, `deltaUnlockChance`, and `maxUnlockChance` are different members; setting the cost does not make a restricted project available.
4. Connect the actual benefit. `effects` refers to effect templates; habs, ship parts, orgs, traits, and objectives may carry their own project/tech requirement fields. Search both the research record and the content it is intended to unlock.
5. Supply localization and resource references appropriate to that type. Test the full research tree, the available-research list, the completion notification, and the unlocked item/action.
6. Include AI behavior and scenario starting/completed research in testing. Research a project naturally as well as using a diagnostic shortcut, because forced completion can bypass unlock checks.

For a deliberate replacement of the Ad Astra prerequisite list, metadata would select `"TemplatesToReplaceArrays": ["TITechTemplate.json"]`, and the patch might be:

```json
[
  {
    "dataName": "AdAstra",
    "prereqs": [
      "MissionToSpace"
    ]
  }
]
```

This is a **separate balance change**, not part of the cost-only example. Without replacement-array handling, supplying one prerequisite changes index zero and leaves the old second prerequisite in place. Concatenation instead preserves all old prerequisites and adds another; use it only when adding a requirement is the intended design. `altPrereq0` and `altPrereq1` have their own consumers; do not infer their full boolean relationship from the names.

Shipped English tech keys include `TITechTemplate.displayName.<ID>`, `TITechTemplate.summary.<ID>`, `TITechTemplate.description.<ID>`, and `TITechTemplate.quote.<ID>`. Projects have their own `TIProjectTemplate` keys. Copy the relevant key pattern from the same type, then author your own text. Keep scenario start nodes and completed research in `TIStartTimeTemplate`; `techTreeUIStarters` is not a current global-config field.

For large trees, test the full-tree UI as well as project availability. **1.0.57** fixes a full-tree viewing crash introduced in 1.0.54. It also makes AI repeatable-project selection consider daily income, reducing willingness to choose repeatables; retest research balance when moving from 1.0.53a. [1.0.57 developer notes](https://discord.com/channels/462769550841348126/1023705782161776650/1544083776710377593).

## Narrative events and conditions

`TINarrativeEventTemplate` describes eligibility, selection weight, targets, options, and outcomes. The event's availability and effects depend on the state the event system supplies. A syntactically valid event can remain unreachable or fail only when a specific option is chosen.

Trace the complete chain:

```text
Eligibility / required unlock / date / cooldown
  -> primary target and target conditions
  -> optional secondary state and conditions
  -> eventOptions and affordability / AI preference
  -> weighted outcomes
  -> effect templates, grants, follow-up events, and notification text
```

Useful current member groups include:

| Concern | Members to inspect |
| --- | --- |
| Eligibility and repeatability | `requiresAliens`, `year`, `endYear`, `earliestMonth`, `latestMonth`, `reqTechDataName`, `reqEventUnlock`, `repeatable`, `forceEvent` |
| Selection | `baseWeight`, `altBaseWeight`, weight deltas, global/target cooldowns |
| Targets | `targetType`, `possibleTargetDataNames`, `targetConditions`, `targetWeightModifiers` |
| Secondary state | `secondaryStateType`, `secondaryStateConditions`, `possibleSecondaryStateDataNames`, secondary weights |
| Choices | `numOptions`, `eventOptions`, each option's conditions/costs/AI preferences and outcomes |
| Presentation | `illustrationResource`, `soundResource`, publicity fields, and localization |

For an existing event, begin with a scalar patch. For example, this changes the Hurricane event's ordinary base weight from 4 to 2:

```json
[
  {
    "dataName": "event_Hurricane",
    "baseWeight": 2
  }
]
```

A weight is not a direct percent chance. Hurricane also has an alternative weight conditioned on `ClimateChangeMitigation`, target/season restrictions, and cooldowns. The patch does not override those. Validate the intended eligible state rather than waiting for one random occurrence and treating it as a frequency measurement.

If an outcome with zero starting weight never appears despite its modifiers, a [community workaround from Cend](https://discord.com/channels/462769550841348126/780213497028018207/1469732101531107471) uses a positive starting weight and an equally large negative modifier unless the condition holds. This is unverified on the target builds: test both condition states before using it. Outcome weights are separate from the event's `baseWeight` above.

For a new event, use one current event with the same target shape as a starting point. Give it a unique ID, remove unrelated restrictions deliberately, and preserve complete option/outcome structure. Conditions use polymorphic `$type` names. Hurricane includes this condition:

```json
{
  "$type": "TIRegionCondition_bCoastal",
  "sign": "EqualTo",
  "strValue": "TRUE"
}
```

This fragment belongs inside an appropriate region condition list; it is not a standalone template file. The class, comparison enum, value representation, and target type must agree. Other conditions may use different fields or expect another state type. Do not invent condition class names or assume every condition accepts a numeric JSON value instead of its documented string representation.

Use replacement-array handling when supplying complete redesigned `targetConditions`, `eventOptions`, or nested outcomes. The default index merge can preserve an old condition field, cost, or outcome after you think you have replaced it. The setting affects all supplied arrays in `TINarrativeEventTemplate.json`; review the whole file before combining small and large event edits.

Event localization uses multiple patterns. Hurricane uses keys including:

```text
TINarrativeEventTemplate.displayName.event_Hurricane
TINarrativeEventTemplate.event_Hurricane.summary
TINarrativeEventTemplate.event_Hurricane.query
TINarrativeEventTemplate.event_Hurricane.option0
TINarrativeEventTemplate.event_Hurricane.optionDetail0
TINarrativeEventTemplate.event_Hurricane.optionResult0
```

These lines illustrate key names only; actual localization entries need `=text`. Copy the corresponding patterns for your event ID and every option/outcome used. Verify target substitutions, actor-dependent text, affordability, each choice, each possible outcome, cooldowns, and subsequent scheduling. Check that `numOptions` matches the intended choices. A forced debug trigger is useful for UI/effect testing but does not establish that natural eligibility or scheduling works.

Effects themselves live in `TIEffectTemplate`; event outcomes reference them using fields such as `effectTemplateNames` and `delayedEffectTemplateNames`. Reuse effects whose input state and intended scope match the event. Inspect effect implementation and stacking/removal behavior before repurposing one based only on its friendly name.

## Nations, regions, and scenarios

The [Alaska tutorial](../tutorials/Create_Template_JSON_mod.md) demonstrates a small initial-ownership change while preserving a claim. A fuller scenario change must also account for the selected nation/region IDs, capitals, army locations, bilateral relations, initial ownership consistency, and the relevant meta groups. Map geography and map visual assets are separate concerns from many national data changes.

### Nation flags

`TINationTemplate` exposes the string members `flagResource` and `unionFlagResource`. The base record `ACE` uses `nationflags/Aceh`; Algeria's `DZA` record uses `nationflags/Algeria` and also supplies `unionFlagResource: "nationflags/MaghrebUnion"`. These are resource references, not paths to loose PNG files.

For a minimal resource-wiring test, put this valid `TINationTemplate.json` beside your native `ModInfo.json`:

```json
[
  {
    "dataName": "DZA",
    "flagResource": "nationflags/Aceh"
  }
]
```

This deliberately assigns the existing Aceh image to Algeria's ordinary flag so that a visible change can be tested without building an asset. It is a temporary demonstration, not a suggested final flag. Enable the mod, restart, and inspect Algeria in a new 2022 campaign. Check both the nation information panel and another UI surface that displays its flag; an image cached by one surface is not proof of every consumer's behavior.

For your own flag, follow [the small-bundle asset workflow](assets.md#build-one-small-bundle-first): import your image as **Sprite (2D and UI)**, choose a unique bundle and sprite name, build with the matching editor/platform, and deploy the named content bundle plus its matching text manifest inside your mod. Replace `flagResource` with the exact resource string for that bundle/asset, such as `handbook_flags/handbook_algeria` only after those names actually exist. Keep the root build bookkeeping bundle and authoring project out of the deployed package. Validate bundle discovery, asset name/type, transparency, aspect ratio, and the rendered flag.

Treat `unionFlagResource` as a separate optional change. The shipped Algeria record also has `unionTrigger`; the field's presence does not prove that its union image is always selected or that changing the image changes the political trigger. Trace the current flag-selection consumer and test the relevant union state before claiming support. Changing a flag resource alone leaves nation creation, claims, diplomacy, localization, and scenario membership to their existing data.

Patch the nation ID selected by your scenario. The installed data also contains `2026_DZA` and `2070_DZA` records with their own flag fields; patching `DZA` does not change those entries. Check DLC variants and the selected scenario's references, then test each intended start date.

### A new scenario root

A new `TIMetaTemplate` root can reuse the 2022 groups as a starting point:

```json
[
  {
    "dataName": "Handbook_2022Scenario",
    "friendlyName": "Handbook 2022 Scenario",
    "templateType": "TIMetaTemplate",
    "isNewCampaignOption": true,
    "newCampaignOptionCategory": "Scenario",
    "optionPriority": 1,
    "listPriority": 20,
    "tutorialAllowed": false,
    "scenarioTags": [
      "NotPostApoc"
    ],
    "templateNames": [
      "ModernStartTime",
      "ModernRegions",
      "ModernNations",
      "ModernArmies",
      "StandardCouncilors",
      "ModernStartFleets",
      "ModernOrgTemplates",
      "ModernHabs",
      "StandardRepeatingEvents"
    ]
  }
]
```

Package it with native metadata and appropriate `TIMetaTemplate.en` text using the current scenario-name lookup pattern. This example reuses existing content; verify its menu choice and campaign creation before replacing one group at a time. Base a new start-time record on a current complete record and select it through a corresponding `TIStartTimeTemplate` meta group. Avoid changing a shared start record when only your new scenario should change.

When extending a scenario, list which groups are intentionally shared: changing `ModernOrgTemplates`, for example, affects every scenario that selects that group. New distinct group IDs give you a separate list; sparse patches to shared IDs alter the shared list. For Dark Skies variants, trace `requiredDLC`, tags, and duplicate-template resolution as explained in [native data](native-data.md#scenarios-and-dlc).

## A content release checklist

| Stage | Check |
| --- | --- |
| Packaging | Exact mod title/directory, required template/localization/assets, no accidental backup data |
| Static data | Valid JSON, supported members, references resolve, intentional array policy, unique new IDs |
| Discovery | Mod/template appears in logs and the intended scenario selects the necessary content |
| Reachability | Org reaches the relevant pool, project unlocks, event qualifies, scenario creates required states |
| Presentation | Names, grammar, placeholders, icons, descriptions, and UI fit in each claimed language |
| Behavior | Actual stats/effects/choices work, including AI and disabled/restricted cases |
| Persistence | Disposable campaign survives save/reload and the next relevant update |
| Compatibility | Recorded build/branch, DLC, scenarios, overlapping mods, and new-game requirements |
