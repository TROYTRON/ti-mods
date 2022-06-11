# Installing the Terra Invicta mod loader

The tool that allows mods to modify Terra Invicta's code without conflicting with each other and breaking whenever the game is updated is called BepInEx. This tutorial will go over how to install BepInEx 5 and then how to install any Terra Invicta mods using it. This process has only been attempted on Windows so far - Mac and Linux users may need to improvise a bit.

This Terra-Invicta-specific tutorial was adapted from the generic BepInEx installation tutorial which can be found here: https://docs.bepinex.dev/v5.4.16/articles/user_guide/installation/index.html

If you have any questions or issues, feel free to ping me @NotSoLoneWolf on the Terra Invicta discord.

### Step 1: Download

Go to the link below and scroll down. At the bottom under "Assets" there will be a list of five files. We want the 64-bit version, since Terra Invicta is a 64-bit game. Click on `BepInEx_x64_5.4.16.0.zip` to download it.

https://github.com/BepInEx/BepInEx/releases/tag/v5.4.16

### Step 2: Relocate

Use 7Zip or WinRAR or whichever package extraction program you like to turn the `.zip` file you downloaded into a regular folder. Inside the extracted folder "BepInEx_x64_5.4.16.0", you should see four items: a subfolder named "BepInEx", and the three files "winhttp.dll", "doorstop_config.ini", and "changelog.txt".

Next, locate your Terra Invicta game folder. Knowing where your Steam library is located will be useful. The path looks something like `<YOUR_STEAM_LIBRARY_FOLDER>\steamapps\common\Terra Invicta`. On Windows PCs, the default location for your Steam library is `C:\Program Files (x86)\Steam`.

Finally, cut and paste or simply drag the four items mentioned earlier to your Terra Invicta folder. The folder structure should look like `steamapps\common\Terra Invicta\BepInEx` with the other three files inside `Terra Invicta`.

### Step 3: Initialize

Run Terra Invicta, either from Steam or from the `.exe` file in the game folder. Wait until the main menu comes up, then feel free to exit the game. Now go back to our folder structure. If you see a `plugins` subfolder (as well as a few other folders and `LogOutput.log`) inside your `Terra Invicta\BepInEx` folder, everything worked correctly. You're ready to start loading mods!

### Step 4: Load Mods

Mods you would like to install should be moved into the `Terra Invicta\BepInEx\plugins` folder. If your game is running, close it and restart to allow the newly installed mods to take effect.

The format mods should be in are singular `.dll` files. Any folders or any other type of file are not valid mods and could possibly break things if moved into the `plugins` folder.
