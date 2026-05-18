FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY . .
RUN dotnet restore src/CapitecFraudEngine/CapitecFraud.Api/CapitecFraud.Api.csproj
RUN dotnet publish src/CapitecFraudEngine/CapitecFraud.Api/CapitecFraud.Api.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

COPY --from=build /app .

ENV ASPNETCORE_ENVIRONMENT=Development
ENV DOTNET_USE_POLLING_FILE_WATCHER=1

ENTRYPOINT ["dotnet", "CapitecFraud.Api.dll"]