# Stage 1: Base runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_HTTP_PORTS=8080

# Stage 2: Build & Restore
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files for optimal layer caching
COPY ["MiniSiniestros.SrtMock/MiniSiniestros.SrtMock.csproj", "MiniSiniestros.SrtMock/"]
COPY ["MiniSiniestros.Dto/MiniSiniestros.Dto.csproj", "MiniSiniestros.Dto/"]
COPY ["MiniSiniestros.Common/MiniSiniestros.Common.csproj", "MiniSiniestros.Common/"]

RUN dotnet restore "MiniSiniestros.SrtMock/MiniSiniestros.SrtMock.csproj"

# Copy all source files and build
COPY . .
WORKDIR "/src/MiniSiniestros.SrtMock"
RUN dotnet build "MiniSiniestros.SrtMock.csproj" -c Release -o /app/build

# Stage 3: Publish
FROM build AS publish
RUN dotnet publish "MiniSiniestros.SrtMock.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 4: Final Image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MiniSiniestros.SrtMock.dll"]
