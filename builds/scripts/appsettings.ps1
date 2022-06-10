param(
	[string]$updatePackage, 
	[string]$rollbarKey,
	[string]$rollbarEnv,
	[string]$version
)

$config = "EmbyStat.Configuration\Config.cs"
$defaultConfig = "EmbyStat.Hosts.Cmd\DefaultConfig.cs"

Write-Host "Updating version to $version";
(Get-Content $config).Replace("0.0.0.0", "$version") | Set-Content $config

Write-Host "Updating Rollbar environment to $rollbarEnv";
(Get-Content $config).Replace("RollbarEnvironment", $rollbarEnv) | Set-Content $config

Write-Host "Updating Rollbar key to $rollbarKey";
(Get-Content $config).Replace("RollbarAccessToken", "$rollbarKey") | Set-Content $config

Write-Host "Updating update package name to $updatePackage"
(Get-Content $defaultConfig).Replace("win10-x64-v{version}.zip", $updatePackage + '-v{version}.zip') | Set-Content $defaultConfig