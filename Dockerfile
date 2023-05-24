# Learn about building .NET container images:
# https://github.com/dotnet/dotnet-docker/blob/main/samples/README.md
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY *.sln .
COPY nuget.config .
COPY HerPublicWebsite/*.csproj HerPublicWebsite/
COPY HerPublicWebsite.BusinessLogic/*.csproj HerPublicWebsite.BusinessLogic/
COPY HerPublicWebsite.Data/*.csproj HerPublicWebsite.Data/
COPY Lib/ Lib/
RUN dotnet restore HerPublicWebsite/ --use-current-runtime

# copy and publish app and libraries
COPY . .
RUN dotnet publish HerPublicWebsite/ --use-current-runtime --self-contained false --no-restore -o /app


# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "HerPublicWebsite.dll"]
