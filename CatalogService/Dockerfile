FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR "/CatalogService"

COPY "Application/Application.csproj" "CatalogService.Application/"
COPY "Domain/Domain.csproj" "CatalogService.Domain/"
COPY "Infrastructure/Persistance.csproj" "CatalogService.Persistance/"
COPY "Web/Web.csproj" "CatalogService.Web/"
RUN dotnet restore CatalogService.Web/Web.csproj

COPY . .
RUN dotnet build -c $BUILD_CONFIGURATION -o /app/build

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/build .
ENTRYPOINT ["dotnet", "Web.dll"]