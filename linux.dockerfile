FROM microsoft/dotnet:2.2.1-runtime as base
ENV LC_ALL=en_US.UTF-8
ENV LANG=en_US.UTF-8
ENV LANGUAGE=en_US.UTF-8

WORKDIR /app
ENV ASPNETCORE_URLS=http://*:5432
LABEL author="UPing"

COPY /publish .
ENTRYPOINT ["dotnet", "EmbyStat.Web.dll"] 