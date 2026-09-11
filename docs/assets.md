# Assets: bundles, portraits, audio, and maps

For Windows stable **1.0.53a**, **Dark Skies**, and **Unity 2020.3.49f1**. Bundle discovery is checked against this build; the authoring workflows still require testing in the game.

## Choose the workflow

| Change | Workflow |
| --- | --- |
| Org icon, illustration, UI sprite | Unity texture/sprite import, AssetBundle, corresponding template asset path |
| Councillor appearance | [Portrait guide](../tutorials/Councillor%20Portraits.md): sprites, VP8 WebM, appearance template |
| Voice, music, sound effect | [Audio guide](../tutorials/Audio%20Modding%20Guide.md): FMOD banks and event references |
| Region shape or new map | [Map guide](../tutorials/MapCreation.md): outlines, geometry, bundles, template references |
| New ship model | Prefab, materials, attachments, hull/model mapping; see framework notes below |

For native metadata and JSON placement, start with [Creating Template JSON Mods](../tutorials/Create_Template_JSON_mod.md). A Unity bundle is not a ZIP archive, and an FMOD bank is not an AssetBundle.

## Build one small bundle first

1. Install [Unity 2020.3.49f1](https://unity.com/releases/editor/whats-new/2020.3.49) and Windows build support. The helper requires this Editor version to match the player; compatibility with other Editor versions is unverified.
2. Create a separate Unity **3D Core** project using the built-in render pipeline. Keep the editable project outside the game installation.
3. Put [CreateAssetBundles.cs](../tutorials/tutorial-files/CreateAssetBundles.cs) in `Assets/Editor/`. `Editor` is significant because the script uses `UnityEditor`; names such as `Portraits` and `2d` are organizational choices.
4. Import an asset. For UI images, select **Sprite (2D and UI)** and **Single** sprite mode. For models, inspect scale, materials, shaders, and required child objects against the intended game prefab. Importing an FBX alone does not reproduce that prefab.
5. In the Inspector's AssetBundle selector, assign a unique lower-case name such as `example_portraits`. Keep asset basenames unique within the bundle. Reusing a vanilla or another mod's bundle name can cause collisions.
6. Set **File > Build Settings > PC, Mac & Linux Standalone > Windows > x86_64**, and switch to that target. Select **Assets > Build AssetBundles**. The helper checks the Unity version and target, then writes to `AssetBundles/Windows` under the project root. Inspect the Unity Console for errors.

The helper uses Unity's standard bundle build API. Bundles target a platform; successful Editor import does not establish compatibility with another platform's player. [Unity 2020.3 build API](https://docs.unity3d.com/2020.3/Documentation/ScriptReference/BuildPipeline.BuildAssetBundles.html)

## Package the bundle and its manifest

A minimal native asset mod can keep these files together:

```text
Mods/Enabled/ExamplePortraits/
  ModInfo.json
  TICouncilorAppearanceTemplate.json
  example_portraits
  example_portraits.manifest
```

Enabled mod folders are scanned recursively for bundle manifests. **Ship each content bundle with its matching text `.manifest` file.** Rebuild both together and do not rename them afterward.

Unity also emits an aggregate manifest bundle named after the output directory, here `Windows`, with `Windows.manifest`. Those are build bookkeeping, not a portrait content bundle. Retain them in the build archive; do not blindly copy the entire output directory into the mod. The game's filename-based discovery can treat unrelated manifests as bundle candidates. Keep obsolete bundles, backup manifests, and `.meta` files out of the deployed folder. Include every content dependency bundle and corresponding manifest when your assets depend on other bundles.

Template paths follow the game's `bundleName/assetName` convention, for example `example_portraits/ada_portrait`. Use the imported asset name without its extension and preserve case. These are asset identifiers, not disk paths such as `C:\...` or `Assets/Portraits/...`. Compare the same field in installed templates when adapting another asset type.

## Check what the player loaded

Restart after changing a bundle. Confirm the intended mod is enabled, then reproduce the exact screen that loads its asset. Main-menu startup does not exercise a campaign portrait, tactical ship model, or map surface.

| Symptom | First checks |
| --- | --- |
| Asset missing | Enabled folder, bundle/manifest pair, template field, bundle and asset names |
| Pink material | Shader availability, render pipeline, material references, target platform |
| Black portrait background | Source alpha, encoded alpha, VideoClip import settings, player rendering |
| Old content persists | Duplicate mod copies, obsolete bundles, name collisions, complete restart |
| Works only in Editor | Player version, build target, dependencies, required components |

Use [debugging](debugging.md) to distinguish a loader failure from an error in the consuming UI. Record the game version, branch, DLC, OS, mod list, screenshot, and relevant log for a successful test.

## Inspect assets and ship examples

[AssetRipper](https://github.com/AssetRipper/AssetRipper) can inspect/export Unity assets and reconstructed projects. An export is a reference: scripts, materials, and prefabs may need repair before reuse. The original [AssetStudio](https://github.com/Perfare/AssetStudio) is archived; it remains historical tooling, but is not the only option for Unity 2020 assets. See [tools](tools.md).

[TIShipModdingFramework by UNNRazorback](https://github.com/UNNRazorback/TIShipModdingFramework) provides ship examples and integration code. Check the chosen revision's loader requirements and game compatibility, then follow its prefab hierarchy, hull/model mappings, materials, and attachment transforms. The framework adds no ship content by itself.

The repository also retains [Tayta's MonoMod adaptation](../tutorials/tutorial-files/TIShipModdingFramework_MonoMod.cs), including hull registration, prefab lookup, attachment, and effects code. It is a separate loader-specific implementation, useful when maintaining those ship mods. Its game API signatures have not been checked against the handbook baseline; compare them with your installation and follow the [MonoMod guide](../tutorials/MonoMod%20Guide.md) before adapting it.

Extra weapon slots need matching Unity ship objects and code, as well as template edits. Razorback's hierarchy guidance calls for paired `dorsal` and `ventral` mount objects for hull weapon slots, with a nose-weapon exception; verify it against the framework revision you use. [Mount guidance](https://discord.com/channels/462769550841348126/1378062688520769617/1463068361389899940).

Test ship icons, the designer, tactical combat, and visible undocked ships on the strategic map separately. Confirm propulsion and RCS effects in both views; a successful skirmish does not exercise every strategic-map effect.

## Separate data, behavior, and consuming UI

**Custom gun warheads:** nash's [More Weapon Functionality](https://discord.com/channels/462769550841348126/1540009421512380466/1540009421512380466) describes a Harmony extension for `warheadClass` behavior on gun, magnetic and plasma weapons. Its example requires the code extension; adding the JSON field alone is not a complete recipe. Compatibility and performance need testing with the chosen release.

**Additional councilors:** capacity changes require checking every affected screen, including the full councilor list. [Portrait art and capacity](../tutorials/Councillor%20Portraits.md#additional-councilors-need-separate-ui-work) are separate tasks.

**Surface hab modules:** when changing a module from `habType: Station` to `Any`, compare its surface assets with a working base module. In particular, a null asset key while opening the base construction UI is a reason to check `baseIconResource`. This is a diagnostic lead from [Stallion](https://discord.com/channels/462769550841348126/780213497028018207/1476001296602370058), not a complete model/icon conversion.

## Publish a reproducible package

Keep the editable project and tool versions for rebuilding after game updates. Distribute authored content and required metadata. [Workshop publishing](../tutorials/Uploading%20and%20Updating%20Workshop%20Mod.md) explains staging the tested package and preserving the item identity.
