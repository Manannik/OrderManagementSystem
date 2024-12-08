FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8082

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["OrderProcessingService.Web.csproj", "OrderProcessingService/OrderProcessingService.Web/"]
COPY ["OrderProcessingService.Infrastructure.csproj", "OrderProcessingService/OrderProcessingService.Infrastructure/"]
COPY ["OrderProcessingService.Application.csproj", "OrderProcessingService/OrderProcessingService.Application/"]
COPY ["OrderProcessingService.Persistence.csproj", "OrderProcessingService/OrderProcessingService.Persistence/"]
COPY ["OrderProcessingService.Domain.csproj", "OrderProcessingService/OrderProcessingService.Domain/"]
RUN dotnet restore "OrderProcessingService.Web.csproj"
COPY . .
WORKDIR "/src/OrderProcessingService.Web"
RUN dotnet build "OrderProcessingService.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "OrderProcessingService.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrderProcessingService.Web.dll"]
