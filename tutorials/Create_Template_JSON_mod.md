# Creating a Template/JSON Mod

- [Template Structure](#template-structure)
- [Setting Up The Mod Folder](#setting-up-the-mod-folder)
- [Creating The JSON Mod File](#creating-the-json-mod-file)
- [Testing The Mod](#testing-the-mod)
- [Upload your Mod](#upload-your-mod)
- [Known Issues](#known-issues)

## Template Structure
Terra Invicta stores static / configuration data in the \Terra Invicta\TerraInvicta_Data\StreamingAssets\Templates folder. Each file in this folder corresponds to a Template class in the game assembly. Each file is organized as [JSON](https://www.json.org/json-en.html) formatted data. It is the serialization of an array of class instances.

For example, here is the definition of `TIBilateralTemplate.cs`

```cs
public class TIBilateralTemplate : TIDataTemplate
{
	// public fields are read
	public BilateralRelationType relationType;
	public string federation;
	public string nation1;
	public string nation2;
	public string region1;
	public string region2;
	public string projectUnlockName;
	public bool capitalClaim;
	public bool initialOwner;
	public bool initialColony;
	public bool friendlyOnly;

	// private fields are not read and are for temporary code use only
	private bool _currentScenarioSet;
	private bool _inCurrentScenario;
}
```

The corresponding file _always_ has the same name as the class, appended with the .json suffix. So in the Templates folder, we look for `TIBilateralTemplate.json`. The first few entries are shown below.

```json
[
  {
    "dataName": "WarRUSUKR",
    "relationType": "War",
    "nation1": "RUS",
    "nation2": "UKR",
    "federation": "",
    "region1": "",
    "region2": "",
    "projectUnlockName": "",
    "capitalClaim": null,
    "initialOwner": null,
    "initialColony": null,
    "friendlyOnly": null
  },
  {
    "dataName": "FederationBASEUAFed",
    "relationType": "Federation",
    "nation1": "BAS",
    "nation2": "",
    "federation": "EUAFed",
    "region1": "",
    "region2": "",
    "projectUnlockName": "",
    "capitalClaim": null,
    "initialOwner": null,
    "initialColony": null,
    "friendlyOnly": null
  },
```

The opening `[` and closing `]` indicate that this is an array/list of class instances.

Note that the `dataName` field is defined in the JSON file but not in the TIBilateralTemplate. This field is inherited from the base `TIDataTemplate.cs` class, which means that _every_ template type will include the following fields.

```cs
public class TIDataTemplate
{
	public string dataName { get; protected set; }
	public string friendlyName { get; protected set; }
}
```

Also note that not all allowed fields need be defined in the JSON file. Any field not defined will retain the default value defined in the C# class. Any field defined in the JSON class that does not exist in the C# class will be ignored by the serializer.

## Setting Up The Mod folder

A mod folder resides in \Terra Invicta\Mods\Disabled or \Terra Invicta\Mods\Enabled. The game will move a mod folder between these two when the player enables or disables a mod using the in-game interface.

Each mod requires a `ModInfo.json` file. This file allows description of all possible information a mod may need to function, but not all fields are required for a mod to work. For a mod that modifies only JSON Template files, the following contents are sufficient. If Unity Mod Manager is not installed, the following field are available. The `Title` field and the name of your Mod folder must be identical for your mod to work.

```json
	"Title": "Your Mod Name",
	"Author": "Optional",
	"ModURL": "Currently unused",
	"Description": "Short description that shows up in-game and is uploaded to Steam Workshop"
```

If Unity Mod Manager is installed, an expanded number of fields are available. (Reminder : any field in JSON that does not exist in the C# source is ignored, so including the below field will not cause problems if UMM is not installed)

NOTE: if UMM is installed it will be complaining that there is no EntryPoint
specified. You can ignore it for JSON modes, the in-game mod manager will pick
it up regardless. To suppress the warning, you would need to turn your code into
a proper [code mod](code-mods-with-umm.md) - you don't have to add logic, but
you need to create an entry point.

```json
{
	"Id": "Nexus Mods / Unity Mod Manager ID",
	"DisplayName": "Pretty name for Unity Mod Manager",
	"Title": "Your Mod Name",
	"Author": "Optional",
	"ModURL": "Currently unused",
	"Requirements": [], //array of other mods your mod requires, by Id
	"Version": "Optional - useful for Nexus Mod users in conjunction with Homepage so Unity Mod Manager can display if an update is available",
	"HomePage": "Optional - will link to your mod's website if Unity Mod Manager is installed"
	"Description": "Short description that shows up in-game and is uploaded to Steam Workshop"
}
```

For this example, we will include some simple changes to the `TIBilateralTemplate.json` that will start a new campaign with Alaska owned by Canada instead of the USA. To accomplish this only the following files are required in the mod folder

![image](https://user-images.githubusercontent.com/11687023/195460805-dbf2eb70-feb0-475a-8839-4bfb03d96c16.png)

[ModInfo file](https://github.com/TROYTRON/ti-mods/blob/main/tutorials/tutorial-files/template-json-mod-examples/TIBilateralTemplate.json)

## Creating The JSON Mod File

Mod JSON files need only include the fields that they are changing with respect to the base game's definition (including the `dataName` field for matching) and any _new_ entries that will include a _new_ `dataName`

Continuing our example mod, the first piece we will look at is the existing ownership of the Alaska region by USA at game start. This is defined in the base game's `TIBilateralTemplate.json` with the entry

```json
  {
    "dataName": "ClaimUSAAlaska",
    "relationType": "Claim",
    "nation1": "USA",
    "nation2": "",
    "federation": "",
    "region1": "Alaska",
    "region2": "",
    "projectUnlockName": "",
    "capitalClaim": null,
    "initialOwner": true,
    "initialColony": null,
    "friendlyOnly": null
  },
```

The only thing we want to change is that the initial owner is false. This will preserve a claim on the Alaska region by the USA, but it will not start a new campaign as a part of the USA. To accomplish this, we add to our mod's `TIBilateralTemplate.json`

```json
  {
    "dataName": "ClaimUSAAlaska",
    "initialOwner": false,
  },
```

Canada starts with no claim or relation to the Alaska region, so we have to create a new entry, with a new dataName, filling out all of the relevant information. I did so by copying the information from an existing claim (British Columbia) and modifying it.

```json
  {
    "dataName": "ExampleMod_ClaimCANAlaska",
    "relationType": "Claim",
    "nation1": "CAN",
    "nation2": "",
    "federation": "",
    "region1": "Alaska",
    "region2": "",
    "projectUnlockName": "",
    "capitalClaim": null,
    "initialOwner": true,
    "initialColony": null,
    "friendlyOnly": null
  },
```

Put together, the full mod `TIBilateralTemplate.json` files reads

```json
[
  {
    "dataName": "ClaimUSAAlaska",
    "initialOwner": false,
  },
  {
    "dataName": "ExampleMod_ClaimCANAlaska",
    "relationType": "Claim",
    "nation1": "CAN",
    "nation2": "",
    "federation": "",
    "region1": "Alaska",
    "region2": "",
    "projectUnlockName": "",
    "capitalClaim": null,
    "initialOwner": true,
    "initialColony": null,
    "friendlyOnly": null
  },
]
```

Once this file is saved, the mod is complete.
- [ModInfo.json](https://github.com/TROYTRON/ti-mods/blob/main/tutorials/tutorial-files/template-json-mod-examples/TIBilateralTemplate.json)
- [TIBilateralTemplate.json](https://github.com/TROYTRON/ti-mods/blob/main/tutorials/tutorial-files/template-json-mod-examples/TIBilateralTemplate.json)

## Testing The Mod

* Launch the game.
* Navigate to `Mods` menu.
* Make sure to `Enable` the mod if you created it in `Disabled` folder.
* Make sure "Use Mods" check box is checked.
* Relaunch the game.
* When the game starts it will make a backup copy of the original
  `TIBilateralTemplate.json` and then will merge in your mod's changes to the
  file in the Templates folder.  
  ![image](https://user-images.githubusercontent.com/11687023/195463092-dc9abea8-99a3-4f40-9d79-2781cbbc017a.png)
* You can examine the changed `TIBilateralTemplate.json` int the Templates
  folder to verify that the "ClaimUSAAlaska" has `"initialOwner": false` and
  Canada has a new claim added.
* Launch a new campaign to observe the modded-in regional claim and initial
  ownership.  
  ![image](https://user-images.githubusercontent.com/11687023/195463605-0a5aa5ea-74bd-417c-b2c9-9db119016963.png)

## Upload your Mod

After your mod is complete, you can
[upload your mod to Steam Workshop](https://github.com/TROYTRON/ti-mods/blob/main/tutorials/Uploading%20and%20Updating%20Workshop%20Mod.md).

## Known Issues

* If mod directory contains a json file that doesn't match the name of an
  existing one in the game's Templates folder (with the exception for
  `ModInfo.json`), it will crash. If you have a code-mode that needs to load a
  new template type, you need to alternate the extension of the file and load
  it manually.
* `TIMapGroupVisualizerTemplate.json` and `TISpaceFleetTemplate.json` contain
  invalid json, and will likely cause a crash if tried modding.
* Patching arrays performs field-wise merge for entries on the matching indices.
  The only presently known ways to "extend" the list with a json mode are to
  either specify the list in its entirety or to specify empty stubs for already
  presented entries. Both ways likely make multiple mods patching the same list
  incompatible. You may create a code mod to do such patching.
  <TODO(dkoiman): add a guide for in-code modifications of templates>
