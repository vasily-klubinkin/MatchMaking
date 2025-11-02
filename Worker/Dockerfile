FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

COPY src/MatchMaking.Worker.Host/*.csproj ./src/MatchMaking.Worker.Host/
COPY src/MatchMaking.Worker.Application/*.csproj ./src/MatchMaking.Worker.Application/
COPY src/MatchMaking.Worker.Infra/*.csproj ./src/MatchMaking.Worker.Infra/

RUN dotnet restore src/MatchMaking.Worker.Host/MatchMaking.Worker.Host.csproj

COPY src/ ./src/

WORKDIR /app/src/MatchMaking.Worker.Host
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish ./

ENTRYPOINT ["dotnet", "MatchMaking.Worker.Host.dll"]
