# Learn about building .NET container images:
# https://github.com/dotnet/dotnet-docker/blob/main/samples/README.md
FROM public.ecr.aws/docker/library/node:18 AS node_base
COPY HerPublicWebsite /HerPublicWebsite
WORKDIR /HerPublicWebsite
RUN npm ci
RUN npm run build

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /source

COPY --from=node_base . .
# copy csproj and restore as distinct layers
COPY *.sln .
COPY nuget.config .
COPY HerPublicWebsite/*.csproj HerPublicWebsite/
COPY HerPublicWebsite.BusinessLogic/*.csproj HerPublicWebsite.BusinessLogic/
COPY HerPublicWebsite.Data/*.csproj HerPublicWebsite.Data/
COPY HerPublicWebsite.ManagementShell/*.csproj HerPublicWebsite.ManagementShell/
COPY Lib/ Lib/
RUN dotnet restore HerPublicWebsite/ --use-current-runtime
RUN dotnet restore HerPublicWebsite.ManagementShell/ --use-current-runtime

# copy and publish app and libraries
COPY . .
RUN dotnet publish HerPublicWebsite/ --use-current-runtime --self-contained false --no-restore -o /app
RUN dotnet build HerPublicWebsite.ManagementShell/ --use-current-runtime --self-contained false --no-restore -o /cli


# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app .
COPY --from=build /cli ./cli
ENTRYPOINT ["dotnet", "HerPublicWebsite.dll"]
