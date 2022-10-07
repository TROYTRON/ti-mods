Tools used:
- Unity.
- GIMP.
- FFmpeg.

1. Install ffmpeg and configure your PATH variable for it.
    -	[Here](https://www.thewindowsclub.com/how-to-install-ffmpeg-on-windows-10) is a good guide on how to set this up.
3. Procure a suitable image, preferably of size 512x512*.
4. Duplicate the image to form a sequence `1.png, 2.png`.
5. Run this command in cmd/shell: `ffmpeg -framerate 1 -i %1d.png -c:v libvpx -auto-alt-ref 0 <PORTRAIT_NAME>.webm`. This will produce a 2-frame video.
6. Import your new .webm into Unity, add it to your preferred AssetBundle, build the AssetBundle, and put it into `StreamingAssets\AssetBundles`.
7. Don't forget to configure `TICouncilorAppearanceTemplate.json` with your new portrait.

\*TI's portraits are 1024x1024, but nobody has a monitor big enough to actually need that, so 512x512 should be fine.
