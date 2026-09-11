# Modifying Unity UI

For settings, use the maintained [starter's UMM `OnGUI` callback](../examples/code/TiMods.Starter/Main.cs). Modifying a game screen requires a separate step: inspect the actual objects and controller in the target build. Old ship-designer object names and grid dimensions are not a current interface contract.

## Unity's object structure

A `GameObject` owns components describing appearance and behavior. Its `Transform` defines the parent/child hierarchy; UI objects commonly use `RectTransform`. A visible control may combine an image, a text component, input handlers and game-specific scripts.

Cloning a prefab or existing row copies its children and components, including references and event wiring. It does not automatically register the clone in a game controller's lists or dictionaries.

## Inspect a screen before patching it

1. Open the screen with [UnityExplorer or RuntimeUnityEditor](../docs/debugging.md#explore-live-objects-with-unityexplorer).
2. Record the screen root, relevant children, component types and active/inactive state. Note which objects appear lazily or are recreated on reopening.
3. Find the controlling type in the installed `Assembly-CSharp.dll`. Trace setup, refresh and teardown, plus any collections that track controls.
4. Choose a hook after the required controls exist. Resolve the current instance there instead of retaining a controller from a previous screen or campaign.
5. Start with one visible change, then close/reopen the screen before expanding the patch.

| Task | API | Detail to check |
| --- | --- | --- |
| Inspect descendants | `GetComponentsInChildren<T>(true)` | Include inactive objects deliberately; avoid a whole-scene search every frame |
| Resolve a child | `Transform.Find("Parent/Child")` | A simple name is not recursive; handle a null result |
| Clone a control | `UnityEngine.Object.Instantiate` | Review copied references, listeners, scripts and layout settings |
| Change grid layout | `GridLayoutGroup` and `SetSiblingIndex` | Constraint mode/count and child order must agree |
| Remove owned objects | `UnityEngine.Object.Destroy` | Remove your listeners and controller registrations too |

Unity's [2020.3 scripting reference](https://docs.unity3d.com/2020.3/Documentation/ScriptReference/) documents these APIs. Add references to the actual component-owning assemblies with `Private=false`; prefab UI may need `UnityEngine.UI.dll` and the appropriate text assembly, beyond the starter's IMGUI references.

## Extend a row or slot layout

Find a suitable prototype, clone it under the correct parent, and set its data and layout. Update every controller collection used for refresh, input and lookup. Assign stable sibling order and check clipping, anchors and scroll bounds.

Increasing a grid count does not create usable slots. Ship-slot changes also involve the hull template, prefab mount hierarchy and game code; follow the [ship requirements](../docs/assets.md#inspect-assets-and-ship-examples). Extra councilor capacity likewise needs the large councilor screen to support the added entries. Inspect each consuming screen instead of assuming one visible list covers the feature.

Make setup repeatable: skip controls already created for that screen instance, and avoid duplicate listeners. Clean up only your own controls and callbacks. Reacquire game-state references after loading a campaign; teardown must also tolerate partially initialized or already-destroyed objects.

## Test the whole screen lifecycle

Check first open, refresh, close, second open, changed selection, game reload and a second campaign. Test tooltips, buttons, keyboard/input focus, scrolling, long localized text and relevant display scales. Enabling/disabling a mod must not leave duplicate or orphaned controls.

The starter's settings UI compiles for 1.0.53a; game-screen expansion has not been runtime-tested here. See [dkoiman's worked ship-designer example](Ship%20Designer%20UI.md) for object traversal, slot cloning, controller collections, sibling ordering, and screenshots. Its private fields and grid dimensions must be adapted to the target build, and its setup needs a repeated-initialization guard. Keep your source project and a clear reproduction sequence with screenshots and logs.
