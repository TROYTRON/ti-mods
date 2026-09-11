# Preserving mod state over save/load

Use [TiMods.SaveState](../../examples/code/TiMods.SaveState/Main.cs) for a small campaign counter. It creates, increments or removes state only after a button click in a loaded campaign, and never saves automatically. [Build it](../../examples/code/README.md) and use a disposable campaign.

## Register persistent state

Derive a public class from `TIGameState` and create it through `GameStateManager.CreateNewGameState<T>()`. The game assigns its ID and registers it for persistence. Plain static fields and dictionaries in your mod are not automatically saved.

Keep the assembly identity, namespace and class name stable after distribution. Include a schema version and define migrations before changing saved fields. The example retrieves its singleton with `GameStateManager.FindGameState<CampaignCounterState>()`; it does not retain a reference from a previous campaign. Its `PostInitializationInit_4` callback illustrates work performed after state restoration.

## Attach extra state to existing objects

For a feature such as ship veterancy, use a separate persistent record for each owner:

1. Store the owning ship's **`GameStateID`** alongside the counter or other custom values. Resolve it after loading with `GameStateManager.FindGameState<TISpaceShipState>(ownerId, false)`.
2. Build an in-memory dictionary keyed by that owner ID for quick lookup. Rebuild it from restored records and clear it when the campaign is unloaded; never carry old object references into a new campaign.
3. Make registration idempotent. A restored record must be indexed as-is, without creating a duplicate through the creation path.
4. When an owner is destroyed, look up an existing record with `TryGetValue`, remove that record through `GameStateManager`, then remove its index entry. A removal path must not call an accessor that creates missing state.

Choose initialization hooks after the owning objects exist, tolerate missing/destroyed owners, and test the owner relationship after reload. The maintained counter is deliberately smaller than a complete per-ship implementation.

[dkoiman's per-ship veterancy walkthrough](ship-veterancy.md) supplies the larger worked example: serialized ship references, initialization stages, index rebuilding, combat/destruction hooks, and UI indicators. It records the original tested version and the corrections needed when adapting its source; it is not covered by the maintained counter's build check.

## Remove state before uninstalling

Toggling a mod off does not erase its saved types. In **1.0.53a**, `RemoveGameState<T>` also leaves an empty type bucket that the serializer can still write. The example's cleanup removes that bucket only when it belongs to `CampaignCounterState` and is empty, using the private `GameStateManager.gamestates` dictionary.

Recheck that private API after updates. If cleanup fails, keep the mod installed. A removal changes the current campaign only: save to a **new file** before testing uninstall, and retain the original. Other saved references to a removed type would need their own migration.

## Test the complete cycle

1. Create/increment the counter and save a new disposable file.
2. Return to the menu and reload; check the value and restoration log.
3. Restart the application with the mod installed, then reload again.
4. Load a second campaign and confirm it has independent state.
5. Remove the counter, verify that instance and owned-bucket cleanup succeeded, and save a new file. Restart without the mod and test that new file.

The project compiles for the 1.0.53a baseline; the complete persistence and uninstall cycle has not been tested in game for this handbook. Rebuild and repeat it for **1.0.57**.
