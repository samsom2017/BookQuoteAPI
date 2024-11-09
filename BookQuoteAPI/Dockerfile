# Base image for runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Build image with SDK
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Update the path to reflect the actual location of BookQuoteAPI.csproj
COPY ["BookQuoteAPI/BookQuoteAPI.csproj", "BookQuoteAPI/"]
RUN dotnet restore "BookQuoteAPI/BookQuoteAPI.csproj"

# Copy the entire project
COPY . .
WORKDIR "/src/BookQuoteAPI"
RUN dotnet build "BookQuoteAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish the application
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "BookQuoteAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage for running the application
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BookQuoteAPI.dll"]
