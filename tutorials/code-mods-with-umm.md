# Code Mods in Terra Invicta

The recommended way to alter code functionality in Terra Invicta is by using Unity Mod Manager and the Harmony functionality built in to it. This requires [Unity Mod Manager](https://www.nexusmods.com/site/mods/21) be installed for both the mod developer and the mod user.

Below are some links to general information on using these tools:
- [Nexus: How to create mod for unity game](https://wiki.nexusmods.com/index.php/How_to_create_mod_for_unity_game)
- [Nexus: How to render mod options](https://wiki.nexusmods.com/index.php/How_to_render_mod_options_(UMM))
- [Nexus: How to make mod updates](https://wiki.nexusmods.com/index.php/How_to_make_updates_(UMM))
- [Harmony: Guide and API](https://harmony.pardeike.net/index.html)
- [Local: Guide to setting up Build Environment](Build%20Environment.md)

## Terra Invicta Example

The following example mod alters the `TICouncilorState.projectContributionString` property getter to return a numeric value string instead of the `+`, `++`, `+++` in the base game.

<details><summary>FULL CODE (MyTerraInvictaMod.cs)</summary>
<p>
  
```cs
using HarmonyLib;
using UnityEngine;
using UnityModManagerNet;
using System.Reflection;
using PavonisInteractive.TerraInvicta;

namespace MyTerraInvictaMod
{
    public class Main
    {
        public static bool enabled;
        public static UnityModManager.ModEntry mod;
        public static Settings settings;

        //Boilerplate code, the entry point to the mod
        static bool Load(UnityModManager.ModEntry modEntry)
        {
            //
            var harmony = new Harmony(modEntry.Info.Id);
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            settings = Settings.Load<Settings>(modEntry);
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;

            mod = modEntry;
            modEntry.OnToggle = OnToggle;
            return true;
        }

        //Boilerplate code, called when the user toggles the mod on/off in UMM in-game
        static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            enabled = value;
            return true;
        }

        //Boilerplate code, draws the configurable settings in the UMM
        static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            settings.Draw(modEntry);
        }

        //Boilerplate code, saves settings changes to the xml file
        static void OnSaveGUI(UnityModManager.ModEntry modEntry)
        {
            settings.Save(modEntry);
        }
    }

    //Settings class to interface with Unity Mod Manager
    public class Settings : UnityModManager.ModSettings, IDrawable
    {
        //TODO: switch to localization once it is available
        [Draw("Example Int Setting", Collapsible = true)] public int exampleIntSetting = 3;
        [Draw("Example Bool Setting", Collapsible = true)] public bool exampleBoolSetting = true;

        //Boilerplate code to save your settings to a Settings.xml file when changed
        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }

        //Hook to allow to do things when a value is changed, if you want
        public void OnChange()
        {
        }
    }

    //Example Harmony patch
    // Replaces the default "+" "++" "+++" display for Engineering Projects with a number
    [HarmonyPatch(typeof(TICouncilorState), nameof(TICouncilorState.projectContributionString), MethodType.Getter)]
    static class PreciseEngineeringProjectInfo
    {
        static void Postfix(ref string __result, TICouncilorState __instance)
        {
            FileLog.Log("Entering TICouncilorState.projectContributionString - Main.enabled = " + Main.enabled.ToString());
            if (Main.enabled && Main.settings.exampleBoolSetting)
            {
                var value = __instance.GetMonthlyIncome(FactionResource.Projects);
                FileLog.Log("Projects income = " + value.ToString());
                if (__result == "-") return;
                FileLog.Log("Got past - check");
                __result = value.ToString("N0");
            }
        }
    }
}
```
  
</p>
</details>

<details><summary>MODFILE (ModFile.json)</summary>
<p>
  
```json
{
	"Id": "MyTerraInvictaMod",
	"DisplayName": "My Terra Invicta Mod",
	"Title": "MyTerraInvictaMod",
	"Author": "Author",
	"Version": "1.0.0",
	"Requirements": [],
	"ManagerVersion": "0.24.0.0",
	"AssemblyName": "MyTerraInvictaMod.dll",
	"EntryMethod": "MyTerraInvictaMod.Main.Load",
	"ModURL": "URL",
	"HomePage": "NexusModsURL",
	"Description": "Example Description"
}
```
  
</p>
</details>

Build the mod dll

![image](https://user-images.githubusercontent.com/11687023/195727600-35a34df5-f25b-4dcd-8d21-ee5846212821.png)

For VS 2022 users, use [this guide](https://learn.microsoft.com/en-us/visualstudio/ide/how-to-add-or-remove-references-by-using-the-reference-manager?view=vs-2022) on Adding References. Which references are needed will vary based on mod functionality, but the ones above are a good starting point.

Files placed in Terra Invicta Mods folder

![image](https://user-images.githubusercontent.com/11687023/195728189-af1aad5e-3a64-4fec-ad9c-50664f505e10.png)

Unity Mod Manager view of mod in-game

![image](https://user-images.githubusercontent.com/11687023/195728985-72be1711-499e-460a-b512-9360956fa0aa.png)

In-game effect of mod

![image](https://user-images.githubusercontent.com/11687023/195729460-b1e5a536-069c-4fdd-849a-54e4064f866e.png)


## Terra Invicta Architecture

TODO
