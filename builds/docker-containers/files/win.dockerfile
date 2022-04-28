FROM mcr.microsoft.com/dotnet/core/aspnet:6.0.4-nanoserver-20H2 AS base
LABEL author="UPing"
LABEL maintainer="mikhael@uping.be"

ENV EMBYSTAT_LISTENURL=http://*
ENV EMBYSTAT_PORT=6555

WORKDIR /app
COPY . .

VOLUME C:\\app\\data
EXPOSE 6555/tcp
ENTRYPOINT ["dotnet", "EmbyStat.dll", "--data-dir", "\\app\\data"]