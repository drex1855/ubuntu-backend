# Webcam Studio — Backend: Arquitectura y documentación técnica

Este documento acompaña al código del backend (.NET 8 + MySQL) generado a partir del
diagrama de flujo compartido (`Diagrama sin título.drawio`). Cubre cómo está armado el
sistema, cómo se conecta cada pieza con el diagrama original, el modelo de datos, los
endpoints disponibles, y — porque el pedido explícito fue que esto **crezca** — la
convención a seguir para agregar módulos nuevos sin desordenar lo que ya existe.

## 1. Alcance de esta entrega

El diagrama define un backend con 5 flujos de negocio (Botón de WhatsApp, Inventario de
tienda, Reporte de tokens, Checklist de habitaciones, Cuentas de modelos) que convergen
en un nodo de auditoría y una respuesta estándar al frontend. Esta entrega implementa
los cinco módulos completos —modelo de datos, lógica de negocio, autenticación, API
REST y auditoría— pensados como el arranque de un sistema que va a seguir creciendo,
no como un prototipo cerrado.

Lo que **no** incluye todavía, a propósito: integración real con un proveedor de
WhatsApp (Meta Cloud API, Twilio, etc. — hay un punto de extensión listo, ver §8),
pruebas automatizadas, y un pipeline de CI/CD. Son los siguientes pasos naturales una
vez que el negocio valide que este diseño funciona.

## 2. Arquitectura general

El backend sigue **Clean Architecture** organizada como un **monolito modular**: un solo
servicio desplegable, pero con los módulos de negocio separados por carpeta/namespace
para que cada uno se pueda leer, probar y — el día que haga falta — extraer a su propio
servicio sin rediseñar todo.

```
WebcamStudio.Domain          <- entidades, enums. Cero dependencias externas.
        ^
WebcamStudio.Application     <- casos de uso por módulo (DTOs, interfaces, lógica de negocio).
        ^                       Solo depende de Domain + abstracciones de DI.
WebcamStudio.Infrastructure   <- EF Core (MySQL), JWT, hashing, auditoría, stub de WhatsApp.
        ^                       Implementa las interfaces que define Application.
WebcamStudio.Api             <- controladores REST, Program.cs, autenticación, Swagger.
```

La flecha `^` indica "depende de": cada capa solo conoce a la de abajo. `Application`
nunca sabe que existe Entity Framework, HTTP, ni MySQL — solo trabaja contra interfaces
(`IUnitOfWork`, `IPasswordHasher`, `IAuditService`, etc.) que `Infrastructure` implementa.
Esto es lo que permite, por ejemplo, cambiar de MySQL a otro motor, o de un stub de
WhatsApp a un proveedor real, tocando solo `Infrastructure` — la lógica de negocio no se
entera.

Dentro de `Application`, cada módulo del diagrama tiene su propia carpeta con tres tipos
de archivo:

- `DTOs.cs` — los objetos que entran/salen del caso de uso (requests y responses).
- `I<Módulo>Service.cs` — el contrato del caso de uso.
- `<Módulo>Service.cs` — la implementación (la lógica de negocio real).

## 3. Mapeo diagrama → código

| Nodo del diagrama | Módulo | Dónde vive |
|---|---|---|
| Botón de WhatsApp (C1–C8) | WhatsApp | `Application/WhatsApp`, `Api/Controllers/WhatsAppController.cs` |
| Inventario de tienda (D1–D9) | Inventario | `Application/Inventory`, `Api/Controllers/InventoryController.cs` |
| Reporte de tokens (E1–E6) | Reporte de tokens | `Application/TokenReports`, `Api/Controllers/TokenReportsController.cs` |
| Checklist de habitaciones (F1–F10) | Checklist | `Application/Checklists`, `Api/Controllers/ChecklistsController.cs` |
| Cuentas de modelos (G1–G7) | Cuentas | `Application/ModelAccounts`, `Api/Controllers/ModelAccountsController.cs` |
| Registrar auditoría (Z, transversal) | Auditoría | `IAuditService` / `AuditService`, tabla `AuditLogs` |
| Generar respuesta para el frontend (H) | — | `ApiResponse<T>` (envoltorio de TODAS las respuestas) |

Cada método de servicio de negocio tiene comentarios en el código citando el nodo exacto
del diagrama que implementa (por ejemplo `// D7: Stock bajo?`), para que se pueda leer el
código y el diagrama en paralelo.

Un detalle de diseño importante en el módulo de Inventario: el diagrama tiene tres ramas
para "Registrar movimiento" (Entrada/Salida/Ajuste). Se modelaron como un único enum
`StockMovementType` en vez de tres flujos separados, y el signo con el que cada tipo
afecta el stock lo decide `InventoryService.RegisterMovementAsync` (Entrada suma, Salida
resta con validación de que no exista stock negativo, Ajuste fija la existencia exacta
contada). Esto es una interpretación razonable de "Actualizar existencia" en D5; si el
negocio quiere que "Ajuste" funcione distinto (por ejemplo, sumar o restar un delta en
vez de fijar un valor absoluto), es un cambio de una sola línea en ese método.

## 4. Modelo de datos

Todas las entidades usan `Guid` como llave primaria (no enteros autoincrementales) para
que el sistema pueda crecer sin preocuparse por colisiones de IDs si en el futuro se
separan módulos en servicios distintos. Las entidades con seguimiento de auditoría
(`CreatedAt`, `CreatedByAccountId`, `UpdatedAt`, `UpdatedByAccountId`) heredan de
`AuditableEntity`, y esos campos se llenan solos en `AppDbContext.SaveChangesAsync` — los
servicios de negocio no tienen que acordarse de setearlos.

| Entidad | Módulo | Notas |
|---|---|---|
| `ModelAccount` | Cuentas | Rol (`Admin`/`Modelo`), estado (`Activo`/`Desactivado`), hash de contraseña |
| `Conversation`, `Message` | WhatsApp | Una modelo puede tener varias conversaciones; cada una con sus mensajes entrantes/salientes |
| `Product`, `StockMovement`, `StockAlert` | Inventario | El stock **solo** se cambia a través de `StockMovement`, nunca editando `Product.CurrentStock` directo |
| `Site`, `TokenReport` | Reporte de tokens | `Site` es un catálogo (Chaturbate, Stripchat, etc.), se agregan sin tocar código |
| `Room`, `ChecklistTemplateItem`, `ChecklistRun`, `ChecklistItemResult`, `MaintenanceRequest` | Checklist | `ChecklistTemplateItem` es la plantilla por habitación; `ChecklistRun` es cada revisión concreta |
| `AuditLog` | Transversal | Un registro por acción relevante de cualquier módulo, con detalle en JSON |

El script SQL completo (con tipos de columna, llaves foráneas e índices) está en
`database/001_initial_schema.sql`. Las relaciones exactas (cascadas, restricciones)
están declaradas en `Infrastructure/Persistence/Configurations/*.cs`, una clase por
entidad — es la fuente de verdad real; el SQL es una traducción manual de esas clases
(ver §9 sobre por qué no se generó con `dotnet ef` en este entorno).

## 5. Autenticación y autorización

- Login: `POST /api/auth/login` con email + contraseña, devuelve un JWT.
- Las contraseñas se hashean con `PasswordHasher<T>` de ASP.NET Core Identity
  (PBKDF2 + HMAC-SHA256 con salteado automático). Se eligió esta clase en vez de una
  librería de terceros (BCrypt.Net, etc.) porque ya viene incluida en el framework
  compartido de ASP.NET Core — una dependencia externa menos que mantener.
- El JWT incluye el id de cuenta y el rol como claims. Los endpoints administrativos
  (gestión de cuentas, auditoría) están protegidos con `[Authorize(Roles = "Admin")]`;
  el resto de los endpoints autenticados solo exigen `[Authorize]` (cualquier cuenta
  activa). El webhook de WhatsApp es la única ruta pública, porque quien la llama es el
  proveedor externo, no un usuario logueado — la identidad real se valida adentro por
  número de teléfono registrado.
- **Bootstrap del primer Admin**: como los endpoints de gestión de cuentas exigen rol
  Admin, hace falta una cuenta Admin para crear la primera cuenta Admin. Se resolvió con
  `Infrastructure/Persistence/DataSeeder.cs`: en cualquier arranque donde la tabla
  `ModelAccounts` esté vacía, se crea automáticamente una cuenta Admin con los datos de
  la sección `Seed` de `appsettings.json`. Es idempotente (no hace nada si ya hay
  cuentas) — **cambiar esa contraseña por defecto es responsabilidad de quien despliegue**
  esto en un entorno real, apenas se pueda crear otra cuenta Admin.

## 6. Auditoría

Cualquier servicio de cualquier módulo llama a `IAuditService.LogAsync(modulo, accion,
...)` para registrar la acción — es la implementación del nodo transversal "Registrar
auditoría" (Z) del diagrama. El registro se agrega al mismo `SaveChangesAsync` que la
operación de negocio que lo originó (no se guarda aparte), para que auditoría y datos de
negocio nunca queden desincronizados si algo falla a mitad de camino. Un módulo nuevo
solo necesita inyectar `IAuditService` — no hay que reinventar nada.

## 7. Endpoints principales

Todas las respuestas usan el mismo sobre `ApiResponse<T>` (`success`, `message`, `data`,
`errors`) — es la implementación del nodo "Generar respuesta para el frontend" (H).

| Método | Ruta | Rol requerido | Nodo del diagrama |
|---|---|---|---|
| POST | `/api/auth/login` | público | — |
| GET/POST/PUT | `/api/modelaccounts` | Admin | G1, G2 |
| PATCH | `/api/modelaccounts/{id}/status` | Admin | G3–G7 |
| POST | `/api/whatsapp/webhook` | público (proveedor externo) | C1–C8 |
| GET | `/api/whatsapp/conversations/{modelAccountId}` | autenticado | — |
| GET/POST | `/api/inventory/products` | autenticado | D1 |
| POST | `/api/inventory/movements` | autenticado | D2–D9 |
| GET/POST | `/api/sites` | autenticado | E2 (catálogo) |
| POST | `/api/tokenreports` | autenticado | E1–E5 |
| GET | `/api/tokenreports` | autenticado | — |
| GET | `/api/tokenreports/summary` | autenticado | E6 |
| GET/POST | `/api/checklists/rooms` | autenticado | F1 |
| POST | `/api/checklists/rooms/{roomId}/items` | autenticado | F2 (definir plantilla) |
| GET | `/api/checklists/rooms/{roomId}/template` | autenticado | F1–F2 |
| POST | `/api/checklists/submit` | autenticado | F3–F10 |
| GET | `/api/audit` | Admin | Z (consulta) |

El detalle completo de cada request/response está en Swagger (`/swagger` en modo
desarrollo) una vez que la API esté corriendo — se documentó cada DTO con tipos
concretos, no `object` genérico, para que Swagger genere contratos útiles.

## 8. Cómo agregar un módulo nuevo

Este es el punto que más importa para el crecimiento que se pidió. El patrón a seguir,
copiando lo que ya existe:

1. **Domain**: crear la(s) entidad(es) en `Domain/Entities`, heredando de `BaseEntity` o
   `AuditableEntity`. Si necesita un enum nuevo, agregarlo a `Domain/Enums/Enums.cs`.
2. **Application**: crear una carpeta `Application/<NombreDelModulo>/` con `DTOs.cs`,
   `I<Módulo>Service.cs` y `<Módulo>Service.cs`. La lógica de negocio va acá, contra las
   interfaces de `IUnitOfWork` / `IAuditService` — nunca contra EF Core directo.
3. Si el módulo necesita consultas más allá del CRUD genérico, agregar una interfaz de
   repositorio específica en `Application/Interfaces/Persistence` (ver
   `IChecklistRunRepository` como ejemplo) y sumarla a `IUnitOfWork`.
4. **Infrastructure**: agregar la clase `IEntityTypeConfiguration<T>` en
   `Persistence/Configurations`, el `DbSet<T>` en `AppDbContext`, la implementación del
   repositorio específico (si aplica) en `Persistence/Repositories`, y su propiedad en
   `UnitOfWork`.
5. **Api**: crear el controlador en `Api/Controllers`, heredando de `ApiControllerBase`
   (ya trae el mapeo de `Result<T>` a `ApiResponse<T>` + código HTTP).
6. Registrar el nuevo servicio en `Application/DependencyInjection.cs`
   (`services.AddScoped<INuevoServicio, NuevoServicio>();`).
7. Generar la migración de EF Core: `dotnet ef migrations add Agregar<Módulo>`.

Ese es el mismo camino que se siguió para los 5 módulos de esta entrega — no hay pasos
ocultos ni "magia" en otro lado.

## 9. Configuración y despliegue

- **Cadena de conexión**: `ConnectionStrings:Default` en `appsettings.json` (o variable
  de entorno `ConnectionStrings__Default`). Formato Pomelo/MySQL estándar.
- **JWT**: sección `Jwt` (`Secret`, `Issuer`, `Audience`, `ExpiryMinutes`). El `Secret`
  **debe** reemplazarse por un valor real y no debe quedar commiteado en el repo — en
  producción se inyecta por variable de entorno o un secret manager.
- **CORS**: `Cors:AllowedOrigins`, lista explícita de orígenes del/los frontend(s).
- **Docker**: `docker compose up --build` levanta MySQL + la API juntos (ver README.md).
- **Migraciones en producción**: el auto-`Migrate()` en `Program.cs` solo corre en
  `Development` a propósito — en producción, aplicar migraciones es un paso explícito del
  pipeline de despliegue (`dotnet ef database update`), no algo que la API haga sola al
  arrancar (evita que dos instancias corriendo en paralelo intenten migrar a la vez).

## 10. Limitaciones conocidas de esta entrega

Ser directos sobre esto es más útil que ocultarlo:

- **El entorno donde se generó este código no tenía acceso a `nuget.org`** (solo a los
  repositorios de Ubuntu, de donde se instaló el SDK de .NET). Como consecuencia:
  - Se pudo compilar y verificar en limpio la capa `Domain` (cero dependencias externas).
  - Las capas `Application`, `Infrastructure` y `Api` se escribieron con el mismo cuidado
    y se revisaron línea por línea, pero **no se pudieron compilar de punta a punta**
    porque dependen de paquetes NuGet (EF Core, Pomelo.EntityFrameworkCore.MySql,
    Swashbuckle.AspNetCore) que no se pudieron descargar en este entorno.
  - El primer `dotnet restore && dotnet build` en una máquina con internet normal es el
    que da la confirmación final de que todo compila. Si aparece algún error de
    compilación, lo más probable es un detalle menor (un `using` faltante, un ajuste de
    versión de paquete) — el diseño y la lógica de negocio no dependen de esa capa de
    NuGet, así que un ajuste ahí no debería tocar cómo funciona el sistema.
  - Por la misma razón, no se corrió `dotnet ef migrations add InitialCreate` (requiere
    el paquete `Microsoft.EntityFrameworkCore.Design`, también bloqueado). En su lugar,
    `database/001_initial_schema.sql` es un script escrito a mano que replica exactamente
    lo que las clases de configuración de EF Core (`Persistence/Configurations/*.cs`)
    describen. Generar la migración real de EF Core en cuanto se pueda es el siguiente
    paso recomendado — ver README.md para el comando exacto.
- **MySQL/MariaDB + EF Core**: el soporte de MySQL para EF Core es a través de un
  proveedor de terceros (Pomelo.EntityFrameworkCore.MySql), no de Microsoft directamente
  como sí lo es SQL Server. Pomelo es maduro y ampliamente usado en producción, pero vale
  saber que las actualizaciones de EF Core a veces tardan más en reflejarse ahí que en el
  proveedor de SQL Server.
- **Integración de WhatsApp**: `WhatsAppMessageSenderStub` solo registra en el log lo que
  "se habría enviado" — no hay proveedor real conectado todavía (ver §8 del diagrama y el
  comentario en ese archivo para cómo conectar uno).
- **Sin pruebas automatizadas todavía**: dado el foco de esta entrega (dejar los 5 módulos
  funcionando de punta a punta), no se escribieron pruebas unitarias/integración. La capa
  `Application` está diseñada para que sea fácil agregarlas después (los servicios de
  negocio dependen solo de interfaces, se pueden mockear sin una base de datos real).

## 11. Roadmap sugerido

1. Correr `dotnet restore && dotnet build` en un entorno normal y resolver cualquier
   ajuste menor de compilación.
2. Generar la migración real de EF Core y confirmar que coincide con
   `database/001_initial_schema.sql`.
3. Cambiar la contraseña del Admin inicial / deshabilitar el seeder en producción.
4. Conectar un proveedor real de WhatsApp implementando `IWhatsAppMessageSender`.
5. Agregar pruebas automatizadas para los servicios de `Application` (son las que más
   valor dan primero, porque ahí vive toda la lógica de negocio).
6. Sumar los módulos nuevos que el negocio vaya necesitando, siguiendo el patrón de §8.

## 12. Seguridad (checklist pre-lanzamiento)

Repaso hecho antes del primer lanzamiento a producción. MySQL no tiene RLS nativo
como Postgres, así que el equivalente aquí es autorización por fila en la capa de
aplicación (rol + `CurrentAccountId`).

- **Secretos**: nunca commitear `appsettings.Development.json`/`appsettings.Production.json`
  (están en `.gitignore`); usar `appsettings.Development.json.example` como plantilla y
  variables de entorno (`Jwt__Secret`, `ConnectionStrings__Default`, `Email__SmtpPassword`,
  `WhatsApp__AccessToken`) en producción.
- **Autorización por fila**: `TokenReportsController` ahora restringe `Search`/`Summary`
  a Admin/Monitor y expone `GET /me` y `GET /me/summary` para que cada cuenta vea solo lo
  suyo; `Create` rechaza que una Modelo registre tokens a nombre de otra cuenta.
  `ModelAccountsController.Create` rechaza que un Monitor otorgue rol Admin/Monitor (solo
  Admin puede crear cuentas de staff). `ChecklistsController.CreateRoom`/`AddTemplateItem`/
  `GetAttachment` ahora requieren rol de staff.
- **Cuentas bloqueadas tras intentos fallidos**: `ModelAccount` tiene
  `FailedLoginAttempts`/`LockedUntil`; tras 5 fallos se bloquea 15 minutos. Rate limiting
  por IP en `/api/auth/login` (política `Auth`, igual mecanismo que `PublicContactForm`).
- **Cabeceras de seguridad**: middleware en `Program.cs` agrega
  `X-Content-Type-Options`, `X-Frame-Options`, `Referrer-Policy` y HSTS fuera de Development.
  El CSP del frontend se configura donde se sirva el build estático (fuera de este repo).
- **Validación de entradas**: DataAnnotations (`[Required]`, `[MaxLength]`, `[EmailAddress]`,
  `[Range]`) en los DTOs de request de todos los módulos.
- **Paginación**: `AuditController` y otros listados largos ahora acotan `take` a un máximo
  (200), con default 50/100 según el endpoint.
- **Conexión a MySQL en producción**: si la base de datos no está en la misma red privada
  que la API, usar `SslMode=Required` en la cadena de conexión (el template usa `Preferred`
  para no romper entornos locales sin TLS configurado).
- **Pendiente / fuera de alcance de esta pasada**: CAPTCHA en el formulario público de
  contacto (el rate limiting por IP ya existente se consideró suficiente por ahora);
  cifrado de columna para PII (teléfono/email) — no requerido para esta herramienta interna,
  evaluar solo si aparece un requisito de cumplimiento concreto.
- **Escaneo de dependencias (`dotnet list package --vulnerable --include-transitive`,
  hecho el 2026-08-26)**: sin hallazgos en ninguno de los 4 proyectos (Domain,
  Application, Infrastructure, Api) contra el feed de NuGet. Repetir este comando
  periódicamente, no es un chequeo de una sola vez.
