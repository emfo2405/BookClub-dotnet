# Bygg
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# Kopiera och återställ beroenden
COPY . ./
RUN dotnet publish -c Release -o /out

# Kör
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /out ./

ENTRYPOINT ["dotnet", "BookClub.dll"]