# syntax=docker/dockerfile:1

# Start with .NET 9.0 SDK image (contains only .NET 9.0 by default)
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

# Install .NET 8.0 SDK and runtime
RUN apt-get update && \
    apt-get install -y wget && \
    wget https://dotnet.microsoft.com/download/dotnet/scripts/v1/dotnet-install.sh && \
    bash dotnet-install.sh --version 8.0.100 --install-dir /usr/share/dotnet && \
    rm dotnet-install.sh && \
    rm -rf /var/lib/apt/lists/*

# Ensure all installed SDKs and runtimes are discoverable
ENV PATH="/usr/share/dotnet:${PATH}"
ENV DOTNET_ROOT=/usr/share/dotnet

# Set up working directory
WORKDIR /app

# Copy global.json to pin SDK version (if present)
COPY global.json ./

# Copy solution and project files
COPY *.sln ./
COPY src/ ./src/
COPY tests/ ./tests/
COPY Directory.* ./
COPY .config ./

# Restore dependencies for all projects
RUN dotnet restore

# Build all projects for Release configuration
RUN dotnet build --configuration Release -v minimal --no-restore

# Run tests using the built artifacts in Release configuration
RUN dotnet test --no-build --configuration Release -v minimal
