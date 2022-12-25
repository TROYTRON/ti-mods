Tools used:
- Unity.
- GIMP.
- FFmpeg.

1. Install ffmpeg and configure your PATH variable for it.
    -	[Here](https://www.thewindowsclub.com/how-to-install-ffmpeg-on-windows-10) is a good guide on how to set this up.
    -	![image](https://user-images.githubusercontent.com/16394154/194540028-481c8543-fa51-45af-9884-e88a7bac52f9.png)
3. Procure a suitable image, preferably of size 512x512*.
4. Duplicate the image to form a sequence `1.png, 2.png`.
5. Run this command in cmd/shell: `ffmpeg -framerate 1 -i %1d.png -c:v libvpx -auto-alt-ref 0 <PORTRAIT_NAME>.webm`. This will produce a 2-frame video.
    -	The PowerShell script I wrote [here](https://github.com/TROYTRON/ti-mods/blob/main/mods/tayta/anime-councilors/waifu2vid.ps1) allows you to skip steps 3 and 4. Simply run the script in the directory with your .pngs to produce the needed .webms.
7. Import your new .webm into Unity, add it to your preferred AssetBundle, build the AssetBundle, and put it into `StreamingAssets\AssetBundles`.
    -	PROTIP: If your new AssetBundle has the string "charactericons" in it, the game will try to replace it with "charactericons_gui" in certain situations, which will cause an NRE crash. Either make this bundle as well, or use a different name to avoid this.
9. Don't forget to configure `TICouncilorAppearanceTemplate.json` with your new portrait.

\*TI's portraits are 1024x1024, but nobody has a monitor big enough to actually need that, so 512x512 should be fine.
