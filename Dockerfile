FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY DigitalBanking.API.csproj ./
RUN dotnet restore DigitalBanking.API.csproj

COPY . ./
RUN dotnet publish DigitalBanking.API.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish ./

EXPOSE 8080
ENTRYPOINT ["dotnet", "DigitalBanking.API.dll"]
