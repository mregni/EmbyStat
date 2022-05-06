FROM mcr.microsoft.com/dotnet/runtime:6.0-nanoserver-ltsc2022 AS base
LABEL author="UPing"
LABEL maintainer="mikhael@uping.be"

ENV EMBYSTAT_LISTENURL=http://*
ENV EMBYSTAT_PORT=6555

WORKDIR /app
COPY . .

VOLUME C:\\app\\data
EXPOSE 6555/tcp
ENTRYPOINT ["dotnet", "EmbyStat.dll", "--data-dir", "\\app\\data"]