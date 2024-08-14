Mirrored from this guide on Discord written by __Stallion__:
https://discord.com/channels/462769550841348126/1155428797408088064

I've attached the fmod project that will need to be opened in fmod for modding. You will need to use the same version as us, FMOD 2.01.07
https://www.fmod.com/download#fmodstudio

Once you open the package you can import your audio files here

![2](https://github.com/user-attachments/assets/3a81d9c6-28c5-4721-9a80-56138b592160)

You can then right-click the assets and "create event", 2D timeline is the standard for regular sounds that aren't in 3d space.

![3](https://github.com/user-attachments/assets/bfd00360-1cf9-4fd9-ac53-9ef4d488a86e)

In the banks tab, create your own bank, with a name that you want. Make sure to "Mark as Master Bank"

![4](https://github.com/user-attachments/assets/a6203e49-62ca-482d-8cf3-a04d757fad44)

The Events tab has the audio events, rename them however you like, the name is what will go into the json templates in order to get the game to play them.

You can also apply volume or special effects to the audio here if need.

![5](https://github.com/user-attachments/assets/6710bc9a-c8a7-4b94-991d-2dcbfedf0965)

If you right-click an event you can "copy path" this is the path that goes in the jsons for example ModTest is "event:/ModTest"

Make sure to right click and add each event into your bank

![6](https://github.com/user-attachments/assets/027af609-c599-4769-82e7-91f47e01c175)

Next is the Window -> Mixer screen, here is where you route the audio into the buses. For example if you are adding voiceovers, add them into "Voice processing". That will make sure they work with the volume sliders in-game

![7](https://github.com/user-attachments/assets/6d4a2565-b604-4378-bb57-1cb9d62e3482)

When you are finished with everything, go to edit -> preferences make sure to enable the metadata and assets into a single bank

![8](https://github.com/user-attachments/assets/e67eee4c-9f9e-4188-973a-311c2e0bfe5f)

Then File -> Build to create your banks

![9](https://github.com/user-attachments/assets/f785abc6-67ae-4794-a682-b158bc57cac8)

"Export GUIDs" will create a text file of every asset if you wanted that for a reference
To include the audio in your mod just copy both of the built banks into your mod folder. Then you just need to call the paths in the json templates where applicable

![10](https://github.com/user-attachments/assets/ffda3648-c292-4e72-9ab8-e997bcd80388)

Stallion — 28/09/2023 04:09
I believe the leader audio is in both the tech template and objective template
Stallion — 28/09/2023 04:18
Not all of the audio is easily moddable right now, like councilors on missions, the path is generated in code depending on the context. We may need to expose some more paths in the future for things like that. I'll think on an easier way to replace audio? Maybe if a modded audio event had the same path as a vanilla file, it replaces it.
Stallion — 30/09/2023 02:59
I've attached another .fspackage, this one has the metadata for all events which means you can swap audio without having to touch the .json template files. (I'll ping this thread once we actually upload the patch which has the functionality)

-As in the picture you can select the vanilla event you wish to replace.
-Click "single instrument"
-At the bottom you will see there is no audio file, you can drag your file there.
-Follow the previous instructions above to create a bank and assign the events to your mod's bank and build.
-The mixer buses are already setup with this package so if you are just replacing vanilla audio no need to route them.

I think this workflow would be much easier for just replacing audio in a mod. 

![11](https://github.com/user-attachments/assets/743bc294-9a1b-423a-b546-cb033b8b92fe)
