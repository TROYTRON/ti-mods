# Setting up your environment for Terra Invicta mod development

STILL UNDER CONSTRUCTION

### Step 1: Install Mod Loader

First, you'll need to go the "Players_Start_Here" tutorial in this repo and complete it.

### Step 2: Setup Development Environment

Follow these two tutorials in order, but read my notes below first.

https://docs.bepinex.dev/v5.4.16/articles/dev_guide/plugin_tutorial/1_setup.html

https://docs.bepinex.dev/v5.4.16/articles/dev_guide/plugin_tutorial/2_plugin_start.html

In the first tutorial, you could skip the section "Installing and configuring BepInEx" because we already did that. However, it may be useful to read it, as it will tell you where you can enable a logging window that will be useful when debugging your mods.

In the second tutorial, you can skip steps 1 and 2 of the section "Initializing a plugin project from template" where you find Terra Invicta's .NET version and Unity version. On step 3, use this command with your desired mod ID: `dotnet new bepinex5plugin -n <YOUR_MOD_ID> -T net46 -U 2020.3.12`

If you are using Visual Studio Community, attempting to build your mod at the end of the second tutorial will fail. This can be fixed by going to `Tools -> Options -> NuGet Package Manager -> Package Sources` and creating a new source with the URL `https://api.nuget.org/v3/index.json`. Other IDEs may suffer from the same problem, I don't know.

### Step 3: Mod Development

This step is yet to be constructed. In the meantime, you can probably figure it out with some C# skills and by reading the Harmony wiki:

https://harmony.pardeike.net/articles/intro.html
