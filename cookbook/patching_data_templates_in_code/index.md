# Patching data templates in code

Use [native JSON patches](../../docs/native-data.md) for straightforward data edits. Use code when the change requires logic or coordinated access to loaded templates. The [TiMods.TemplatePatch project](../../examples/code/TiMods.TemplatePatch/Main.cs) demonstrates changing `PointDefenseLaserTurret.targetingRange_km` to `1337`; this is a balance-changing example.

## Apply after loading, before consumption

Find templates by their exact type and `dataName`, for example `TemplateManager.Find<TILaserWeaponTemplate>("PointDefenseLaserTurret", false)`. Handle a missing result, especially across versions and scenarios.

The example patches `TemplateManager.ValidateAllTemplates` with a prefix, so its edit runs after the templates have been registered. It also applies once during mod load because the first template pass may predate UMM. Setting one fixed value makes repeated application idempotent.

For array/list edits, define an identity for added entries and check before inserting. Repeated loading must not accumulate duplicates. If native array concatenation or replacement is sufficient, prefer those [documented merge controls](../../docs/native-data.md).

## Respect existing references

Do not call `ClearAllTemplates` to force an edit into a running campaign. Replacing the template database can invalidate objects that retain references to the old instances. A field edit may also leave derived or cached values stale; find the consuming code's update path.

The example has no hot-disable callback: **disable it and restart** to return to ordinary play. Removing a Harmony hook does not restore template values or values already copied elsewhere. If your mod supports live reversal, it must own the saved original values and account for later edits by other mods.

## Test

[Build and install](../../examples/code/README.md), check the log for the intended value, then exercise the consuming UI/gameplay. Test initial startup, campaign creation, save loading and skirmish when relevant. Repeat with supported scenarios and conflicting template mods.

The project compiles for **1.0.53a**. For **1.0.57**, recheck template identity, hook timing and caches against that installation before claiming compatibility.
