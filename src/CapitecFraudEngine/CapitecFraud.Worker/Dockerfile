FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY . .
RUN dotnet restore src/CapitecFraudEngine/CapitecFraud.Worker/CapitecFraud.Worker.csproj
RUN dotnet publish src/CapitecFraudEngine/CapitecFraud.Worker/CapitecFraud.Worker.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

COPY --from=build /app .

ENTRYPOINT ["dotnet", "CapitecFraud.Worker.dll"]