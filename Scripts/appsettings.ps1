$appSettings = "EmbyStat.Web\appsettings.json"
$version = "$($env:GitVersion_SemVer)"

(GC $appSettings).Replace("0.0.0.0", "$version") | Set-Content $appSettings
(GC $appSettings).Replace("RollbarENV", "dev") | Set-Content $appSettings
(GC $appSettings).Replace("XXXXXXXXXX", "$(RollbarKey)") | Set-Content $appSettings