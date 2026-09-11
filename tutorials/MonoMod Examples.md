# MonoMod worked examples

Adapted from Tayta/TROYTRON's tutorial. These examples preserve method/getter patches, private-member access, enum handling, and constructor initialization. Start with [loader setup and compatibility](MonoMod%20Guide.md). Check exact signatures and namespaces in your installed assemblies; these snippets have not been compiled or run against the handbook baseline.

## Patching Methods

First, you need to identify the method you want to patch.
You will need a decompiler to view the game's code: use [dnSpyEx or ILSpy](../docs/tools.md).
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

Getters return a property value; setters assign one. Here is an example:
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
Essentially what this does is create a local version of `gameStateSubjectCreated` which we can then access and reference in our code. The compiler uses this declaration for type checking. The MonoMod patcher interprets `[MonoModIgnore]` and maps references to the existing member rather than adding a duplicate.

One situation that can occur is if you have subclasses of a patched class which also want to use the private variable or method. Because the variable or method is still private in the game's code, it will throw an error when you try to access it during runtime. Therefore, we need to use the `[MonoModPublic]` annotation to make the game consider it a public variable or method instead.
`[MonoModIgnore][MonoModPublic] private int get_baseTechLevel() { return 0; }`
This is also a good example of how to handle methods which have non-void return values. Simply define an arbitrary return value as a placeholder. As long as the `[MonoModIgnore]` annotation is present, this placeholder will not be used by the game and the proper return value will be used as normal.

A **ref-returning wrapper** lets another patched caller replace the field itself. Mutating the contents of an existing dictionary does not require a ref return.
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

For example, `TechCategory` maps categories such as Materials and Energy to integer values. Inspect its current declaration and namespace before adding a category.

MonoMod can be used to extend or replace the values in this list, enabling us to pass custom enum values into functions which use enums as variables. To patch this enum with a new category, let's say `Infrastructure`, we would type:

```csharp
public enum patch_TechCategory : ushort
{
    Infrastructure = 20,
}
```
You **must** define an integer value for the **first** new item when patching. Choose a value within the actual underlying type's range that does not collide with vanilla or other mods. The `ushort` below is illustrative: match the target enum's underlying type.

To replace the enum's contents entirely, we use the `[MonoModEnumReplace]` annotation:
```csharp
[MonoModEnumReplace]
public enum patch_TechCategory : ushort
{
    Infrastructure = 0,
}
```
Be aware that many enums in TI are defined _outside_ of a namespace, so your patch will have to be outside the namespace as well.

Code consuming an enum may switch on each value, for example when choosing a technology icon. Inspect all relevant switches, array indexing, localization, and saved values.

When patching in new enum values, we will usually need to define new return values for them in these sorts of methods as well. In order to avoid having to duplicate the original code, we can use the orig_methodName process, like so:
```csharp
public static extern string orig_PathTechCategoryIcon(TechCategory category);
public static string PathTechCategoryIcon(TechCategory category)
{
    switch ((patch_TechCategory)category)
    {
        case patch_TechCategory.Infrastructure: return TemplateManager.global.pathEnergyIcon; // your path here
        default:
            return orig_PathTechCategoryIcon(category);
    }
}
```

The original tutorial reported save failures for newly patched enum values when FullSerializer expected a known enum name. Its workaround serialized unknown values as integers with `Enum.GetName`, `Convert.ToInt64`, and `fsData` inside `fsEnumConverter.TrySerialize`. A global converter replacement can affect unrelated enums; first reproduce the failure, then limit any workaround to your type and test both serialization and deserialization. An integer written successfully does not establish a safe save/load round trip.

## Adding Variables to Existing Classes

Adding variables to existing classes is very easy. Simply define the new variables anywhere in your patch class, and they will be considered part of the original class when the game runs.

Be aware that the compiler does not recognize that your patch class is "supposed" to be the same as the original class, but still considers it a child class. This means that attempting to access the new variables from methods which use the original class as an argument does not work.

Therefore, the best practice when writing your patch is to first recast the original class to the type of the patch class, like so:
`patch_TINationState nation_PVC = nation as patch_TINationState;`
You can then reference the new variables defined in patch_TINationState through nation_PVC.

Added fields initially have their CLR defaults unless initialization code runs: reference fields default to null, while value types have their own defaults. Field initializers are constructor code; adding only the field declaration does not make the original constructor execute them. Normally, this is not a problem, as you will usually assign a value to the variable in the course of using the class anyway. But there can be situations where you want to ensure that a specific value will be initialized with a given variable.

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
