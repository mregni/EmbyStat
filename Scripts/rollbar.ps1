$version = "$($env:BuildVersion.SemVer)";
if($version -like '*dev*'){
  $postParams = @{
    access_token="$($env:RollbarKey)";
    environment="dev";
    revision=$version;
    local_username="Mikhaël Regni";
  }
  Invoke-WebRequest -Uri https://api.rollbar.com/api/1/deploy -Method POST -UseBasicParsing -Body $postParams
}