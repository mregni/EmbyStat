FROM microsoft/dotnet:2.2.1-runtime AS base
LABEL author="UPing"
LABEL maintainer="mikhael@uping.be"

WORKDIR /app
COPY . .

VOLUME c:\\app\\config
EXPOSE 6555/tcp
ENTRYPOINT ["dotnet", "EmbyStat.dll"]
