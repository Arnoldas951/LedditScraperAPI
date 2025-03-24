# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["LedditScraperAPI.csproj", "."]
RUN dotnet restore "./LedditScraperAPI.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "./LedditScraperAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./LedditScraperAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
USER root
RUN apk update && apk add --no-cache \
    wget \
    gnupg \
    chromium \
    chromium-chromedriver \
    ca-certificates \
    ttf-freefont \
    freetype \
    fontconfig \
    curl \
    unzip \
    udev \
    dbus

RUN rm -rf /var/cache/apk/* /tmp/*


RUN addgroup -S appuser && adduser -S appuser -G appuser
RUN chown -R appuser:appuser /app

ENV CHROME_OPTIONS="--headless --disable-gpu --no-sandbox --disable-dev-shm-usage"

COPY --from=publish /app/publish .

RUN chmod +x /app/selenium-manager/linux/selenium-manager
RUN chown -R appuser:appuser /app/selenium-manager
USER appuser
ENTRYPOINT ["dotnet", "LedditScraperAPI.dll"]