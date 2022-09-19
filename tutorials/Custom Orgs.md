Tools used:
-	Unity 2020.1.0f1
-	AssetStudio

Steps:
1. Use AssetStudio to decompile `orglogos` (found in `TerraInvicta_Data/StreamingAssets/AssetBundles`). This will give you access to the existing org logos in the game, which is necessary to keep the game’s content intact.
2. Follow the steps in this tutorial: https://learn.unity.com/tutorial/introduction-to-asset-bundles#6028bab6edbc2a750bf5b8a4, up to Step 4. In particular:
    -	Create a new Unity project.
    -	Create a new folder named “Editor” in Assets.
    -	In Editor, create a new C# script named “CreateAssetBundles”. Copy the script shown in the tutorial.
    -	Create new folders named “BundledAssets” and “StreamingAssets” in Assets.
3. ~~For safety: in the Assets folder in Unity, create the following subfolders: 
“UI/Common Art/Org Logos”. Unsure if this step is necessary.~~
    -	It turned out this is not necessary after all.
4. Import all desired logos into the Org Logos folder in Unity. Drag-and-drop from Windows Explorer is sufficient. Make sure to include the existing game logos!
5. Ctrl+A to select all the logos you just imported.
6. In the Inspector pane in Unity (should already be open on the right of the screen) you should see “[number] Texture 2Ds Import Settings” near the top. Below that is a drop-down menu marked “Texture Type”. Change this from “Default” to “Sprite (2D and UI)”. Click “Apply” further down.
a. You should now see arrows on all your imported logos.
7. Ctrl+A to select all the logos again.
8. At the bottom of the Inspector pane, you should see a drop-down menu marked “AssetBundle”. Click on the centre menu, click “New”, and type in “orglogos”.
9. In the Unity toolbar, click “Assets -> Build AssetBundles”.
10.	Navigate to your Unity project folder. In the StreamingAssets folder you should find two files: orglogos and orglogos.manifest.
11.	Navigate to `TerraInvicta_Data/StreamingAssets/AssetBundles`. Replace the existing orglogos and orglogos.manifest with your own. Always be sure to make backups before replacing game files!
12.	Configure a new or existing org in TIOrgTemplate.json (found in `TerraInvicta_Data\StreamingAssets\Templates`) to use your new logo(s). You might also want to configure `TIOrgTemplate.en` in `TerraInvicta_Data\StreamingAssets\Localization\en` (or your equivalent) to make sure your org’s name appears correctly in-game.
13.	Launch the game.
14.	Use the console command “giveorg [faction], [org]” to spawn your modified orgs. [faction] is the dataName of the faction (e.g. “ResistCouncil” for the Resistance), and [org] is the dataName of the org. Remember there is a comma between [faction] and [org]!
15.	Bask in the glory of your modded content.

If you want custom non-generic orgs to be available for spawning randomly during the campaign, you will need to update TIMetaTemplate.json. Specifically:
1. Navigate to TIMetaTemplate.json (found in `TerraInvicta_Data\StreamingAssets\Templates`).
2. Scroll down to `ModernOrgTemplates`.
3. Add an entry at the end with the dataName of your new org, as defined in TIOrgTemplate.json previously.
