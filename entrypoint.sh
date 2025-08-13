#!/bin/bash
set -e

echo "Esperando a que Postgres esté listo..."
until dotnet ef database update --project GestionesBackend; do
  >&2 echo "Postgres no está listo, reintentando en 5 segundos..."
  sleep 5
done

echo "Migraciones aplicadas correctamente."

exec dotnet GestionesBackend.dll
