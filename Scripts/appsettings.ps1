param($updatePackage)

$appSettings = "EmbyStat.Web\appsettings.json"
$version = "$($env:GitVersion_SemVer)"
$rollbar = "$(rollbarkey)"

Write-Host "Updating version to $version";
(GC $appSettings).Replace("0.0.0.0", "$version") | Set-Content $appSettings

Write-Host "Updating RollbarENV to dev";
(GC $appSettings).Replace("RollbarENV", "dev") | Set-Content $appSettings

Write-Host "Updating Rollbar key to $($env:rollbarkey)";
(GC $appSettings).Replace("XXXXXXXXXX", "$rollbar") | Set-Content $appSettings

Write-Host "Updating update package name to $updatePackage"
(GC $appSettings).Replace("win10-x86", "$updatePackage") | Set-Content $appSettings