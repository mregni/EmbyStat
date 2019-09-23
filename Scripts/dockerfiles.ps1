New-Item -Path "$($env:System_DefaultWorkingDirectory)" -Name "publish" -ItemType "directory"

Copy-Item "Docker\win.dockerfile" -Destination "$($env:System_DefaultWorkingDirectory)\publish\win.dockerfile"
Copy-Item "Docker\linux.dockerfile" -Destination "$($env:System_DefaultWorkingDirectory)\publish\linux.dockerfile"
Copy-Item "Docker\manifest-tool-windows-amd64.exe" -Destination "$($env:System_DefaultWorkingDirectory)\publish\manifest-tool-windows-amd64.exe"
Copy-Item "Docker\manifest-nightly.yaml" -Destination "$($env:System_DefaultWorkingDirectory)\publish\manifest-nightly.yaml"
Copy-Item "Docker\manifest-beta.yaml" -Destination "$($env:System_DefaultWorkingDirectory)\publish\manifest-beta.yaml"