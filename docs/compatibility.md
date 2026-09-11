# Compatibility

## Recorded validation baseline

These versions record the installation used to check the handbook; they do not require future mods or documentation changes to target that release. Stable and beta labels below describe that validation snapshot. Confirm the installed build and repeat affected checks when updating.

| Component | Version |
|---|---|
| Game | Windows Steam stable **1.0.53a**, with Dark Skies |
| Unity player / bundle-build profile | **2020.3.49f1**, Windows x86_64 |
| Unity Mod Manager | **0.33** |
| Bundled Harmony | **2.3.6**, targeting .NET Framework 4.8 |
| Maintained mod projects | **net48**, SDK **8.0.424** |
| Beta / experimental | **1.0.57**; relevant differences below |

The five C# mod examples compile against this stable installation. The native merge-helper checks pass against its installed code. In-game smoke tests and asset/audio authoring tests remain incomplete; **1.0.57 binaries have not been checked**. Follow the [example test steps](../examples/code/README.md) before relying on a feature in a campaign.

Confirm the actual version in the title screen and `Player.log` after a Steam update. The branch selection alone does not confirm the downloaded version. Other storefronts may ship different builds.

## Differences when targeting 1.0.57

| Area | What changes for a mod author |
|---|---|
| DLC templates and new records | 1.0.54 includes additional fixes for DLC template mods and new `dataName` records. Do not assume those fixes are in 1.0.53a. Test registration and availability in each intended scenario. |
| Large technology trees | Stable includes increased tree capacity, without a documented numeric limit. A full-tree display crash introduced in 1.0.54 is fixed in 1.0.57. Test full-tree rendering, prerequisites, project availability, and AI selection. |
| Repeatable projects | 1.0.57 considers daily income when the AI selects repeatable projects; rebalance assumptions based on older AI behavior may need adjustment. |
| Localization | From 1.0.54, the reported `TIOfficerTemplate.InternalDamageTaken` placeholder changes from `{4}` to `{1}`. Match the target build's formatting arguments. |
| Code hooks and caches | 1.0.54 adds caches around councilor stat limits, faction habs, research connections/tooltips, and AI/army queries. Check refresh/invalidation when patching underlying values. |
| Saves | 1.0.54 adds scenario/game-version metadata. Metadata helps identify a save; it does not make custom state portable between mod versions or DLC configurations. |
| Orbital movement | 1.0.55 reverts a `CartesianState.ChangeReferenceFrame` optimization that broke L1/L2 trajectories. Recheck any workaround written for 1.0.54. |

The 1.0.53a release backports a selected army crash fix; it does not include every 1.0.54 change. Developer notes: [1.0.53a](https://steamcommunity.com/games/1176470/announcements/detail/681885756685288240), [1.0.54](https://discord.com/channels/462769550841348126/1023705782161776650/1542321724899729419), [1.0.55](https://discord.com/channels/462769550841348126/1023705782161776650/1542690453541888010), [1.0.57](https://discord.com/channels/462769550841348126/1023705782161776650/1544083776710377593).

## Updating a mod

Rebuild against the destination installation. Check changed field types, method overloads, patch timing, localization arguments, and persisted state. Repeat the affected tests in a disposable campaign, including save/load and removal where relevant. For Dark Skies, test each targeted scenario separately.

Keep your source revision and tool versions with each release. State supported game branches and dependencies in the mod's documentation. A code loader cannot make conflicting patches or missing DLC content compatible automatically.
