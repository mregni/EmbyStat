FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
LABEL author="UPing"
LABEL maintainer="mikhael@uping.be"
ENV ASPNETCORE_URLS=http://*:6555

WORKDIR /app
COPY . .

VOLUME C:\\app\\data
EXPOSE 6555
ENTRYPOINT ["dotnet", "EmbyStat.dll", "--data-dir", "\\app\\data"]