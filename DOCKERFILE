# https://hub.docker.com/_/microsoft-dotnet-core
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY *.sln .
COPY ACPosterMaker.Server/*.csproj ./ACPosterMaker.Server/
RUN dotnet restore

# copy everything else and build app
COPY ACPosterMaker.Server/. ./ACPosterMaker.Server/
WORKDIR /source/ACPosterMaker.Server
RUN dotnet publish -c release -o /app

# final stage/image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
RUN apt-get update \
 && apt-get install -y cabextract wget xfonts-utils \
 && curl -s -o ttf-mscorefonts-installer_3.7_all.deb http://ftp.us.debian.org/debian/pool/contrib/m/msttcorefonts/ttf-mscorefonts-installer_3.7_all.deb \
 && sh -c "echo ttf-mscorefonts-installer msttcorefonts/accepted-mscorefonts-eula select true | debconf-set-selections" \
 && dpkg -i ttf-mscorefonts-installer_3.7_all.deb

WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "ACPosterMaker.Server.dll"]
