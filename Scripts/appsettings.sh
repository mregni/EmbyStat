sed -i 's/win10-x64-v{version}.zip/ubuntu.16.10-x64-v{version}.tar.gz/g' EmbyStat.Web/appsettings.json
sed -i 's/0.0.0.0/$(BuildVersion.MajorVersion).$($env:BuildVersion.MinorVersion).$($env:BuildVersion.PatchVersion).$($env:BuildVersion.BuildCounter)/g' EmbyStat.Web/appsettings.json
sed -i 's/RollbarENV/dev/g' EmbyStat.Web/appsettings.json
sed -i 's/XXXXXXXXXX/$(RollbarKey)/g' EmbyStat.Web/appsettings.json