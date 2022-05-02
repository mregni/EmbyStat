	[CmdletBinding()]
param(
    [switch]$MakeNSIS,
    [switch]$InstallNSIS,
    [switch]$InstallNSSM,
    [switch]$GenerateZip,
    [string]$version,
    [string]$InstallLocation = "./publish",
    [string]$UXLocation = "./",
    [ValidateSet('Debug','Release')][string]$BuildType = 'Release',
    [ValidateSet('Quiet','Minimal', 'Normal')][string]$DotNetVerbosity = 'Minimal',
    [ValidateSet('win','win7', 'win8','win81','win10')][string]$WindowsVersion = 'win',
    [ValidateSet('x64','x86', 'arm', 'arm64')][string]$Architecture = 'x64'
)

#PowershellCore and *nix check to make determine which temp dir to use.
if(($PSVersionTable.PSEdition -eq 'Core') -and (-not $IsWindows)){
    $TempDir = mktemp -d
}else{
    $TempDir = $env:Temp
}
#Create staging dir
New-Item -ItemType Directory -Force -Path $InstallLocation
$ResolvedInstallLocation = Resolve-Path $InstallLocation
$ResolvedUXLocation = Resolve-Path $UXLocation

function Install-NSSM {
    param(
        [string]$ResolvedInstallLocation,
        [string]$Architecture
    )
    Write-Host "Checking Architecture"
    if($Architecture -notin @('x86','x64')){
        Write-Warning "No builds available for your selected architecture of $Architecture"
        Write-Warning "NSSM will not be installed"
    }else{
         Write-Host "Downloading NSSM"
         # [Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
         # Temporary workaround, file is hosted in an azure blob with a custom domain in front for brevity
         Invoke-WebRequest -Uri http://files.evilt.win/nssm/nssm-2.24-101-g897c7ad.zip -UseBasicParsing -OutFile "$tempdir/nssm.zip" | Write-Host
    }

    Expand-Archive "$tempdir/nssm.zip" -DestinationPath "$tempdir/nssm/" -Force | Write-Host
    if($Architecture -eq 'x64'){
        Write-Host "Copying Binaries to Embystat location"
        Get-ChildItem "$tempdir/nssm/nssm-2.24-101-g897c7ad/win64" | ForEach-Object {
            Copy-Item $_.FullName -Destination $installLocation | Write-Host
        }
    }else{
        Write-Host "Copying Binaries to Embystat location"
        Get-ChildItem "$tempdir/nssm/nssm-2.24-101-g897c7ad/win32" | ForEach-Object {
            Copy-Item $_.FullName -Destination $installLocation | Write-Host
        }
    }
    Remove-Item "$tempdir/nssm/" -Recurse -Force -ErrorAction Continue | Write-Host
    Remove-Item "$tempdir/nssm.zip" -Force -ErrorAction Continue | Write-Host
}

function Make-NSIS {
    param(
        [string]$ResolvedInstallLocation
    )

    $env:InstallLocation = $ResolvedInstallLocation
    if($InstallNSIS.IsPresent -or ($InstallNSIS -eq $true)){
        & "$tempdir/nsis/nsis-3.08/makensis.exe" /D$Architecture /DVERSION=$version /DUXPATH=$ResolvedUXLocation  ".\builds\windows-installer\embystat.nsi"
    } else {
        & "makensis" /D$Architecture /DVERSION=$version /DUXPATH=$ResolvedUXLocation ".\builds\windows-installer\embystat.nsi"
    }
    Move-Item .\builds\windows-installer\embystat_*.exe $ResolvedInstallLocation\..\
}


function Install-NSIS {
    Write-Host "Downloading NSIS"
    Invoke-WebRequest -Uri "https://downloads.sourceforge.net/project/nsis/NSIS%203/3.08/nsis-3.08.zip" -UseBasicParsing -OutFile "$tempdir/nsis.zip" -UserAgent [Microsoft.PowerShell.Commands.PSUserAgent]::FireFox | Write-Host

    Expand-Archive "$tempdir/nsis.zip" -DestinationPath "$tempdir/nsis/" -Force | Write-Host
}

    function Cleanup-NSIS {
    Remove-Item "$tempdir/nsis/" -Recurse -Force -ErrorAction Continue | Write-Host
    Remove-Item "$tempdir/nsis.zip" -Force -ErrorAction Continue | Write-Host
}

Write-Host "Starting Build Process: Selected Environment is $WindowsVersion-$Architecture"

if($InstallNSSM.IsPresent -or ($InstallNSSM -eq $true)){
    Write-Host "Starting NSSM Install"
    Install-NSSM $ResolvedInstallLocation $Architecture
}
Copy-Item .\LICENSE $ResolvedInstallLocation\LICENSE
if($InstallNSIS.IsPresent -or ($InstallNSIS -eq $true)){
    Write-Host "Installing NSIS"
    Install-NSIS
}
if($MakeNSIS.IsPresent -or ($MakeNSIS -eq $true)){
    Write-Host "Starting NSIS Package creation"
    Make-NSIS $ResolvedInstallLocation
}
if($InstallNSIS.IsPresent -or ($InstallNSIS -eq $true)){
    Write-Host "Cleanup NSIS"
    Cleanup-NSIS
}
if($GenerateZip.IsPresent -or ($GenerateZip -eq $true)){
    Compress-Archive -Path $ResolvedInstallLocation -DestinationPath "$ResolvedInstallLocation/embystat.zip" -Force
}
Write-Host "Finished"