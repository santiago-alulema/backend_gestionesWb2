## Imagen base para runtime
#FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
#WORKDIR /app
#
## Imagen para compilar
#FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
#WORKDIR /src
#COPY . .
#RUN dotnet restore gestiones_backend.sln
#RUN dotnet publish gestiones_backend.sln -c Release -o /app/publish
#
## Imagen final
#FROM base AS final
#WORKDIR /app
#COPY --from=build /app/publish .
#
#ENV ASPNETCORE_ENVIRONMENT=Production
#
#ENTRYPOINT ["dotnet", "gestiones_backend.dll"]
#

# syntax=docker/dockerfile:1.7

ARG DOTNET_VERSION=8.0

# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_VERSION} AS base
WORKDIR /app
ENV ASPNETCORE_ENVIRONMENT=Production \
    ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Build
FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION} AS build
WORKDIR /src

# 1) Copia sólo el csproj para maximizar caché
#    Ajusta la ruta si tu csproj no está en la raíz
COPY gestiones_backend.csproj ./
COPY packages.lock.json ./
RUN dotnet restore --locked-mode --nologo

# 2) Restore (se cachea si no cambian los PackageReference)
RUN dotnet restore --nologo --use-lock-file

# 3) Copia el resto del código
COPY . .

# 4) Publish del PROYECTO (no de la solución) y sin restore extra
RUN dotnet publish ./gestiones_backend.csproj -c Release -o /app/publish --no-restore

# Final
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "gestiones_backend.dll"]
