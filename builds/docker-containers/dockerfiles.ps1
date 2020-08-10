New-Item -Path "$($env:System_DefaultWorkingDirectory)" -Name "publish" -ItemType "directory"

Copy-Item "builds\docker-containers\files\win.dockerfile" -Destination "$($env:System_DefaultWorkingDirectory)\publish\docker\win.dockerfile"
Copy-Item "builds\docker-containers\files\linux.dockerfile" -Destination "$($env:System_DefaultWorkingDirectory)\publish\docker\linux.dockerfile"
Copy-Item "builds\docker-containers\files\manifest-tool-windows-amd64.exe" -Destination "$($env:System_DefaultWorkingDirectory)\publish\docker\manifest-tool-windows-amd64.exe"
Copy-Item "builds\docker-containers\files\manifest-nightly.yaml" -Destination "$($env:System_DefaultWorkingDirectory)\publish\docker\manifest-nightly.yaml"
Copy-Item "builds\docker-containers\files\manifest-beta.yaml" -Destination "$($env:System_DefaultWorkingDirectory)\publish\docker\manifest-beta.yaml"