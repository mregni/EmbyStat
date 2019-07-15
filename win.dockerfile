FROM microsoft/dotnet:2.2.1-runtime AS base
LABEL author="UPing"

WORKDIR /app
COPY . .
ENTRYPOINT ["dotnet", "EmbyStat.dll"]
