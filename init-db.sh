#!/bin/bash

echo "Esperando a que PostgreSQL esté listo..."
sleep 5

echo "Generando migraciones..."
cd /src/PruebaAPI
dotnet ef migrations add InitialCreate --verbose

echo "Aplicando migraciones..."
dotnet ef database update --verbose

echo "Base de datos inicializada correctamente"
