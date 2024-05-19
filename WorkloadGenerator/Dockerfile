# Use the official image as the base image
FROM mcr.microsoft.com/dotnet/runtime:latest as base
WORKDIR /app

# Copy csproj and restore as distinct layers
FROM mcr.microsoft.com/dotnet/sdk:latest as build
WORKDIR /src
COPY ["WorkloadGenerator.csproj", "./"]
RUN dotnet restore "./WorkloadGenerator.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/."
RUN dotnet build "WorkloadGenerator.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WorkloadGenerator.csproj" -c Release -o /app/publish

# Build runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WorkloadGenerator.dll"]
