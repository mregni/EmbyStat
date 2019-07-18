param(
	[string]$updatePackage, 
	[string]$rollbarKey
)

$appSettings = "EmbyStat.Web\appsettings.json"
$version = "$($env:GitVersion_SemVer)"

Write-Host "Updating version to $version";
(GC $appSettings).Replace("0.0.0.0", "$version") | Set-Content $appSettings

Write-Host "Updating RollbarENV to dev";
(GC $appSettings).Replace("RollbarENV", "dev") | Set-Content $appSettings

Write-Host "Updating Rollbar key to $rollbarKey";
(GC $appSettings).Replace("XXXXXXXXXX", "$rollbarKey") | Set-Content $appSettings

Write-Host "Updating update package name to $updatePackage"
(GC $appSettings).Replace("win10-x64-v{version}.zip", $updatePackage'-v{version}.zip') | Set-Content $appSettings