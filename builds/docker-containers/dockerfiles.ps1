New-Item -Path "$($env:System_DefaultWorkingDirectory)" -Name "publish" -ItemType "directory"

Copy-Item ".\builds\docker-containers\files\win.dockerfile" -Destination "$($env:System_DefaultWorkingDirectory)\publish\win.dockerfile"
Copy-Item ".\builds\docker-containers\files\arm64.dockerfile" -Destination "$($env:System_DefaultWorkingDirectory)\publish\arm64.dockerfile"
Copy-Item ".\builds\docker-containers\files\linux.dockerfile" -Destination "$($env:System_DefaultWorkingDirectory)\publish\linux.dockerfile"
Copy-Item ".\builds\docker-containers\files\manifest-tool-windows-amd64.exe" -Destination "$($env:System_DefaultWorkingDirectory)\publish\manifest-tool-windows-amd64.exe"
Copy-Item ".\builds\docker-containers\files\manifest-nightly.yaml" -Destination "$($env:System_DefaultWorkingDirectory)\publish\manifest-nightly.yaml"
Copy-Item ".\builds\docker-containers\files\manifest-beta.yaml" -Destination "$($env:System_DefaultWorkingDirectory)\publish\manifest-beta.yaml"