# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
WORKDIR /app
EXPOSE 5000


FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src
COPY ["Lilith.Server.Api/Lilith.Server.Api.csproj", "Lilith.Server.Api/"]

RUN dotnet restore "Lilith.Server.Api/Lilith.Server.Api.csproj"
COPY . .
WORKDIR "/src/Lilith.Server.Api"
RUN dotnet build "Lilith.Server.Api.csproj" -c Release -o /app/build

FROM build as publish
RUN dotnet publish "Lilith.Server.Api.csproj" -c Release -o /app/publish

FROM base AS final
# Set timezone
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false \
    TZ=Europe/Madrid

WORKDIR /app
RUN mkdir -p /files

RUN apk add --no-cache tzdata icu-libs

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Lilith.Server.Api.dll"]
