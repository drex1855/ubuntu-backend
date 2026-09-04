# Webcam Studio - Backend

Backend en .NET 8 / ASP.NET Core Web API + MySQL para el sistema del estudio webcam
(WhatsApp de modelos, inventario de tienda, reporte de tokens, checklist de habitaciones
y cuentas de modelos). Ver **[docs/ARQUITECTURA.md](docs/ARQUITECTURA.md)** para la
documentacion completa (arquitectura, modulos, modelo de datos, endpoints, como agregar
un modulo nuevo, seguridad y limitaciones conocidas).

## Requisitos

- [.NET SDK 8.0](https://dotnet.microsoft.com/download/dotnet/8.0)
- MySQL 8.0+ o MariaDB 10.6+ (o Docker, ver mas abajo)
- (Opcional) [Docker](https://www.docker.com/) + Docker Compose, para levantar todo con un comando

## Arranque rapido con Docker (recomendado para probar)

```bash
docker compose up --build
```

Esto levanta MySQL y la API juntos. La API queda en `http://localhost:8080`, con
Swagger en `http://localhost:8080/swagger` (solo en modo Development, que es el
que usa docker-compose.yml por defecto). Al primer arranque:

1. Se aplican las migraciones de EF Core automaticamente (solo en Development).
2. Se crea una cuenta **Admin** inicial con el correo/contraseña definidos en
   `Seed:AdminEmail` / `Seed:AdminPassword` (ver `appsettings.json`, por defecto
   `admin@webcamstudio.local` / `ChangeMe123!`). **Cambia esa contraseña de inmediato**
   una vez que puedas crear otra cuenta Admin.

## Arranque manual (sin Docker)

```bash
# 1. Restaurar dependencias (necesita acceso normal a nuget.org)
dotnet restore

# 2. Copiar src/WebcamStudio.Api/appsettings.Development.json.example a
#    appsettings.Development.json (este ultimo esta en .gitignore, nunca se commitea)
#    y poner ahi tu cadena de conexion real y un Jwt:Secret aleatorio de dev.
#    En produccion, esos valores SIEMPRE deben venir de variables de entorno
#    (ConnectionStrings__Default, Jwt__Secret), nunca de un appsettings*.json commiteado.

# 3. Generar y aplicar las migraciones de EF Core (requiere la CLI de EF Core)
dotnet tool install --global dotnet-ef   # una sola vez
cd src/WebcamStudio.Api
dotnet ef migrations add InitialCreate --project ../WebcamStudio.Infrastructure --startup-project .
dotnet ef database update --project ../WebcamStudio.Infrastructure --startup-project .

# 4. Levantar la API
dotnet run --project src/WebcamStudio.Api
```

Si prefieres no usar la CLI de EF Core todavia, en `database/001_initial_schema.sql`
hay un script SQL escrito a mano equivalente al esquema (ver nota de por que existe
en la cabecera de ese archivo, y por que sigue siendo buena idea generar la migracion
real de EF Core en cuanto puedas).

## Estructura del repositorio

```
WebcamStudio.sln
src/
  WebcamStudio.Domain/          # Entidades y enums, sin dependencias externas
  WebcamStudio.Application/     # Casos de uso por modulo (DTOs, interfaces, logica de negocio)
  WebcamStudio.Infrastructure/  # EF Core, MySQL, JWT, hashing, auditoria
  WebcamStudio.Api/             # Controladores REST, Program.cs, Swagger
database/                       # Script SQL de referencia (ver docs/ARQUITECTURA.md)
docs/ARQUITECTURA.md            # Documentacion completa
docker-compose.yml
```

## Nota sobre esta primera entrega

Este backend se genero en un entorno sin acceso a `nuget.org` (solo se pudo instalar
el SDK de .NET vía los repositorios de Ubuntu). Se verificó que la capa `Domain`
compila limpio de forma aislada; el resto de las capas (`Application`, `Infrastructure`,
`Api`) se escribieron con el mismo cuidado pero **no se pudieron compilar de punta a
punta en este entorno** por esa restricción de red. El primer `dotnet restore` /
`dotnet build` que corras en tu máquina (con internet normal) es el que confirma que
todo compila — revisa `docs/ARQUITECTURA.md` → "Limitaciones conocidas" para el detalle.
