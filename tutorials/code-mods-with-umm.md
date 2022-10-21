# Code Mods in Terra Invicta

The recommended way to alter code functionality in Terra Invicta is by using Unity Mod Manager and the Harmony functionality built in to it. This requires [Unity Mod Manager](https://www.nexusmods.com/site/mods/21) be installed for both the mod developer and the mod user.

Below are some links to general information on using these tools:
- [Nexus: How to create mod for unity game](https://wiki.nexusmods.com/index.php/How_to_create_mod_for_unity_game)
- [Nexus: How to render mod options](https://wiki.nexusmods.com/index.php/How_to_render_mod_options_(UMM))
- [Nexus: How to make mod updates](https://wiki.nexusmods.com/index.php/How_to_make_updates_(UMM))
- [Harmony: Guide and API](https://harmony.pardeike.net/index.html)
- [Local: Guide to setting up Build Environment](Build%20Environment.md)

## Coding Language

Coding in Terra Invicta is done using C#. If you are new to coding in C#, here are some resources for learning the language:
- [Learn C# at Microsoft.com](https://dotnet.microsoft.com/en-us/learn/csharp)
- [Learn How to Code in C# for Beginners at Unity3d](https://unity.com/how-to/learning-c-sharp-unity-beginners)

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

## Data Architecture for Terra Invicta

Terra Invicta's design pattern is that **Templates** hold static data, and are only instanced by deserializing JSON files. Any data values that don't change during a campaign are best placed in a template and configured through JSON so that the data is easily visible/moddable and changes adjust the ongoing campaign.

The other core data structure is the **GameState**, which holds dynamic data. It holds all data that is important to the overall game state. Any dynamic data that must be preserved through a Save/Load cycle must be stored in a GameState. The savefile consists of a direct serialization of all GameState class instances.

### Data Templates

The base class for data templates is TIDataTemplate. TIDataTemplate has the following structure, which affects all template classes derived from it

```cs
public class TIDataTemplate
{
	public string dataName { get; protected set; }		// key field used to store/retrieve a template
	public string friendlyName { get; protected set; }	// display-only field

	public string displayName {get;}			// getter to retrieve localized dataName
	public string displayNameCurrent();			// method access to above getter
	public TIDataTemplate();				// base constructor, used by deserializer
	public TIDataTemplate(string templateName);		// constructor that allows specifying a dataName for code-based template construction
	public virtual TIGameState CreateGameState();		// polymorphic method that allows loaded templates to create associated gamestates
	public virtual bool IsValid(out string error);		// unit-test code for templates to validate the data loaded into them
}
```

Template classes hold **static** data. They may also contain properties or methods that compute values based on passed parameters and/or local fields. No data stored in a Template class will be serialized into the savefile, so data in a Template is only "safe" between game sessions only in that the same data is re-loaded from the JSON files. Best practice is that static data should be referenced from a Template so that changes to the JSON files will be reflected in an ongoing campaign.

Template class instances are in practice created from the JSON files located in the Templates folder. Each file in the Templates folder is the JSON serialization of an array of Template class instances. The *type* of the template is the name of the JSON file. For example, *TIOrgTemplate.json* contains an array of serialized classes of type `TIOrgTemplate`. For each element in the array, the serializer calls the constructor for the Template class (which will set any fields to the default value as defined in the class definition). Each entry in the JSON class is matched against (public) fields defined in the class. If the field exists in the class, it is overwritten. If the field does not exist, the entry is discarded. Any field not overwritten retains its default value.

Template instances are commonly accessed through their associated GameState (if they have one) via the TIGameState method:
```cs
public virtual T GetMyTemplate<T>() where T : TIDataTemplate
```

The other common access method for Template instances is through the TemplateManager. All instanced Templates are stored in the TemplateManager, and can be accessed through the following methods:

```cs
public class TemplateManager
{
	// getter for the special-case TIGlobalConfig singleton Template
	public static TIGlobalConfig global {get;}	
	
	// allows iterating through all Templates of a particular class. optional parameter includes child classes of the specified Template type
	public static IEnumerable<T> IterateByClass<T>(bool allowChild = true) where T : TIDataTemplate;
	
	// Retrieves an array of all templates of the specified template type
	public static T[] GetAllTemplates<T>(bool allowChild = true) where T : TIDataTemplate;
		
	// finds a specific template keyed off the dataName of the template
	public static T Find<T>(string templateName, bool allowChild = false) where T : TIDataTemplate;
}
```

A typical use of Template.IterateByClass<T>
```cs
foreach (TIOrgTemplate orgTemplate in from org in TemplateManager.IterateByClass<TIOrgTemplate>(true)
	where org.randomized
	select org)
{ ... }
```

A typical usage of Template.GetAllTemplates<T>
```cs
List<TIShipHullTemplate> hulls = (from x in TemplateManager.GetAllTemplates<TIShipHullTemplate>(true)
	where !x.alien || this.fullDesignerTest
	select x) : base.activePlayer.allowedShipHulls).OrderBy((TIShipHullTemplate x) => x.volume_m3).ToList<TIShipHullTemplate>();
```

A typical usage of Template.Find<T>
```cs
TIProjectTemplate projectTemplate = TemplateManager.Find<TIProjectTemplate>(this.selectedProjectEntry);
```
	
### GameStates
	
TODO
