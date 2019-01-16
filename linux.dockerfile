# escape=`

#Installer env image
FROM microsoft/dotnet:2.2.103-sdk-stretch as builder
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT false
ENV LC_ALL en_US.UTF-8
ENV LANG en_US.UTF-8
ENV LANGUAGE en_US.UTF-8
ENV DOTNET_SDK_VERSION 2.2.103

#set up dotnet
RUN apt-get update
RUN apt-get install -y --no-install-recommends curl gnupg wget apt-transport-https apt-utils
RUN rm -rf /var/lib/apt/lists/*

# Install .NET Core
RUN wget -qO- https://packages.microsoft.com/keys/microsoft.asc | gpg --dearmor > microsoft.asc.gpg
RUN mv microsoft.asc.gpg /etc/apt/trusted.gpg.d/
RUN wget -q https://packages.microsoft.com/config/debian/9/prod.list
RUN mv prod.list /etc/apt/sources.list.d/microsoft-prod.list
RUN chown root:root /etc/apt/trusted.gpg.d/microsoft.asc.gpg
RUN chown root:root /etc/apt/sources.list.d/microsoft-prod.list

RUN apt-get update
RUN apt-get install dotnet-sdk-2.2 -y

# set up node
ENV NODE_VERSION 8.9.4
RUN curl -sL https://deb.nodesource.com/setup_8.x | bash -
RUN apt-get update
RUN apt-get install -y nodejs 

ENV DOTNET_USE_POLLING_FILE_WATCHER false
ENV NUGET_XMLDOC_MODE skip

COPY . .
RUN dotnet publish ./EmbyStat.Web/EmbyStat.Web.csproj --framework netcoreapp2.2 --configuration Release --runtime ubuntu-x64 --output /app
RUN dotnet publish ./Updater/Updater.csproj --framework netcoreapp2.2 --configuration Release --runtime ubuntu-x64 --output /app/updater

FROM microsoft/dotnet:2.2.103-sdk-stretch as base
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT false
ENV LC_ALL en_US.UTF-8
ENV LANG en_US.UTF-8
ENV LANGUAGE en_US.UTF-8

RUN apt-get update
RUN apt-get install -y --no-install-recommends gnupg wget apt-transport-https apt-utils
RUN rm -rf /var/lib/apt/lists/*

RUN wget -q https://packages.microsoft.com/config/ubuntu/18.04/packages-microsoft-prod.deb
RUN dpkg -i packages-microsoft-prod.deb
RUN dpkg --purge packages-microsoft-prod && sudo dpkg -i packages-microsoft-prod.deb
RUN add-apt-repository universe
RUN apt-get install apt-transport-https
RUN apt-get update
RUN apt-get install dotnet-sdk-2.2 -y

WORKDIR /app
ENV ASPNETCORE_URLS=http://*:5432
LABEL author="UPing"

COPY --from=builder /app .
ENTRYPOINT ["dotnet", "EmbyStat.Web.dll"]