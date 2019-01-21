#Runtime image
FROM microsoft/dotnet:2.2.1-runtime AS base

WORKDIR /app
ENV ASPNETCORE_URLS=http://*:5432
LABEL author="UPing"

COPY /publish .
ENTRYPOINT ["dotnet", "EmbyStat.Web.dll"]