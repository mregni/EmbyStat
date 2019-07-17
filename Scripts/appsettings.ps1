$appSettings = "EmbyStat.Web\appsettings.json"
$version = "$($env:BuildVersion.MajorVersion).$($env:BuildVersion.MinorVersion).$($env:BuildVersion.PatchVersion).$($env:BuildVersion.BuildCounter)"

(GC $appSettings).Replace("0.0.0.0", "$version") | Set-Content $appSettings
(GC $appSettings).Replace("RollbarENV", "dev") | Set-Content $appSettings
(GC $appSettings).Replace("XXXXXXXXXX", "$($env:RollbarKey)") | Set-Content $appSettings