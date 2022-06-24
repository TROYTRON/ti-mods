# Terra Invicta Modding Hub

## Basic Knowledge
- Terra Invicta is currently (as of build 0.2.04) built with Unity 2020.3.30f1.
- Configuration files are located in `\TerraInvicta_Data\StreamingAssets\Templates`.
- Localization files (game text) are located in `\TerraInvicta_Data\StreamingAssets\Localization\en`.
- The game's assets (textures, models, videos) are located in `\TerraInvicta_Data\StreamingAssets\AssetBundles`.
- The game's code is located in `\TerraInvicta_Data\Managed\Assembly-CSharp.dll`.

## Useful Tools
- [Unity](https://unity3d.com/get-unity/download/archive): Necessary for creating new AssetBundles to import new assets into the game.
- [FFmpeg](https://www.ffmpeg.org/download.html): Used for creating .webms to add as new councillor portraits to TI.
  - [PowerShell script](mods/tayta/anime-councilors/waifu2vid.ps1) for mass conversion of pngs to webms.
- [GIMP](https://www.gimp.org/downloads/): Open-source image editor, useful for preparing new 2D graphics to import into the game.
  - [Older versions archive](https://download.gimp.org/pub/gimp/)
- [dnSpy](https://github.com/dnSpy/dnSpy/releases): Used to view and edit the game's code.
  - Note that this is decompiled code, therefore lacks documentation and can be difficult to decipher.
- [AssetStudio](https://github.com/Perfare/AssetStudio/releases): Allows decompiling the game's AssetBundles to see what's inside.
  - Currently the only known such utility that works with Unity 2020.
- [JSON to CSV](http://www.convertcsv.com/json-to-csv.htm): Enables easier editing of the game's configuration files in spreadsheet format.

## Tutorials
- [Introduction to Importing Assets](tutorials/Custom%20Orgs.md)
- [Making New Councillor Portraits](tutorials/Councillor%20Portraits.md)

## Modding Wishlist
- Ability for code changes to somehow be retained or easily redone across new builds.
- ~~- Ability to specify new AssetBundles to be loaded into the game (as opposed to having to repackage existing ones).~~
  - This has now been officially implemented into the game and confirmed to be working by the devs, though not yet by modders. It also currently (as of 0.2.04) has a major bug preventing existing mods from being updated properly. Official tutorial also TBD.
- Ability to modify specific classes instead of the Assembly-CSharp.dll as a whole. This would mean that your mod would be easily compatible with other mods or new builds that do not make changes to or overly rely on the classes your mod modifies.
  - The current thinking is to use Bepinx / Harmony to make this possible. NotSoLoneWolf has made this work and written a draft tutorial on this repository, but it has yet to be tested by other modders.

## Future Investigation
- Ability to add new sounds (e.g. custom voice packs for councillors) to the game.
- Ability to add new UI elements to the game.

## Mod Releases
- [Tayta Malikai](mods/tayta/Tayta's%20Mods.md)
