FROM microsoft/dotnet:2.2.1-runtime as base
ENV LC_ALL=en_US.UTF-8
ENV LANG=en_US.UTF-8
ENV LANGUAGE=en_US.UTF-8
LABEL author="UPing"

WORKDIR /app
COPY . .

VOLUME c:\\config
EXPOSE 6555/tcp
ENTRYPOINT ["dotnet", "EmbyStat.dll"] 
