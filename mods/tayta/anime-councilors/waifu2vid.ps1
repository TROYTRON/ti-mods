$files = Get-ChildItem
foreach ($f in $files) {
	if ($f -Match ".png") {
		cp $f $f"_1.png"
		cp $f $f"_2.png"
		ffmpeg -framerate 1 -i $f"_%1d.png" -c:v libvpx -auto-alt-ref 0 $f".webm"
		rm $f"_1.png"
		rm $f"_2.png"
	}
}
$files = Get-ChildItem
foreach ($f in $files){
	if ($f -Match ".png.webm") {
		$name = $f.BaseName.split('.')[0]
		mv $f $name".webm"
	}
}
md videos
$files = Get-ChildItem
foreach ($f in $files){
	if ($f -Match ".webm") {
		mv $f videos\$f
	}
}
