# Terra Invicta Modding Hub

## Basic Knowledge
- Terra Invicta is currently (as of build 0.3.111) built with Unity **2020.3.48f1**.
- Configuration files are located in `\TerraInvicta_Data\StreamingAssets\Templates`.
- Localization files (game text) are located in `\TerraInvicta_Data\StreamingAssets\Localization\en`.
- The game's assets (textures, models, videos) are located in `\TerraInvicta_Data\StreamingAssets\AssetBundles`.
- The game's code is located in `\TerraInvicta_Data\Managed\Assembly-CSharp.dll`.

## Useful Tools
- [Unity](https://unity3d.com/get-unity/download/archive): Necessary for creating new AssetBundles to import new assets into the game.
  - Requires version Unity 2020.3.48f1. You **must** use this version or your game will crash when loading new bundles.
- [FFmpeg](https://www.ffmpeg.org/download.html): Used for creating .webms to add as new councillor portraits to TI.
  - [PowerShell script](mods/tayta/anime-councilors/waifu2vid.ps1) for mass conversion of pngs to webms.
- [GIMP](https://www.gimp.org/downloads/): Open-source image editor, useful for preparing new 2D graphics to import into the game.
  - [Older versions archive](https://download.gimp.org/pub/gimp/)
- [dnSpy](https://github.com/dnSpy/dnSpy/releases): Used to view and edit the game's code.
  - Note that this is decompiled code, therefore lacks documentation and can be difficult to decipher.
- [AssetStudio](https://github.com/Perfare/AssetStudio/releases): Allows decompiling the game's AssetBundles to see what's inside.
  - Currently the only known such utility that works with Unity 2020.
- [JSON to CSV](http://www.convertcsv.com/json-to-csv.htm): Enables easier editing of the game's configuration files in spreadsheet format.
- [Navigatable Tech Tree](https://rookiv.github.io/terra-invicta/) : Allows navigating the large tech tree outside the game.

## Tutorials
- [Introduction to Importing Assets](tutorials/Custom%20Orgs.md)
- [Creating Template JSON Mods](https://github.com/TROYTRON/ti-mods/blob/main/tutorials/Create_Template_JSON_mod.md)
- [Building Code Mods](/tutorials/code-mods-with-umm.md)
- [Reference Guide for Narrative Events](https://docs.google.com/document/d/1s3x96SyjvKFwx3pRSaMS7Zjo3FLwVVSLzSWzidT4CEo/edit)
- [Making New Councillor Portraits](tutorials/Councillor%20Portraits.md)
- [Uploading and Updating Steam Workshop Mods](https://github.com/TROYTRON/ti-mods/blob/main/tutorials/Uploading%20and%20Updating%20Workshop%20Mod.md)
- [An example of modifying ingame UI with code](tutorials/IntroToUI.md)
- [Modding Cookbook](cookbook/cookbook.md)
- [Introduction to MonoMod](tutorials/MonoMod%20Guide.md)
- [Audio Modding Guide by Stallion](tutorials/Audio%20Modding%20Guide.md)

## Tips and Tricks
- If you want to savegame edit you unpack, edit the embedded json and repack with z7ip, default settings for gzip (format: gzip, 5 - normal,word size:32)

## Modding Wishlist
- Native support for DLL patching, similar to how Rimworld does it.
  - This would alleviate the need for players to download and install third party patching tools (like Unity Mod Manager or BepInEx/MonoMod) to be able to play code mods.

## Future Investigation
- Ability to add new sounds (e.g. custom voice packs for councillors) to the game.
  - This has now been officially implemented by Pavonis.
- Ability to add new UI elements to the game.
  - See an example of modifying the UI in code - it is possible to do, but different things might be less or more complex to alter.
  - In general, this is extremely difficult to do from outside the editor.

## Mod Releases
- [Tayta Malikai](mods/tayta/Tayta's%20Mods.md)
