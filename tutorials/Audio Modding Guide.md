# Audio modding with FMOD

Adapted from [Stallion's FMOD guide](https://discord.com/channels/462769550841348126/1155428797408088064). Package paths and bank discovery apply to Windows stable **1.0.53a** with **Dark Skies**; authored banks still need playback testing.

## Start with the package shipped with your game

Copy and extract this file into a separate working directory:

```text
<Terra Invicta>/TerraInvicta_Data/StreamingAssets/ModdingTools/TerraInvictaFMODModdingPackage.zip
```

Open the contained `.fspro` project from the extracted copy, retaining its associated folders. Keep the working project outside the game installation.

Stallion's guide specifies **FMOD Studio 2.01.07**; the exact authoring version for newer packages is unconfirmed. Use [FMOD's downloads](https://www.fmod.com/download#fmodstudio) and check the game's package instructions before upgrading the project. Keep an untouched copy: a newer Studio release can change bank compatibility.

## Add a new event

1. Import your WAV or other supported source into the project's audio assets. Keep source recordings in your working project so future builds do not depend on files in a downloads folder.
2. Create an event from the asset. Use a **2D timeline** for ordinary non-positional sounds. Match a comparable supplied event when the sound needs parameters, sequencing, or positional behavior.
3. Give a new event a distinct path such as `event:/ExampleMod/ObjectiveComplete`. Use the event's **Copy Path** command; typing a guessed path makes troubleshooting harder.
4. Create a uniquely named bank, for example `ExampleAudio`, and assign the event to it. Unassigned events are not included just because their source audio is in the project.
5. Stallion's small-mod workflow marks the mod bank as **Master Bank** so its project-wide mixer metadata is available. Preserve the provided project's mixer setup and use a unique bank name. This is the TI package workflow, not a rule that every FMOD project should place all audio in a master bank.
6. In **Window > Mixer**, route voice events through the supplied voice processing bus, and other sounds through their matching supplied buses. This is what lets the game's volume controls affect them. Preview the event and adjust gain without clipping.

Banks contain event metadata and sample data; master banks also carry project-wide mixer information. FMOD does not automatically put a newly created event into a bank. [FMOD 2.01 bank documentation](https://www.fmod.com/docs/2.01/studio/getting-events-into-your-game.html)

## Reference the new sound

Use a field that already accepts an FMOD event path. In `TIObjectiveTemplate.json`, fields such as `completedVoicePathAppease` contain `event:/...` values. Start with one applicable field on a copied objective entry and replace its value with the event path, following the [native template mod guide](Create_Template_JSON_mod.md).

The field name is not a command that makes arbitrary gameplay play audio. Its consuming code must actually execute, and faction/language/context choices can select another field. Some councillor voice paths are assembled in code; adding a random JSON field cannot expose those call sites. For new audio, prove one simple event works before expanding a voice pack.

## Replace a supplied event

Use the event metadata from the supplied project:

1. Locate the existing event in the package and inspect its instruments, parameters, and routing.
2. Select the relevant **single instrument** and replace the missing/source audio with your asset.
3. Preserve the event's identity and behavior, assign the edited event to your mod bank, and build.

Preserve the supplied event's path and GUID; a new event with the same visible name is not equivalent. Test the replacement in the relevant language with conflicting audio mods disabled.

## Build and package

Select the Desktop target and review the build output directory. Keep **metadata and assets in a single bank** for this example, then package your bank and the generated strings bank used for path lookup, such as `ExampleAudio.bank` and `ExampleAudio.strings.bank`. If using split bank output instead, include its required `.assets.bank` file too. Build with **File > Build**, and retain the exact set of outputs from that build. [FMOD build outputs](https://www.fmod.com/docs/2.01/studio/getting-events-into-your-game.html#what-building-creates)

```text
Mods/Enabled/ExampleAudio/
  ModInfo.json
  TIObjectiveTemplate.json
  ExampleAudio.bank
  ExampleAudio.strings.bank
```

Copy only the mod's needed banks, not the entire vanilla bank set or the editable project. FMOD banks do not need Unity `.manifest` files. An exported GUID listing is a reference file, not audio content.

The loader finds banks in enabled mod folders and uses mod load order. **Audio Bank Mod Found** and **Loading mod audio bank** show discovery and load attempts; they do not confirm successful loading or playback. Check for FMOD errors and trigger the sound in the game.

## Verify in the game

Restart after deploying rebuilt banks. Trigger the exact objective/event or voice context and check that the expected audio plays, ends correctly, and responds to the relevant volume slider. Test the chosen language, repetition/interrupt behavior, and save/reload if the trigger is campaign-dependent.

| Problem | Check |
| --- | --- |
| No bank discovery log | Mod enabled, bank included in deployed folder, correct package copied |
| Bank found but silence | FMOD version mismatch, unassigned event, missing samples/strings bank, wrong event path, bus volume |
| Old sound still plays | Wrong event identity/context/language, duplicate bank, another mod replacement |
| Volume slider has no effect | Event's route through the supplied mixer buses |

Keep the Studio version, built bank list, game version/branch/DLC, and playback test results with your project. Use [debugging](../docs/debugging.md) for logs and [Workshop publishing](Uploading%20and%20Updating%20Workshop%20Mod.md) for release staging.
