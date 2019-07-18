param( 
	[string]$rollbarKey
)

$buildNumber = "$($env:Build.BuildNumber)";
$environment = "production"

if($buildNumber -like '*dev*'){
  $environment = "dev"
}

if($buildNumber -like '*beta*'){
  $environment = "beta"
}


$postParams = @{
    access_token="$rollbarKey";
    environment="$environment";
    revision="$buildNumber";
    local_username="Mikhaël Regni";
	status="succeeded"
  }
  Invoke-WebRequest -Uri https://api.rollbar.com/api/1/deploy -Method POST -UseBasicParsing -Body $postParams