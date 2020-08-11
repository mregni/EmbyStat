FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
LABEL author="UPing"
LABEL maintainer="mikhael@uping.be"

ENV LC_ALL=en_US.UTF-8
ENV LANG=en_US.UTF-8
ENV LANGUAGE=en_US.UTF-8
ENV EMBYSTAT_LISTENURL=http://*
ENV EMBYSTAT_PORT=6555

WORKDIR /app
COPY . .

VOLUME /app/data
EXPOSE 6555/tcp
ENTRYPOINT ["dotnet", "EmbyStat.dll", "--data-dir", "/app/data"]
