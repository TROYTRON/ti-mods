# Terra Invicta Build Environment
This example sets up a Build Environment using Visual Studio. Other code development environments may work. As details of how to set those up are learned they may be added

## Install Visual Studio Community 2019
* Go to the [Microsoft Visual Studio website](https://visualstudio.microsoft.com/) and head into the Downloads section
* Scroll down to the bottom and find the section "Older Downloads". Select the 2019 section, and Download the "Visual Studio 2019 and other Products"
* In the Workloads section,under the "Desktop & Mobile" area select ".NET desktop development" and under the "Gaming" area select "Game development with Unity"
![Visual Studio Community Install Options](https://user-images.githubusercontent.com/11687023/190953860-2a0c0be6-428d-44fd-8a4c-516dc73caa3a.PNG)


## Install Unity Mod Manager 2.0.2
* Go to the [Nexus Mods Unity Mod Manager website](https://www.nexusmods.com/site/mods/21/)
* Select the "Files" tab, and scroll down to "OLD FILES" section and sort them by version in Descending order
* Select the version 0.24.2f. 
* Download this and install it (it is only necessary to decompress it into the desired location)
* Modify the UMM config file "UnityModManagerConfig.xml" to add Terra Invicta to the list of games. At the bottom of this file, add the xml section
```xml
	<GameInfo Name="Terra Invicta">
		<Folder>Terra Invicta</Folder>
		<ModsDirectory>Mods/Enabled</ModsDirectory>
		<ModInfo>ModInfo.json</ModInfo>
		<GameExe>TerraInvicta.exe</GameExe>
		<EntryPoint>[UnityEngine.UIModule.dll]UnityEngine.Canvas.cctor:Before</EntryPoint>
		<StartingPoint>[Assembly-CSharp.dll]StartMenuController.Start:Before</StartingPoint>
		<UIStartingPoint>[Assembly-CSharp.dll]StartMenuController.Start:After</UIStartingPoint>
		<MinimalManagerVersion>0.24.0</MinimalManagerVersion>
	</GameInfo>
  ```
  ![image](https://user-images.githubusercontent.com/11687023/190954269-9e548bcd-ccae-40a4-aa93-b6d9737fdb53.png)
  (Note that this is only necessary until a version of Unity Mod Manager with the needed Terra Invicta config data is available through Nexus Mods)  
  
* After this has been added, run the UnityModManager.exe
  * Select Game = Terra Invicta
  * Select the Folder where Terra Invicta is installed
  * Select Installation method = DoorstopProxy
 
![UnityModManager_PreInstall](https://user-images.githubusercontent.com/11687023/190954427-0093c2d3-43b7-4313-8cb8-d029e6e4812b.PNG)
![UnityModManager_PostInstall](https://user-images.githubusercontent.com/11687023/190954435-838b63fd-7881-4cbd-9b57-fa32caf7294e.PNG)

Unity Mod Manager can also be used to download and update Terra Invicta mods available on Nexus Mods.

## Install Unity (optional)
* Required version is 2020.3.30f1 (64-bit). Last I checked the automatic download of Visual Studio Community 2019 with Unity install was broken (likely because Visual Studio has updated current version to 2022).
* TBD

## Create your first project in Visual Studio
* Open Visual Studio
* Select File / New / Project
* Set options C#, All platforms, Library, choose "Class Library (.NET Framework) C#
* Select "Next"
![image](https://user-images.githubusercontent.com/11687023/190954831-08499f01-e039-4ab8-b0e7-83aac754097a.png)
* Set your Project name and Solution name to your mod's name
* Select a location where your mod project will be created
* Select Framework ".NET Framework 4.6"
* Select "Create"
![image](https://user-images.githubusercontent.com/11687023/190955043-dfdd6892-a58f-4b9e-adb3-309f1c204170.png)
* Rename the default "Class1.cs" to match your mod name (right-click on Class1.cs in Solution explorer and select rename)
* Add at least the following references (right-click on References in Solution explorer and select "Add Reference..."
![image](https://user-images.githubusercontent.com/11687023/190955894-2b11250e-171d-4b9d-8e85-75e908580045.png)
* Insert the following boilerplate initial code
```cs
using HarmonyLib;
using UnityEngine;
using UnityModManagerNet;
using System.Reflection;
using PavonisInteractive.TerraInvicta;

namespace MyTerraInvictaMod
{
    public class MyTerraInvictaMod
    {
        public static bool enabled;
        public static UnityModManager.ModEntry mod;

        //This is standard code, you can just copy it directly into your mod
        static bool Load(UnityModManager.ModEntry modEntry)
        {
            var harmony = new Harmony(modEntry.Info.Id);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            mod = modEntry;
            modEntry.OnToggle = OnToggle;
            return true;
        }

        //This is also standard code, you can just copy it
        static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            enabled = value;
            return true;
        }
    }
}
```

More information on modding using Harmony can be found [here](https://harmony.pardeike.net/)
