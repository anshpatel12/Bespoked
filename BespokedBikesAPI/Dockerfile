# Base image for running the application
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# Ensure the correct port is exposed for Render
EXPOSE 8080

# Build stage: Used to compile the project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy the .csproj file and restore dependencies
COPY ["BespokedBikesAPI.csproj", "."]
RUN dotnet restore "./BespokedBikesAPI.csproj"

# Copy the full project source and build
COPY . .
WORKDIR "/src/."
RUN dotnet build "./BespokedBikesAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish stage: Optimized for deployment
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BespokedBikesAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage: Production-ready container
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BespokedBikesAPI.dll"]
