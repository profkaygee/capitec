FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["CapitecFraud.Api/CapitecFraud.Api.csproj", "CapitecFraud.Api/"]
RUN dotnet restore "CapitecFraud.Api/CapitecFraud.Api.csproj"
COPY . .
WORKDIR "/src/CapitecFraud.Api"
RUN dotnet build "./CapitecFraud.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CapitecFraud.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CapitecFraud.Api.dll"]
