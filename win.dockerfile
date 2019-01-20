#installer env image
FROM mcr.microsoft.com/windows/nanoserver:latest AS installer-env
SHELL ["powershell", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';"]

RUN Invoke-WebRequest -OutFile node.zip https://nodejs.org/dist/v8.9.4/node-v8.9.4-win-x64.zip
RUN Expand-Archive node.zip -DestinationPath nodejs
RUN Remove-Item -Force node.zip

#Build image
FROM microsoft/dotnet:2.2.103-sdk AS builder

COPY --from=installer-env ["nodejs/node-v8.9.4-win-x64", "C:/Program Files/nodejs"]

USER ContainerAdministrator 
RUN setx /M PATH "%PATH%;C:/Program Files/nodejs"
USER ContainerUser

COPY . .
RUN dotnet publish ./EmbyStat.Web/EmbyStat.Web.csproj --framework netcoreapp2.2 --configuration Release --runtime win7-x64 --output /app
RUN dotnet publish ./Updater/Updater.csproj --framework netcoreapp2.2 --configuration Release --runtime win7-x64 --output /app/updater

#Runtime image
FROM microsoft/dotnet:2.2.1-runtime AS base

WORKDIR /app
ENV ASPNETCORE_URLS=http://*:5432
LABEL author="UPing"

COPY --from=builder /app .
ENTRYPOINT ["dotnet", "EmbyStat.Web.dll"]