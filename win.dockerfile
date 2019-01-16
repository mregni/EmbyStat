# escape=`

#installer env image
FROM microsoft/windowsservercore:1803 AS installer-env
SHELL ["powershell", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';"]
ENV DOTNET_SDK_VERSION 2.2.103
ENV NODEJS_VERSION 8.9.4

RUN Invoke-WebRequest -OutFile dotnet.zip https://dotnetcli.blob.core.windows.net/dotnet/Sdk/$Env:DOTNET_SDK_VERSION/dotnet-sdk-$Env:DOTNET_SDK_VERSION-win-x64.zip; `
    $dotnet_sha512 = 'a74d230ab184c6571e445c8e05361f1db32ffb9bd76baafe4d307ef42b7bf9cebd70e1aed37136caa485ebace185fea24b476907ff7bf522f3b485c8cf0c285b'; `
    if ((Get-FileHash dotnet.zip -Algorithm sha512).Hash -ne $dotnet_sha512) { `
        Write-Host 'CHECKSUM VERIFICATION FAILED!'; `
        exit 1; `
    }; `
    
RUN Expand-Archive dotnet.zip -DestinationPath dotnet
RUN Remove-Item -Force dotnet.zip

RUN Invoke-WebRequest -OutFile node.zip https://nodejs.org/dist/v$Env:NODEJS_VERSION/node-v$Env:NODEJS_VERSION-win-x64.zip
RUN Expand-Archive node.zip -DestinationPath nodejs
RUN Remove-Item -Force node.zip

#Build image
FROM microsoft/nanoserver:1803 AS builder

COPY --from=installer-env ["dotnet", "C:\\Program Files\\dotnet"]
COPY --from=installer-env ["nodejs\\node-v8.9.4-win-x64", "C:\\Program Files\\nodejs"]

USER ContainerAdministrator 
RUN setx /M PATH "%PATH%;C:\Program Files\dotnet"
RUN setx /M PATH "%PATH%;C:\Program Files\nodejs"
USER ContainerUser

COPY . .
RUN dotnet publish ./EmbyStat.Web/EmbyStat.Web.csproj --framework netcoreapp2.2 --configuration Release --runtime win7-x64 --output /app
RUN dotnet publish ./Updater/Updater.csproj --framework netcoreapp2.2 --configuration Release --runtime win7-x64 --output /app/updater

#Runtime image
FROM microsoft/nanoserver:1803 AS base

COPY --from=installer-env ["dotnet", "C:\\Program Files\\dotnet"]

USER ContainerAdministrator 
RUN setx /M PATH "%PATH%;C:\Program Files\dotnet"
USER ContainerUser
WORKDIR /app
ENV ASPNETCORE_URLS=http://*:5432
LABEL author="UPing"

COPY --from=builder /app .
ENTRYPOINT ["dotnet", "EmbyStat.Web.dll"]