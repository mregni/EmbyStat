#Installer env image
FROM microsoft/dotnet:2.2.103-sdk AS builder

# set up node
ENV NODE_VERSION 8.9.4
RUN curl -sL https://deb.nodesource.com/setup_8.x | bash -
RUN apt-get update
RUN apt-get install -y apt-utils nodejs 

ENV DOTNET_USE_POLLING_FILE_WATCHER false
ENV NUGET_XMLDOC_MODE skip

COPY . .
RUN dotnet publish ./EmbyStat.Web/EmbyStat.Web.csproj --framework netcoreapp2.2 --configuration Release --runtime ubuntu-x64 --output /app
RUN dotnet publish ./Updater/Updater.csproj --framework netcoreapp2.2 --configuration Release --runtime ubuntu-x64 --output /app/updater

FROM microsoft/dotnet:2.2.1-runtime as base
ENV LC_ALL=en_US.UTF-8
ENV LANG=en_US.UTF-8
ENV LANGUAGE=en_US.UTF-8
LABEL author="UPing"

WORKDIR /app
COPY --from=builder /app .
ENTRYPOINT ["dotnet", "EmbyStat.dll"] 