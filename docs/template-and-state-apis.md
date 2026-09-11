# Template and campaign-state APIs

Templates hold static definitions loaded from JSON; `TIGameState` objects hold dynamic campaign data registered for saving. Keep balance/configuration in templates and persistent changes in game states. A value copied from a template into a state during creation can remain in an existing save even after the template changes.

This reference retains the API navigation from the original code-mod tutorial. Inspect signatures and callers in your own `Assembly-CSharp.dll` before using them; [compatibility](compatibility.md) records the handbook's validation baseline.

## Template identity and construction

A file such as `TIOrgTemplate.json` describes an array of the corresponding template class. `TIDataTemplate` supplies inherited members including `dataName` and `friendlyName`, so a field need not be declared directly on `TIOrgTemplate` to be available. `dataName` identifies the record; `friendlyName` is descriptive text, while `displayName` provides localized display text. The inspected baseline exposes `displayNameCurrentForStartScreen()` where the older tutorial used `displayNameCurrent()`. Follow [localization lookup](content-authoring.md#localization-files) rather than assuming the friendly name is the displayed name.

Inspect the template type's fields, properties, constructor defaults, and `IsValid(out string error)` when adding records or fields absent from vanilla JSON. The deserializer's treatment of a member depends on its attributes and serializer rules; a C# member's existence alone does not guarantee it is configurable. Unknown JSON fields do not create new C# behavior. See [GlobalConfig](global-config.md) for omitted-field examples.

`TIDataTemplate.CreateGameState()` is the template-to-state creation hook. Some types expose a constructor accepting a template name, but constructing a C# object alone does not register it in `TemplateManager`. Prefer the existing native loading/registration path for new content.

## Find and enumerate templates

| API | Use |
| --- | --- |
| `TemplateManager.global` | Access the special `TIGlobalConfig` singleton |
| `TemplateManager.Find<T>(dataName, allowChild: false)` | Find one template by type and identity; handle a missing result |
| `TemplateManager.IterateByClass<T>(allowChild: true)` | Enumerate a template type, optionally including derived types |
| `TemplateManager.GetAllTemplates<T>(allowChild: true)` | Obtain the matching templates as an array |
| `state.GetMyTemplate<T>()` | Access the template associated with a game state |

For example, inspect randomized organizations without copying their template data into the mod:

```csharp
foreach (TIOrgTemplate org in TemplateManager.IterateByClass<TIOrgTemplate>(true))
{
    if (org.randomized)
        modEntry.Logger.Log(org.dataName);
}
```

Here `modEntry` is the UMM entry passed to your mod. Run the query after the relevant templates exist. The `allowChild` flag controls derived template types; it does not select a scenario. Follow [scenario resolution](native-data.md#scenarios-and-dlc) to determine which records a campaign actually uses.

Template objects are shared. Mutating one can affect all consumers and may require cache refresh; it does not automatically update values already copied into game states. Use the [template patch recipe](../cookbook/patching_data_templates_in_code/index.md) for timing and restart requirements.

## Create, restore, and remove campaign state

Register new persistent objects with `GameStateManager.CreateNewGameState<T>()`, then initialize their fields. Use `GameStateManager.FindGameState<T>()` or the overload taking an owner/state ID to retrieve existing objects. Remove an owned record through `GameStateManager.RemoveGameState<T>(stateId, false)` rather than merely dropping it from a local dictionary.

The original persistence recipe identifies these lifecycle hooks, in order:

1. `PostGameStateCreateInit_OnCreationOnly_1`
2. `PostGlobalGameStateCreateInit_2`
3. `PostCanvasManagerCreateInit_3`
4. `PostInitializationInit_4`
5. `PostAllStartUpInit_5`
6. `PostVisualizerCreationInit_6`
7. `PostVisualizerCreationInit_7`

The first hook is creation-only; do not depend on it to rebuild indexes after loading. Select a later hook according to the dependencies your state needs, and inspect which hooks run on each creation/load path in the target build. Stages 4 and 5 are common places to restore mod indexes, but are not interchangeable when another object initializes between them.

The [campaign-counter recipe](../cookbook/save_mod_state_to_save_file/index.md) covers stable saved type identity, schema versions, removal, and uninstall testing. The [per-ship example](../cookbook/save_mod_state_to_save_file/ship-veterancy.md) adds owner references, combat updates, destruction cleanup, and UI feedback.
