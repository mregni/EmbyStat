$appSettings = "EmbyStat.Web\appsettings.json"
$version = "$(BuildVersion.MajorVersion).$(BuildVersion.MinorVersion).$(BuildVersion.PatchVersion).$(BuildVersion.BuildCounter)"

(GC $appSettings).Replace("0.0.0.0", "$version") | Set-Content $appSettings
(GC $appSettings).Replace("RollbarENV", "dev") | Set-Content $appSettings
(GC $appSettings).Replace("XXXXXXXXXX", "$(RollbarKey)") | Set-Content $appSettings