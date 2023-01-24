Tools used:
-	Unity 2020.3.30f1

This tutorial up to step 8) will also generally introduce you to importing new assets into the game.

Steps:
1. Follow the steps in this tutorial: https://learn.unity.com/tutorial/introduction-to-asset-bundles#6028bab6edbc2a750bf5b8a4, up to Step 4. In particular:
    -	Create a new Unity project.
    -	Create a new folder named “Editor” in Assets.
    -	In Editor, create a new C# script named “CreateAssetBundles”. Copy the script shown in the tutorial.
    -	Create new folders named “BundledAssets” and “StreamingAssets” in Assets.
2. Import all desired logos into Unity. Drag-and-drop from Windows Explorer is sufficient.
3. Ctrl+A to select all the logos you just imported.
4. In the Inspector pane in Unity (should already be open on the right of the screen) you should see “[number] Texture 2Ds Import Settings” near the top. Below that is a drop-down menu marked “Texture Type”. Change this from “Default” to “Sprite (2D and UI)”. Click “Apply” further down.
    -	You should now see arrows on all your imported logos.
5. Ctrl+A to select all the logos again.
6. At the bottom of the Inspector pane, you should see a drop-down menu marked “AssetBundle”. Click on the centre menu, click “New”, and type in “orglogos”.
7. In the Unity toolbar, click “Assets -> Build AssetBundles”.
8. Navigate to your Unity project folder. In the StreamingAssets folder you should find two files: orglogos and orglogos.manifest. Copy these files.
9. Navigate to your game directory, and then to the `Mods\Enabled\ folder. Create a new folder, let's call it "NewOrgs", and paste the above two files in there.
10.	Navigate to TIOrgTemplate.json (found in `TerraInvicta_Data\StreamingAssets\Templates`). Copy and paste this file into `NewOrgs`.
11.	Remove all the entries in NewOrgs\TIOrgTemplate.json, and then add new entries with your org's desired stats.
    -	I strongly recommend using [JSON2CSV](http://www.convertcsv.com/json-to-csv.htm) to convert the JSON to a CSV, make your changes there, then convert it back using [CSV2JSON](http://www.convertcsv.com/csv-to-json.htm). Trust me, it's much easier.
12. Configure `TIOrgTemplate.en` in `TerraInvicta_Data\StreamingAssets\Localization\en` (or your equivalent) to make sure your org’s name appears correctly in-game.
13.	Launch the game.
14.	Use the console command “giveorg [faction], [org]” to spawn your modified orgs. [faction] is the dataName of the faction (e.g. “ResistCouncil” for the Resistance), and [org] is the dataName of the org. Remember there is a comma between [faction] and [org]!
15.	Bask in the glory of your modded content.

If you want custom non-generic orgs to be available for spawning randomly during the campaign, you will need to update TIMetaTemplate.json. Specifically:
1. Navigate to TIMetaTemplate.json (found in `TerraInvicta_Data\StreamingAssets\Templates`).
2. Scroll down to `ModernOrgTemplates`.
3. Add an entry at the end with the dataName of your new org, as defined in TIOrgTemplate.json previously.
