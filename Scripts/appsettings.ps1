$appSettings = "EmbyStat.Web\appsettings.json"
$version = "$($env:GitVersion_SemVer)"
$rollbarKey = "$($env:webapp_rollbar_key)"

Write-Host "Updating version to $version";
(GC $appSettings).Replace("0.0.0.0", "$version") | Set-Content $appSettings


Write-Host "Updating RollbarENV to dev";
(GC $appSettings).Replace("RollbarENV", "dev") | Set-Content $appSettings

Write-Host "Updating Rollbar key to $rollbarKey";
(GC $appSettings).Replace("XXXXXXXXXX", "$rollbarKey") | Set-Content $appSettings