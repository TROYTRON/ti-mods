# Installation, architecture, and recovery

These paths apply to Windows, Terra Invicta 1.0.53a with Dark Skies. See [compatibility](compatibility.md) for the toolchain and beta differences. Check paths in your own installation when using another storefront or operating system.

## Identify the actual build

In Steam, use Manage > Browse local files to find the game root. Confirm the title screen and the beginning of `Player.log`; a Steam branch label alone does not confirm that its download has completed. This handbook targets stable 1.0.53a; beta/experimental 1.0.57 needs separate compatibility checks.

## Paths relative to the game root

| Path | Role |
|---|---|
| `TerraInvicta_Data/StreamingAssets/Templates` | Base template inputs; inspect these, do not use edits here as your distribution format |
| `TerraInvicta_Data/StreamingAssets/Localization/en` | Base English localization |
| `TerraInvicta_Data/StreamingAssets/AssetBundles` | Shipped assets |
| `TerraInvicta_Data/StreamingAssets/ModdingTools` | Shipped authoring resources, including the FMOD package |
| `TerraInvicta_Data/Managed/Assembly-CSharp.dll` | Main game assembly for local API inspection and references |
| `DLC_Content/DarkSkies` | DLC scenario templates, localization, and assets |
| `Mods/Enabled` and `Mods/Disabled` | Native mod installation locations used by this build |
| `TerraInvicta_Data/Managed/UnityModManager` | UMM and bundled Harmony in the inspected installation |

Native JSON/assets are loaded by the game's mod system. Code requires a compatible loader such as UMM and its own metadata. In the inspected UMM configuration its metadata filename is `ModFile.json`; native data metadata is `ModInfo.json`. Do not treat them as interchangeable. See [code setup](../tutorials/code-mods-with-umm.md).

The game's current template pipeline stages and combines data **in memory**. A successful native patch does not require the original template file on disk to change. Validate the effective result through gameplay, diagnostics, and logs, as described in [native data](native-data.md).

## Logs and saved state

The observed Unity log location is:

```text
%USERPROFILE%/AppData/LocalLow/Pavonis Interactive/TerraInvicta/Player.log
```

The directory is `TerraInvicta` without a space. `Player-prev.log` may contain the previous launch. UMM also writes `Log.txt` in its managed directory. Preserve relevant logs before relaunching; redact personal paths and unrelated mod output before sharing them.

The default save path observed here is:

```text
%USERPROFILE%/Documents/My Games/TerraInvicta/Saves
```

The log reports `savedGamesPath`; use that value if Documents is redirected or an alternate save directory is configured. Template defaults and existing serialized campaign state are different layers. A new campaign is the clearest first test for new templates, scenario registration, or a value copied into state at campaign creation.

When editing saves, work on a copy, identify its actual compression/format, and verify save/load through the target game build. Do not apply a generic unzip/repack command to the only copy of a campaign.

For a **gzip-compressed JSON save**, the original community workflow uses 7-Zip to extract the embedded JSON, edit it, then compress that JSON as **gzip**, compression level **5 (Normal)**, word size **32**. These are the reported working settings, not proof that every save uses gzip or requires those exact compression settings. Preserve the save's filename/extension, avoid adding a wrapper directory or creating a ZIP instead, and load the edited copy in the game to verify it.

## A recoverable test loop

1. Record game/DLC versions, enabled native mods, UMM version/settings, and the files your test installs. Back up affected configuration and protect existing saves.
2. Install one minimal example into its own uniquely named directory. Keep the source outside the game installation.
3. Start a disposable campaign using the intended scenario. Check both the visible result and logs, including errors appearing before the feature is used.
4. Add a second mod only when testing an explicit interaction. Record load order and the expected result.
5. Exit the game before changing installed assemblies. Remove only your test files and restore the recorded configuration. Test removal on a disposable save if the mod introduces persisted types or references.

Do not disable security software or run the game as administrator as a generic modding fix. Start with malformed JSON, missing metadata/dependencies, wrong versions, missing assets, and conflicting patches. Include the exact enabled mod list when reporting a modded crash.
