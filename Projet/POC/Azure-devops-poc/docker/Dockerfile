# ================================
#  STAGE 1 — Build .NET (SDK)
# ================================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Copy csproj first (layer caching)
COPY ./src/Api/Api.csproj ./Api/

# Restore
RUN dotnet restore ./Api/Api.csproj

# Copy all code
COPY ./src/Api ./Api

# Build + publish
RUN dotnet publish Api/Api.csproj -c Release -o /app/publish


# ================================
#  STAGE 2 — Runtime (ASP.NET)
# ================================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

WORKDIR /app

# Optional: Important for App Service Linux & Container Apps
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Copy published output
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Api.dll"]
