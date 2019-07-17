New-Item -Path "$(System.DefaultWorkingDirectory)" -Name "publish" -ItemType "directory"

Copy-Item "win.dockerfile" -Destination "$(System.DefaultWorkingDirectory)\publish\win.dockerfile"
Copy-Item "linux.dockerfile" -Destination "$(System.DefaultWorkingDirectory)\publish\linux.dockerfile"
