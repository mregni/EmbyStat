FROM microsoft/aspnetcore:2.0-nanoserver-1709 AS base
WORKDIR /app
EXPOSE 5123

FROM microsoft/aspnetcore-build:2.0-nanoserver-1709 AS build
WORKDIR /src
COPY EmbyStat.sln ./
COPY EmbyStat.Web/EmbyStat.Web.csproj EmbyStat.Web/
COPY EmbyStat.Controllers/EmbyStat.Controllers.csproj EmbyStat.Controllers/
COPY EmbyStat.Services/EmbyStat.Services.csproj EmbyStat.Services/
COPY Emby.ApiClient/Emby.ApiClient.csproj Emby.ApiClient/
COPY EmbyStat.Common/EmbyStat.Common.csproj EmbyStat.Common/
COPY EmbyStat.Repositories/EmbyStat.Repositories.csproj EmbyStat.Repositories/
RUN dotnet restore -nowarn:msb3202,nu1503
COPY . .
WORKDIR /src/EmbyStat.Web
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "EmbyStat.Web.dll"]
