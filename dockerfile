# Imagen base para runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# Imagen para compilar
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore gestiones_backend.sln
RUN dotnet publish gestiones_backend.sln -c Release -o /app/publish

# Imagen final
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

# Instalar herramientas EF Core para migraciones
RUN apt-get update && apt-get install -y curl unzip \
    && dotnet tool install --global dotnet-ef \
    && export PATH="$PATH:/root/.dotnet/tools"

# Script de arranque: migraciones + API
COPY entrypoint.sh /entrypoint.sh
RUN chmod +x /entrypoint.sh

ENTRYPOINT ["/entrypoint.sh"]
