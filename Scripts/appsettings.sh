#!/bin/bash

package = $1
rollbarKey = $2
version = $3

echo "Updating version to " $3
sed -i 's/0.0.0.0/$version/g' EmbyStat.Web/appsettings.json

echo "Updating RollbarENV to dev"
sed -i 's/RollbarENV/dev/g' EmbyStat.Web/appsettings.json

echo "Updating Rollbar key to " $2
sed -i 's/XXXXXXXXXX/$rollbarKey/g' EmbyStat.Web/appsettings.json

echo "Updating update package name to " $1
sed -i 's/win10-x64-v{version}.zip/$package-v{version}.tar.gz/g' EmbyStat.Web/appsettings.json