## Software To Have Installed

 - [x] [Visual Studio Code](https://code.visualstudio.com/download)
 - [x] [Docker Desktop](https://www.docker.com/products/docker-desktop/)
 - [x] [Docker Compose](https://docs.docker.com/compose/)

## Steps to Run the Application

1. Clone the [repository](https://github.com/profkaygee/capitec)
2. Open the repository in [Visual Studio Code](https://code.visualstudio.com/download)
3. Open the Visual Studio Code terminal
4. Run `dotnet restore`
5. Run `dotnet build`
6. Run `dotnet ef database update \
   --project CapitecFraud.Infrastructure/CapitecFraud.Infrastructure.csproj \
   --startup-project CapitecFraud.Api/CapitecFraud.Api.csproj`
7. Run `docker-compose up --build`
 
## Troubleshooting

- If you get an error message saying that the `docker-compose.yml` file is not found,
  - make sure you are in the correct directory (src/CapitecFraudEngine)
- If you get an error message that says `You must install or update .NET to run this application.`,
  - make sure you have [.NET 10.0](https://dotnet.microsoft.com/download/dotnet/10.0) installed
  - verify that the docker file for the application is using the correct image (runtime vs aspnet)

## Developer Contact Details

- For any other issues, please contact the developer on this details
  - Email: ``profkagiso@gmail.com``
  - Mobile: ``0658688810``