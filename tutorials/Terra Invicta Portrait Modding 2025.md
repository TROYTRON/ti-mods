Terra Invicta Portrait Modding Documentation

What you need:
1. VC8 coded Webm file for main portrait. Must have alpha channel for transparency. Optional: alternate appearance for old councilor.
2. Two .512x512 pngs of portrait, one for static portrait one and one for badge. Optional: alternate appearance for old councilor.
3. Unity 2020.3.48f1 (https://unity.com/releases/editor/whats-new/2020.3.48) Must have unity account!

WEBM Creation:
I recommend using shutter encoder (https://www.shutterencoder.com/) to compress .png files into a webm. Upload two identical copies of your portrait for encoding. 



PNG CREATION:
You will need a static portrait and a “badge” portrait that acts as your councilors marker on the map. The static portrait can be the png file used for your webm. Your badge portrait should be a 512x512 close crop of your councilor portrait. You can find a template for these in the tutorial-files folder.
They’re called human rights, and aliens don't have them

UNITY:
1. Open a 3d core project in Unity 2020.3.48f1
2. Create this file structure.

In detail: Create Assetbundle folder if it does not exist. Create “Portraits” folder in Assetbundle, then create a “2d” and a “Video” folder within Portraits. Create “Editor” folder in Assets. Ensure folder names are exactly as written above, including caps.
3. Place CreateAssetBundles.cs into Editor folder. 
4. Place your Webms into Video folder and pngs into 2d folder
5. Assign webms to new assetbundle in lower right. Give it the name of your mod. No settings changes are necessary.
6. Change Texture Type of pngs to Sprite (2D and UI). Ensure all settings match those in the below image, if they do not already.
7. Assign pngs to the same assetbundle you added your webms to.
8. Go to upper left toolbar. Assets -> Buildassetbundles (this option will be near bottom). If you do not see this option, you have an issue with CreateAssetBundles.cs in your Editor file.
9. Output will generate “Streamingassets” folder with your assetbundle name in it, a .manifest file, and 2 .meta files. Move assetbundle and its .manifest file to mod directory.
10. Be sure to configure TICouncilorAppearanceTemplate. You can find a template to use by looking at vanilla files or downloading other mods, and a template with descriptions of function is provided below.


 {
   "dataName": "", #Name of your portrait
   "string": "", #can be same as above
   "enable": true, #always make true 
   "specific_person": "true", #Disables randomly generated councilors using this portrait, but it can still be selected manually in the customization screen
    "notes": "",
 
   "idleVideoYoung": "(assetbundle name)/(Webm)", #Appearance
   "idleVideoOld": "(assetbundle name)/(Webm)" #Appearance when old
   "portraitYoung": "(assetbundle name)/(static png)" #Static portrait
   "portraitOld": "(assetbundle name)/(static png)" #Static portrait when old
   "iconYoung": "(assetbundle name)/(badge png)" #Badge
   "iconOld": "(assetbundle name)/(badge png)" #Badge when old
   "allowedGenders": [ #what genders can roll this portrait, and how it is organized
      "Male",
      "Female",
      "NonBinary"
   ],
   "allowedAncestries": [ #what ethnicities can roll this portrait, and how it is organized.
    "African",
    "Asian",
    "EastAsian",
    "European",
    "Hispanic",
    "Oceanic"

   ],
   "allowedJobNames": [ #what councilor roles can roll this portrait
	"Rebel",
	"Operative",
	"Fixer",
	"Spy",
	"Kingpin",
	"Activist",
	"Inspector",
	"Officer",
	"Judge",
	"Politician",
	"Executive",
	"Journalist",
	"Commando",
	"Evangelist",
	"Diplomat",
	"Hacker",
	"Investigator",
	"Celebrity",
	"TechMogul",
	"Scientist",
	"Astronaut",
	"Professor",
	"Tycoon"

   ]
},
