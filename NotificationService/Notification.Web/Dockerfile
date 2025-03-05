FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8083

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Notification.Web/Notification.Web.csproj", "Notification.Web/"]
COPY ["Notification.Persistence/Notification.Persistence.csproj", "Notification.Persistence/"]
COPY ["Notification.Domain/Notification.Domain.csproj", "Notification.Domain/"]
COPY ["Messaging.Kafka/Messaging.Kafka.csproj", "Messaging.Kafka/"]
RUN dotnet restore "Notification.Web/Notification.Web.csproj"
COPY . .
WORKDIR "/src/Notification.Web"
RUN dotnet build "Notification.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Notification.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Notification.Web.dll"]
