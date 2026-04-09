# Bygg
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# Kopiera och återställ beroenden
COPY . ./
RUN dotnet publish -c Release -o /out

# Kör
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

#Installera bibliotek
RUN apt-get update && apt-get install -y libgssapi-krb5-2 && rm -rf /var/lib/apt/lists/*

COPY --from=build /out ./

ENTRYPOINT ["dotnet", "BookClub.dll"]