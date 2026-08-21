# Stage 1: Base runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_HTTP_PORTS=8080

# Stage 2: Build & Restore
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files for optimal layer caching
COPY ["MiniSiniestros.Api/MiniSiniestros.Api.csproj", "MiniSiniestros.Api/"]
COPY ["MiniSiniestros.Services/MiniSiniestros.Services.csproj", "MiniSiniestros.Services/"]
COPY ["MiniSiniestros.Data/MiniSiniestros.Data.csproj", "MiniSiniestros.Data/"]
COPY ["MiniSiniestros.Data.Migrations/MiniSiniestros.Data.Migrations.csproj", "MiniSiniestros.Data.Migrations/"]
COPY ["MiniSiniestros.Models/MiniSiniestros.Entities.csproj", "MiniSiniestros.Models/"]
COPY ["MiniSiniestros.Dto/MiniSiniestros.Dto.csproj", "MiniSiniestros.Dto/"]
COPY ["MiniSiniestros.Common/MiniSiniestros.Common.csproj", "MiniSiniestros.Common/"]
COPY ["MiniSiniestros.ViewModels/MiniSiniestros.ViewModels.csproj", "MiniSiniestros.ViewModels/"]

RUN dotnet restore "MiniSiniestros.Api/MiniSiniestros.Api.csproj"

# Copy all source files and build
COPY . .
WORKDIR "/src/MiniSiniestros.Api"
RUN dotnet build "MiniSiniestros.Api.csproj" -c Release -o /app/build

# Stage 3: Publish
FROM build AS publish
RUN dotnet publish "MiniSiniestros.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 4: Final Image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MiniSiniestros.Api.dll"]
