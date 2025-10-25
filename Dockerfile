# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY PruebaAPI/*.csproj ./PruebaAPI/
RUN dotnet restore "PruebaAPI/PruebaAPI.csproj"

# Copy everything else and build
COPY PruebaAPI/. ./PruebaAPI/
WORKDIR "/src/PruebaAPI"
RUN dotnet build "PruebaAPI.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "PruebaAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PruebaAPI.dll"]
