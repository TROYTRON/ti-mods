# Terra Invicta Build Environment
This example sets up a Build Environment using Visual Studio. Other code development environments may work. As details of how to set those up are learned they may be added

## Install Visual Studio Community 2019
* Go to the [Microsoft Visual Studio website](https://visualstudio.microsoft.com/)
* Currently to access Visual Studio Community 2019 you must have a Microsoft account and be logged in
* Once you are logged in, select the Downloads tab and search for "Visual Studio Community 2019"
![Visual Studio Community 2019 Download](https://user-images.githubusercontent.com/11687023/194719522-262c64a5-0ed1-40a3-b05d-4ef6901d4edb.png)
* In the Workloads section,under the "Desktop & Mobile" area select ".NET desktop development" and under the "Gaming" area select "Game development with Unity"
![Visual Studio Community Installer](https://user-images.githubusercontent.com/11687023/194719674-c7cf8f5f-0a3e-4f7d-8b44-e86b589bea4f.png)
![Visual Studio Community Install Options](https://user-images.githubusercontent.com/11687023/194719784-53a2ad2d-421e-42e0-8f3a-1c2e3d23e68f.png)


## Install Unity Mod Manager
* Go to the [Nexus Mods Unity Mod Manager website](https://www.nexusmods.com/site/mods/21/)
* Select the "Files" tab, any version 0.25.0 or later
* Download this and install it (it is only necessary to decompress it into the desired location)
* Run the UnityModManager.exe
  * Select Game = Terra Invicta
  * Select the Folder where Terra Invicta is installed
  * Select Installation method = DoorstopProxy
 
![UnityModManager_PreInstall](https://user-images.githubusercontent.com/11687023/190954427-0093c2d3-43b7-4313-8cb8-d029e6e4812b.PNG)
![UnityModManager_PostInstall](https://user-images.githubusercontent.com/11687023/190954435-838b63fd-7881-4cbd-9b57-fa32caf7294e.PNG)

Unity Mod Manager can also be used to install and update Terra Invicta mods available on Nexus Mods.

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
