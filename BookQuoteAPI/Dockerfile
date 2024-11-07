# Base runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy and restore dependencies
COPY ["BookQuoteAPI/BookQuoteAPI.csproj", "BookQuoteAPI/"]
RUN dotnet restore "./BookQuoteAPI/BookQuoteAPI.csproj"

# Copy the rest of the app and build
COPY . .
WORKDIR "/src/BookQuoteAPI"
RUN dotnet build "./BookQuoteAPI.csproj" -c ${BUILD_CONFIGURATION} -o /app/build

# Publish stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BookQuoteAPI.csproj" -c ${BUILD_CONFIGURATION} -o /app/publish /p:UseAppHost=false

# Final production image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Set entrypoint for the app
ENTRYPOINT ["dotnet", "BookQuoteAPI.dll"]
