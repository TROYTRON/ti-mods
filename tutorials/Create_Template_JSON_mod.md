# Creating a Template/JSON Mod

- [Template Structure](https://github.com/TROYTRON/ti-mods/new/main/tutorials#template-structure)
- [Setting Up The Mod Folder](https://github.com/TROYTRON/ti-mods/new/main/tutorials#setting-up-the-mod-folder)

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
