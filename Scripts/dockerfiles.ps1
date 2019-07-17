New-Item -Path "$($env:System.DefaultWorkingDirectory)" -Name "publish" -ItemType "directory"

Copy-Item "win.dockerfile" -Destination "$($env:System.DefaultWorkingDirectory)\publish\win.dockerfile"
Copy-Item "linux.dockerfile" -Destination "$($env:	System.DefaultWorkingDirectory)\publish\linux.dockerfile"
