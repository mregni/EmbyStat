New-Item -Path "$($env:System_DefaultWorkingDirectory)" -Name "publish" -ItemType "directory"

Copy-Item "win.dockerfile" -Destination "$($env:System_DefaultWorkingDirectory)\publish\win.dockerfile"
Copy-Item "linux.dockerfile" -Destination "$($env:System_DefaultWorkingDirectory)\publish\linux.dockerfile"
