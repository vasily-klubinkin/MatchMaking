FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

COPY src/MatchMaking.Service.Host/*.csproj ./src/MatchMaking.Service.Host/
COPY src/MatchMaking.Service.Application/*.csproj ./src/MatchMaking.Service.Application/
COPY src/MatchMaking.Service.Infra/*.csproj ./src/MatchMaking.Service.Infra/

RUN dotnet restore src/MatchMaking.Service.Host/MatchMaking.Service.Host.csproj

COPY src/ ./src/

WORKDIR /app/src/MatchMaking.Service.Host
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish ./

ENTRYPOINT ["dotnet", "MatchMaking.Service.Host.dll"]
