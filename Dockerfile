# Learn about building .NET container images:
# https://github.com/dotnet/dotnet-docker/blob/main/samples/README.md
FROM public.ecr.aws/docker/library/node:18 AS node_base
COPY WhlgPublicWebsite /WhlgPublicWebsite
WORKDIR /WhlgPublicWebsite
RUN npm ci
RUN npm run build

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

COPY --from=node_base . .
# copy csproj and restore as distinct layers
COPY *.sln .
COPY nuget.config .
COPY WhlgPublicWebsite/*.csproj WhlgPublicWebsite/
COPY WhlgPublicWebsite.BusinessLogic/*.csproj WhlgPublicWebsite.BusinessLogic/
COPY WhlgPublicWebsite.Data/*.csproj WhlgPublicWebsite.Data/
COPY WhlgPublicWebsite.ManagementShell/*.csproj WhlgPublicWebsite.ManagementShell/
COPY Lib/ Lib/
RUN dotnet restore WhlgPublicWebsite/ --use-current-runtime
RUN dotnet restore WhlgPublicWebsite.ManagementShell/ --use-current-runtime

# copy and publish app and libraries
COPY . .
RUN dotnet publish WhlgPublicWebsite/ --use-current-runtime --self-contained false --no-restore -o /app
RUN dotnet build WhlgPublicWebsite.ManagementShell/ --use-current-runtime --self-contained false --no-restore -o /cli


# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
RUN groupadd -r appuser && useradd -r -g appuser appuser
USER appuser

WORKDIR /app
COPY --from=build /app .
COPY --from=build /cli ./cli

# this ensures psql is available on the container
# we may use this in support when connecting to EC2 container & we need to query the database
RUN apt-get update && apt-get install -y postgresql-client

ENTRYPOINT ["dotnet", "WhlgPublicWebsite.dll"]
