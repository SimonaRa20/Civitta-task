# Set up the runtime environment
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Set up the build environment
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the project file from the correct directory
COPY ["PublicHolidayAPI/PublicHolidayAPI.csproj", "PublicHolidayAPI/"]

# Set the working directory to where the .csproj file is located
WORKDIR /src/PublicHolidayAPI

# Restore dependencies
RUN dotnet restore

# Copy the remaining source code
COPY . .

# Build the project
ARG BUILD_CONFIGURATION=Release
RUN dotnet build "PublicHolidayAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish the build artifacts
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "PublicHolidayAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage - runtime environment
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PublicHolidayAPI.dll"]
