# Introduction to MonoMod

## Mod Framework

1) Download BepInEx 5.1 from here: https://github.com/BepInEx/BepInEx/releases/tag/v5.1
   - Specifically, the x64 version is required for Terra Invicta.
2) Unzip the files into your Terra Invicta game directory, where the game .exe is.
   - Steam users can access this by right-clicking the game and clicking Manage->Browse local files.
   - ![image](https://user-images.githubusercontent.com/16394154/212786586-7904c37b-4f02-4a7a-a995-5f907120534d.png)
3) Run Terra Invicta once. This should generate a folder named `config` inside the BepInEx folder.
4) Download MonoModLoader 1.0 from here: https://github.com/BepInEx/BepInEx.MonoMod.Loader/releases/tag/v1.0.0.0
5) Unzip the files into your Terra Invicta game directory, merging with the existing BepInEx folder.
   - ![image](https://user-images.githubusercontent.com/16394154/212786987-e6d6fdf0-8a6a-402e-b02c-9c4fd0f02e34.png)

# Setting up your Development Environment

1) The recommended IDE is Visual Studio 2019. You can download it from https://visualstudio.microsoft.com/vs/older-downloads/. You may need to create a free account if you don't already have one. See [here](https://github.com/TROYTRON/ti-mods/blob/main/tutorials/Build%20Environment.md) for reference.
2) Once installed, create a new project `<yourmodname>.sln`. The Class Library (.NET Framework) is suggested as a template to use.
3) You will likely need to add the necessary **References** to the game's code. You should see a list of these in the right-hand panel of VS2019.
   - ![image](https://user-images.githubusercontent.com/16394154/212811900-9bc7fcb0-48c1-4de0-b8e7-f9ac51b3ccc0.png)
   - Right-click on References and click Add Reference (or go to Project->Add Reference in the top bar).
   - Use the browser to navigate to `Terra Invicta\BepInEx\core`. Ctrl+A to highlight all the DLL files there and add them all as references.
   - ![image](https://user-images.githubusercontent.com/16394154/212812475-810ef45f-f43b-4cd0-8832-ba19cc41d565.png)
   - Use the browser to navigate to `Terra Invicta\TerraInvicta_Data\Managed`. Add `Assembly-Csharp.dll`, `UnityEngine.dll`, `UnityEngine.CoreModule.dll`, `UnityEngine.UI.dll`, `UnityEngine.UIModule.dll` `Unity.TextMeshPro.dll`, and any other DLL files the editor complains about not having.
   - ![image](https://user-images.githubusercontent.com/16394154/213096527-99ea8625-155d-4cc6-8559-afd160be9909.png)
4) Navigate to Project -> <yourmodname> Properties. Change the Assembly name to follow this format: `Assembly-CSharp.<yourmodname>.mm`.
5) To create a new build of the mod's code, go to Build -> Build Solution in the top menu. If there are no errors, it should compile. If there are any errors, you will need to fix them first. Warnings can be safely ignored (usually).
6) The resulting file will be named `Assembly-CSharp.<yourmodname>.mm.dll` and will be in the `bin\Debug` folder of the project. Copy and paste it into `Terra Invicta\BepInEx\monomod`.
   - ![image](https://user-images.githubusercontent.com/16394154/213363294-dfdd117a-3b14-4457-951c-4d9b158187f3.png)
   - ![image](https://user-images.githubusercontent.com/16394154/213363141-cf5147c0-0842-442a-86e5-98e39c148c4b.png)

You can now start up Terra Invicta and test your newly developed functionality. Hopefully it doesn't crash too much.

# Using MonoMod

MonoMod is a very powerful modding framework, but it does have one major downside: the documentation is virtually non-existent.

The best way to think of using MonoMod is that you are writing shadow code that runs parallel to the game's existing structure. This shadow code then gets merged into the game's own code when it runs.

As such, you generally need to follow the structure of the game's code when writing your own patches.

## Patching Methods

First, you need to identify the method you want to patch.
You will need a decompiler to view the game's code: I recommend using dnSpy, available here: https://github.com/dnSpy/dnSpy/releases
Use it to open up `Assembly-Csharp.dll` in `Terra Invicta\TerraInvicta_Data\Managed`.

Let us use `EconomyPriorityComplete()` in `TINationState` for this example:
![image](https://user-images.githubusercontent.com/16394154/213167704-0285e8f7-40c3-42de-a850-7ce783ad2aac.png)

Take note that `TINationState` is inside the namespace `PavonisInteractive.TerraInvicta`. This is very important.
![image](https://user-images.githubusercontent.com/16394154/213167834-e18d8ed7-beef-40be-b4ef-65bb0a06d5dd.png)

First, you need to create your own copy of the class, which will be merged with the game's class at runtime.
MonoMod uses the convention `patch_classname : classname`. This means that as far as the compiler is concerned, patch_TINationState inherits from TINationState and can access variables and functions defined there.
```csharp
namespace PavonisInteractive.TerraInvicta
{
   public class patch_TINationState : TINationState
   {
   }
}
```

Now we copy the method signature **exactly** into our code.
In this case, `EconomyPriorityComplete()` has no arguments and returns void.
```csharp
namespace PavonisInteractive.TerraInvicta
{
   public class patch_TINationState : TINationState
   {
      public void EconomyPriorityComplete()
      {
         Log.Debug("First patch!");
      }
   }
}
```
This format means that our version of `EconomyPriorityComplete()` will **replace** the original game method. Any code we write will be executed instead when the method is called by the game. Thus, whenever the Economy priority completes in-game, nothing will happen in-game, and the log file will get an entry added `First patch!`.

Often we want to **preserve** the original method and simply add additional code to be executed before or after it. This is done using the convention `orig_methodname` and adding the keyword **extern** to the method signature, creating two methods like so:
```csharp
namespace PavonisInteractive.TerraInvicta
{
   public class patch_TINationState : TINationState
   {
      public extern void orig_EconomyPriorityComplete();
      public void EconomyPriorityComplete()
      {
         Log.Debug("First patch!");
         orig_EconomyPriorityComplete();
         Log.Debug("Second patch!");
      }
   }
}
```
Now, the regular code will still run, and our code will run before and after it, creating two new entries in the log file.

In general, it is recommended to preserve the original method if you only want to make small alterations to how the game works, as this maintains better compatibility with any future updates.

To patch private methods, simply use the same format with the keyword `private` instead of `public`. This also applies to methods marked `internal` or `static`.

The exception is with methods marked `override`; only the patched version needs to be marked `override`, not the original.

## Patching Getters

Getters are a form of method that are used to set the value of a variable. Here is an example:
![image](https://user-images.githubusercontent.com/16394154/213171737-695a6822-6869-4f87-a9c2-e6bf1b92cd42.png)

Internally, the code considers this to be a regular method named `public float get_economyPriorityInequalityChange()`. Thus it can be patched and modded like any normal method.

Simply use the same patching methods to modify the method's return value, like so:
```csharp
namespace PavonisInteractive.TerraInvicta
{
   public class patch_TINationState : TINationState
   {
      public float get_economyPriorityInequalityChange()
      {
         return 0.1f;
      }
   }
}
```

## Accessing and Modifying Private Variables

As stated above, our patch classes inherit from the game's existing classes, allowing us to access variables and methods originally defined there. However, variables and methods marked with the **private** keyword cannot be accessed from any other class than the one they are defined in. This includes child classes of the original class like our patch classes.

(As a general programming side note, if you want to make methods accessible by child classes as well, you should use the keyword **protected** instead.)

In order to make these accessible, we use the `[MonoModIgnore]` annotation on a desired variable, copying it exactly as it appears in the code:
![image](https://user-images.githubusercontent.com/16394154/213176198-f79c3c55-8b10-47af-9104-37aaed7c2780.png)
```csharp
namespace PavonisInteractive.TerraInvicta
{
    public class patch_TIArmyState : TIArmyState
    {
        [MonoModIgnore][SerializeField] private bool gameStateSubjectCreated;
        
        public extern void orig_PostGameStateCreateInit_OnCreationOnly_1();
        public override void PostGameStateCreateInit_OnCreationOnly_1()
        {
            orig_PostGameStateCreateInit_OnCreationOnly_1();
            if (createdFromTemplate && !gameStateSubjectCreated)
            {
                armyType = template.armyType;
            }
        }
    }
}        
```
Essentially what this does is create a local version of `gameStateSubjectCreated` which we can then access and reference in our code. The `[MonoModIgnore]` annotation then instructs the compiler to ignore it, so that it does not interfere with the _actual_ `gameStateSubjectCreated` that already exists in the game's code.

One situation that can occur is if you have subclasses of a patched class which also want to use the private variable or method. Because the variable or method is still private in the game's code, it will throw an error when you try to access it during runtime. Therefore, we need to use the `[MonoModPublic]` annotation to make the game consider it a public variable or method instead.
`[MonoModIgnore][MonoModPublic] private int get_baseTechLevel() { return 0; }`
This is also a good example of how to handle methods which have non-void return values. Simply define an arbitrary return value as a placeholder. As long as the `[MonoModIgnore]` annotation is present, this placeholder will not be used by the game and the proper return value will be used as normal.

If you want to actually edit a private variable instead of just accessing its value, you will need to **wrap** it using a **ref** wrapper.
```csharp
namespace PavonisInteractive.TerraInvicta
{
    public class patch_TINationState : TINationState
    {
         [MonoModIgnore] [SerializeField] private Dictionary<PriorityType, float> _accumulatedInvestmentPoints;

         public ref Dictionary<PriorityType, float> ref_accumulatedInvestmentPoints()
         {
            return ref _accumulatedInvestmentPoints;
         }
    }
}
```
You can then call the wrapper method `ref_accumulatedInvestmentPoints()` whenever you want to access or modify the value of `_accumulatedInvestmentPoints`.

## Patching Enums

Enums (short for "enumerations") are a sort of special list of constants, which map a collection of names to integer values.

An example of a commonly used enum in TI:
```csharp
public enum TechCategory
{
    Materials,
    SpaceScience,
    Energy,
    LifeScience,
    MilitaryScience,
    InformationScience,
    SocialScience,
    Xenology
}
```
The `Materials` type corresponds to an int of 0, `SpaceScience` corresponds to 1, etc. Note that you can specify an integer for each value, e.g. `MilitaryScience = 17`.
Enums are usually used when you know that there will be a fixed number of types or categories for something.

MonoMod can be used to extend or replace the values in this list, enabling us to pass custom enum values into functions which use enums as variables. To patch this enum with a new category, let's say `Infrastructure`, we would type:

```csharp
public enum patch_TechCategory : ushort
{
    Infrastructure = 20,
}
```
You **must** define an integer value for the **first** new item when patching. Any value can be used, including arbitrarily large ones like 5001, as long as it is outside of the normal range used by the vanilla game.

To replace the enum's contents entirely, we use the `[MonoModEnumReplace]` annotation:
```csharp
[MonoModEnumReplace]
public enum patch_TechCategory : ushort
{
    Infrastructure = 0,
}
```
Be aware that many enums in TI are defined _outside_ of a namespace, so your patch will have to be outside the namespace as well.

Many methods in TI make use of switch statements centred on a particular enum type as the variable, which cause different code to be executed depending on which enum value the variable has. For example:
```csharp
public static string PathTechCategoryIcon(TechCategory category)
	{
		switch (category)
		{
		case TechCategory.Materials:
			return TemplateManager.global.pathMaterialsIcon;
		case TechCategory.SpaceScience:
			return TemplateManager.global.pathSpaceScienceIcon;
		case TechCategory.Energy:
			return TemplateManager.global.pathEnergyIcon;
		case TechCategory.LifeScience:
			return TemplateManager.global.pathLifeScienceIcon;
		case TechCategory.MilitaryScience:
			return TemplateManager.global.pathMilitaryScienceIcon;
		case TechCategory.InformationScience:
			return TemplateManager.global.pathInformationScienceIcon;
		case TechCategory.SocialScience:
			return TemplateManager.global.pathSocialScienceIcon;
		case TechCategory.Xenology:
			return TemplateManager.global.pathXenologyIcon;
		default:
			return string.Empty;
		}
	}
```
When patching in new enum values, we will usually need to define new return values for them in these sorts of methods as well. In order to avoid having to duplicate the original code, we can use the orig_methodName process, like so:
```csharp
public static extern string orig_PathTechCategoryIcon(TechCategory category);
public static string PathTechCategoryIcon(TechCategory category)
{
    switch ((patch_TechCategory)category)
    {
        case patch_TechCategory.Magic: return TemplateManager.global.pathEnergyIcon; // your path here
        default:
            return orig_PathTechCategoryIcon(category);
    }
}
```

## Adding Variables to Existing Classes

Adding variables to existing classes is very easy. Simply define the new variables anywhere in your patch class, and they will be considered part of the original class when the game runs.

Be aware that the compiler does not recognize that your patch class is "supposed" to be the same as the original class, but still considers it a child class. This means that attempting to access the new variables from methods which use the original class as an argument does not work.

Therefore, the best practice when writing your patch is to first recast the original class to the type of the patch class, like so:
`patch_TINationState nation_PVC = nation as patch_TINationState;`
You can then reference the new variables defined in patch_TINationState through nation_PVC.

Any variables added this way will initialize with null values; attempting to specify an initial value will not work. Normally, this is not a problem, as you will usually assign a value to the variable in the course of using the class anyway. But there can be situations where you want to ensure that a specific value will be initialized with a given variable.

The main example of this situation is TI-PVC's additions to TIGlobalConfig.cs (truncated for brevity):
```csharp
public class patch_TIGlobalConfig : TIGlobalConfig
{
    public bool skipIntro;

    [MonoModIgnore] public patch_TIGlobalConfig() : base() { }
    [MonoModOriginal] public extern void orig_TIGlobalConfig();
    [MonoModConstructor] public void TIGlobalConfig()
    {
        orig_TIGlobalConfig();

        skipIntro = true;
    }
}
```
Essentially, you are first defining the new variable as part of the existing TIGlobalConfig, `skipIntro`.
Then we are modifying what is called the **constructor** of TIGlobalConfig, which is the code that creates an instance of TIGlobalConfig when the game is run.
What this code does is first run the original constructor so that all the vanilla variables in TIGlobalConfig are initialized as normal, _then_ we initialize `skipIntro` with our desired value of `true`.
