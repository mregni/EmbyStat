	[CmdletBinding()]
param(
    [switch]$MakeNSIS,
    [switch]$InstallNSIS,
    [switch]$InstallNSSM,
    [switch]$GenerateZip,
    [string]$InstallLocation = "./di",
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

function Build-EmbyStat {
    if(($Architecture -eq 'arm64') -and ($WindowsVersion -ne 'win10')){
            Write-Error "arm64 only supported with Windows10 Version"
            exit
        }
    if(($Architecture -eq 'arm') -and ($WindowsVersion -notin @('win10','win81','win8'))){
            Write-Error "arm only supported with Windows 8 or higher"
            exit
        }
    Write-Verbose "windowsversion-Architecture: $windowsversion-$Architecture"
    Write-Verbose "InstallLocation: $ResolvedInstallLocation"
    Write-Verbose "DotNetVerbosity: $DotNetVerbosity"
    dotnet publish Embystat.Web/EmbyStat.Web.csproj -c $BuildType --output $ResolvedInstallLocation -v $DotNetVerbosity --runtime `"$windowsversion-$Architecture`" -p:GenerateDocumentationFile=false -p:DebugSymbols=false -p:DebugType=none
	dotnet publish Updater/Updater.csproj -c $BuildType --output $ResolvedInstallLocation/updater -v $DotNetVerbosity --runtime `"$windowsversion-$Architecture`" -p:GenerateDocumentationFile=false -p:DebugSymbols=false -p:DebugType=none
}

function Install-NSSM {
    param(
        [string]$ResolvedInstallLocation,
        [string]$Architecture
    )
    Write-Verbose "Checking Architecture"
    if($Architecture -notin @('x86','x64')){
        Write-Warning "No builds available for your selected architecture of $Architecture"
        Write-Warning "NSSM will not be installed"
    }else{
         Write-Verbose "Downloading NSSM"
         # [Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
         # Temporary workaround, file is hosted in an azure blob with a custom domain in front for brevity
         Invoke-WebRequest -Uri http://files.evilt.win/nssm/nssm-2.24-101-g897c7ad.zip -UseBasicParsing -OutFile "$tempdir/nssm.zip" | Write-Verbose
    }

    Expand-Archive "$tempdir/nssm.zip" -DestinationPath "$tempdir/nssm/" -Force | Write-Verbose
    if($Architecture -eq 'x64'){
        Write-Verbose "Copying Binaries to Embystat location"
        Get-ChildItem "$tempdir/nssm/nssm-2.24-101-g897c7ad/win64" | ForEach-Object {
            Copy-Item $_.FullName -Destination $installLocation | Write-Verbose
        }
    }else{
        Write-Verbose "Copying Binaries to Embystat location"
        Get-ChildItem "$tempdir/nssm/nssm-2.24-101-g897c7ad/win32" | ForEach-Object {
            Copy-Item $_.FullName -Destination $installLocation | Write-Verbose
        }
    }
    Remove-Item "$tempdir/nssm/" -Recurse -Force -ErrorAction Continue | Write-Verbose
    Remove-Item "$tempdir/nssm.zip" -Force -ErrorAction Continue | Write-Verbose
}

function Make-NSIS {
    param(
        [string]$ResolvedInstallLocation
    )

    $env:InstallLocation = $ResolvedInstallLocation
    if($InstallNSIS.IsPresent -or ($InstallNSIS -eq $true)){
        & "$tempdir/nsis/nsis-3.04/makensis.exe" /D$Architecture /DUXPATH=$ResolvedUXLocation  ".\builds\windows\embystat.nsi"
    } else {
        & "makensis" /D$Architecture /DUXPATH=$ResolvedUXLocation /VERSION=$Env:GITVERSION_SEMVER ".\builds\windows\embystat.nsi"
    }
    Move-Item .\builds\windows\embystat_*.exe $ResolvedInstallLocation\..\
}


function Install-NSIS {
    Write-Verbose "Downloading NSIS"
    [Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
    Invoke-WebRequest -Uri https://nchc.dl.sourceforge.net/project/nsis/NSIS%203/3.04/nsis-3.04.zip -UseBasicParsing -OutFile "$tempdir/nsis.zip" | Write-Verbose

    Expand-Archive "$tempdir/nsis.zip" -DestinationPath "$tempdir/nsis/" -Force | Write-Verbose
}

function Cleanup-NSIS {
    Remove-Item "$tempdir/nsis/" -Recurse -Force -ErrorAction Continue | Write-Verbose
    Remove-Item "$tempdir/nsis.zip" -Force -ErrorAction Continue | Write-Verbose
}

Write-Verbose "Starting Build Process: Selected Environment is $WindowsVersion-$Architecture"
Build-EmbyStat

if($InstallNSSM.IsPresent -or ($InstallNSSM -eq $true)){
    Write-Verbose "Starting NSSM Install"
    Install-NSSM $ResolvedInstallLocation $Architecture
}
Copy-Item .\LICENSE $ResolvedInstallLocation\LICENSE
if($InstallNSIS.IsPresent -or ($InstallNSIS -eq $true)){
    Write-Verbose "Installing NSIS"
    Install-NSIS
}
if($MakeNSIS.IsPresent -or ($MakeNSIS -eq $true)){
    Write-Verbose "Starting NSIS Package creation"
    Make-NSIS $ResolvedInstallLocation
}
if($InstallNSIS.IsPresent -or ($InstallNSIS -eq $true)){
    Write-Verbose "Cleanup NSIS"
    Cleanup-NSIS
}
if($GenerateZip.IsPresent -or ($GenerateZip -eq $true)){
    Compress-Archive -Path $ResolvedInstallLocation -DestinationPath "$ResolvedInstallLocation/embystat.zip" -Force
}
Write-Verbose "Finished"