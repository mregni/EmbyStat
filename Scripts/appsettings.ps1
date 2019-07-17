$appSettings = "EmbyStat.Web\appsettings.json"
$version = "$($env:GitVersion_SemVer)"
$rollbarKey = "$(RollbarKey)"

(GC $appSettings).Replace("0.0.0.0", "$version") | Set-Content $appSettings
(GC $appSettings).Replace("RollbarENV", "dev") | Set-Content $appSettings
(GC $appSettings).Replace("XXXXXXXXXX", $rollbarKey) | Set-Content $appSettings