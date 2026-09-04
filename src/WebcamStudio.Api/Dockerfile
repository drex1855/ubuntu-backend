\
# ---- Etapa de build ----
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiamos primero solo los .csproj para aprovechar la cache de capas de Docker:
# si no cambian las dependencias, "dotnet restore" no se vuelve a ejecutar aunque
# cambie el codigo fuente.
COPY ["src/WebcamStudio.Domain/WebcamStudio.Domain.csproj", "src/WebcamStudio.Domain/"]
COPY ["src/WebcamStudio.Application/WebcamStudio.Application.csproj", "src/WebcamStudio.Application/"]
COPY ["src/WebcamStudio.Infrastructure/WebcamStudio.Infrastructure.csproj", "src/WebcamStudio.Infrastructure/"]
COPY ["src/WebcamStudio.Api/WebcamStudio.Api.csproj", "src/WebcamStudio.Api/"]
RUN dotnet restore "src/WebcamStudio.Api/WebcamStudio.Api.csproj"

COPY . .
RUN dotnet publish "src/WebcamStudio.Api/WebcamStudio.Api.csproj" -c Release -o /app/publish --no-restore

# ---- Etapa final (runtime, sin el SDK completo) ----
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "WebcamStudio.Api.dll"]
