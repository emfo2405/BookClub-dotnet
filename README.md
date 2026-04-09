# Bokklubb
En webbplats för en bokklubb skapades med ASP.NET Core MVC, ASP.NET Core Identity samt Entity Framework Core. Webbplatsen agerar som basen för en bokklubb där användare kan ta del av information och se vilken bok som läses just nu. 
Det finns även diskussionsforum samt recensionsformulär som användare kan använda. Applikationen har en admin som kan publicera 
böcker, information om kapitel samt författarprofiler. Den kan även radera eller uppdatera alla inlägg som skapats på webbplatsen.
För att en användare ska kunna lämna en recension eller delta  diskussionsforumet behöver den vara inloggad. Därefter kan den även 
redigera eller radera sina egna inlägg. En besökare som inte är inloggad kan läsa all information men kan inte interagera med formulär. 

### Tekniker som använts
För det här projektet har följande tekniker använts:
- ASP.NET Core MVC
- Entity Framework Core
- ASP.NET Identity
- Azure SQL Database
- OpenAI

## Sätta upp och köra projektet
För att köra projektet behöver följande kommando köras för att klona det:
```
git clone https://github.com/emfo2405/BookClub-dotnet.git
```
Därefter behöver en koppling till en databas skapas. Detta görs genom att definiera DefaultConnection i filen appsettings.json med en anslutningssträng.
I program.cs definieras även vilken typ av databas som används, den är satt till SQL-server initialt men kan ändras efter behov. 

För att starta applikationen kan sedan kommandot `dotnet run` eller `dotnet watch run` köras. 

### AI-moderering
För att använda AI-modereringen behöver en AI-modell samt en AI-nyckel definieras. Detta sätts till variablerna OpenAI--ApiKey 
och OpenAI--Model. För att inte läcka ut en nyckel kan dessa med fördel definieras med User Secrets. 


## Databas
I program.cs finns inställningar för hur databasen kopplas upp. Detta görs med ApplicationDbContext och som nämnt kan även typen av 
databas ändras där. För att skapa en koppling till databasen behöver en migration göras. Detta görs med följande kommandon: 
```
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### Seeding
I program.cs finns även en roll för admin definierad med ASP.NET Core Identity via en SetAdmin-funktion. Denna roll skapas automatiskt
när projektet startas. Där är en e-postadress till admin 
fördefinierad. För att sätta rollen på en annan användare behöver e-postadressen ändras och ett konto registreras på webbplatsen 
med den angivna e-postadressen. 

## Beroenden och konfiguration 
De paket som används i projektet är: 
- Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore
- Microsoft.AspNetCore.Identity.EntityFrameworkCore
- Microsoft.AspNetCore.Identity.UI
- Microsoft.EntityFrameworkCore.Sqlite eller Microsoft.EntityFrameworkCore.SqlServer (beror på vilken databas som används)
- Microsoft.EntityFrameworkCore.Tools
- Microsoft.VisualStudio.Web.CodeGeneration.Design
- OpenAI

