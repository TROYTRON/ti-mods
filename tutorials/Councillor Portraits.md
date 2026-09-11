# Making councillor portraits

Based on Tayta and TROYTRON's portrait tutorials. Targets Windows stable **1.0.53a**, **Dark Skies**, and **Unity 2020.3.49f1**. The template fields are checked against that build; test your encoded video and bundle in the game.

## Prepare three distinct assets

| Asset | Suggested name | Purpose |
| --- | --- | --- |
| VP8 WebM video | `ada_idle.webm` | Main animated portrait; two identical frames can represent a still |
| RGBA PNG | `ada_portrait.png` | Static portrait, including when councilor videos are disabled |
| RGBA PNG | `ada_badge.png` | Map marker; crop the face for readability at small sizes |

Use a transparent source image for a cutout. Start with a square 512×512 or 1024×1024 source and judge the result at the game's UI scale; 512×512 is a practical choice, not a required maximum. Preserve a higher-resolution original. Optional older appearances need equivalent assets, or can explicitly reuse the young assets.

For the badge, use the supplied [PSD](tutorial-files/Terra%20Invicta%20Badge%20Maker.psd), [placement guide](tutorial-files/Terra%20Invicta%20Badge%20guide.png), and [border](tutorial-files/Terra%20Invicta%20Badge%20border.png). Preview PNGs over light and dark backgrounds to reveal halos and accidental opaque rectangles.

## Encode and check a still portrait

Install [FFmpeg](https://ffmpeg.org/download.html) and confirm `ffmpeg -version` works. Use **VP8** with the `libvpx` encoder; a `.webm` extension alone does not select a codec or preserve transparency.

Run in the directory containing `ada_portrait.png`:

```powershell
ffmpeg -loop 1 -framerate 1 -i ada_portrait.png -frames:v 2 -an -c:v libvpx -pix_fmt yuva420p -auto-alt-ref 0 ada_idle.webm
```

This requests two frames, no audio, VP8, and an alpha-capable pixel format. It does not overwrite an existing output without FFmpeg's confirmation. For animation, replace the looped still input with a prepared image sequence and choose its frame rate and duration. Available encoders depend on the FFmpeg build; inspect `ffmpeg -h encoder=libvpx` if the command fails. [FFmpeg codec documentation](https://ffmpeg.org/ffmpeg-codecs.html#libvpx)

For multiple still portraits, [Tayta's batch-conversion script](../mods/tayta/anime-councilors/waifu2vid.ps1) now uses the same encoding options and writes each PNG's WebM to `videos/`, preserving existing outputs. Run it from a working directory containing only the PNGs you intend to convert. The earlier portrait walkthrough also used [Shutter Encoder](https://www.shutterencoder.com/) as a graphical option; choose VP8 WebM and verify alpha with the checks below.

Check the stream and decode its alpha with the libvpx decoder:

```powershell
ffprobe -v error -select_streams v:0 -show_entries stream=codec_name,width,height,pix_fmt:stream_tags=alpha_mode -of default=noprint_wrappers=1 ada_idle.webm
ffmpeg -c:v libvpx -i ada_idle.webm -frames:v 1 -vf alphaextract ada_alpha.png
```

The alpha image should show transparent areas as black and opaque areas as white, with gray antialiasing at edges. An all-white image, missing-alpha error, or lost cutout edges needs investigation before import. `alpha_mode` metadata alone does not prove valid alpha pixels, and a decoder may report `yuv420p` without exposing WebM's auxiliary alpha stream. Keep the forced decoder check and visual preview.

Unity 2020.3 documents **WebM with VP8 alpha** support. This is a particular WebM alpha representation; ordinary VP8 video is not automatically transparent. For supported desktop decoding paths, Unity says transcoding is unnecessary for this format. If you enable transcoding, check **Keep Alpha** and verify the imported clip. Source alpha, encoder output, importer settings, and the player all matter. [Unity 2020.3 video transparency](https://docs.unity3d.com/2020.3/Documentation/Manual/VideoTransparency.html)

## Import in Unity and build

Follow [the AssetBundle workflow](../docs/assets.md#build-one-small-bundle-first). Its helper requires **Unity 2020.3.49f1** and Windows x86_64. Example project layout:

```text
Assets/
  Editor/CreateAssetBundles.cs
  Portraits/
    Images/ada_portrait.png
    Images/ada_badge.png
    Video/ada_idle.webm
```

Only `Editor` has a special meaning here. A folder named exactly `Assetbundle`, `2d`, or `Video` is not required for game asset lookup.

Avoid `charactericons` in custom bundle names. Tayta's original guide reports game paths that substitute `charactericons_gui` for that string, producing missing-asset errors unless the corresponding bundle also exists. A distinct bundle name avoids relying on this vanilla naming convention; recheck the consuming code when adapting an older portrait pack.

1. Select each PNG, set **Texture Type: Sprite (2D and UI)** and **Sprite Mode: Single**, and apply. Use input alpha and check the sprite preview. Keep sufficient max texture size and inspect compression artifacts on the badge.
2. Select the WebM and inspect its VideoClip preview and platform import settings. Use a preview scene if the importer preview does not make transparency clear.
3. Assign all three assets to a unique bundle, here `example_portraits`. Distinct names for video, portrait, and badge make short asset paths unambiguous.
4. Build for Windows x86_64 with the [helper](tutorial-files/CreateAssetBundles.cs), then copy `example_portraits` and `example_portraits.manifest` from `AssetBundles/Windows/` to the enabled native mod folder.

## Add the appearance template

Create `TICouncilorAppearanceTemplate.json` alongside the mod's `ModInfo.json` and bundle pair. This is valid JSON with one illustrative entry:

```json
[
  {
    "dataName": "ExampleAdaAppearance",
    "string": "ExampleAda",
    "enable": true,
    "specific_person": true,
    "idleVideoYoung": "example_portraits/ada_idle",
    "idleVideoOld": "example_portraits/ada_idle",
    "portraitYoung": "example_portraits/ada_portrait",
    "portraitOld": "example_portraits/ada_portrait",
    "iconYoung": "example_portraits/ada_badge",
    "iconOld": "example_portraits/ada_badge",
    "allowedGenders": ["Female"],
    "allowedAncestries": ["European"],
    "allowedJobNames": ["Scientist"]
  }
]
```

| Field | How to adapt it |
| --- | --- |
| `dataName` | Unique template identity; keep stable once referenced |
| `string` | Appearance's string identifier, following current vanilla examples |
| `enable` | Makes the appearance eligible for use |
| `specific_person` | `true` excludes this entry from ordinary random selection; test the customization/configured-councillor route for this example |
| `idleVideoYoung` / `idleVideoOld` | Video identifiers |
| `portraitYoung` / `portraitOld` | Static sprite identifiers |
| `iconYoung` / `iconOld` | Badge sprite identifiers |
| `allowedGenders`, `allowedAncestries`, `allowedJobNames` | Eligibility/filter values; copy supported values from installed templates |

The game selects old or young fields using the councillor's `useOldPortrait` value. Fill both variants explicitly, even when they point to the same asset.

JSON does not accept `#` comments, missing commas, or trailing commas. [The native JSON guide](Create_Template_JSON_mod.md) explains mod metadata and identity; keep the appearance's `dataName` distinct from the councillor template's identity.

## Test the appearance, not just startup

Restart with the mod enabled and select the portrait through customization or a configured test councillor. Check the customization list, councillor details, map badge, and static portrait with **councilor videos disabled**. Test old art and save/reload in a disposable campaign. Record game branch, version, DLC, and tool versions.

If the appearance is absent, check eligibility and JSON loading first. If selectable but blank, check asset names, manifests, and importer type. For a black cutout, compare the source PNG, decoded alpha, Unity preview, and game rendering in that order. See [debugging](../docs/debugging.md).

## Additional councilors need separate UI work

Portrait art does not expand councilor capacity. A capacity mod also needs every affected screen to handle the additional slots, including the full councilor list. Follow the [UI guide](IntroToUI.md) and test both normal and turned councilors in each view.
