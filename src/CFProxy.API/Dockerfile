FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["src/CFProxy.API/CFProxy.API.csproj", "src/CFProxy.API/"]
COPY ["src/libs/CFProxy.API.Handlers/CFProxy.API.Handlers.csproj", "src/libs/CFProxy.API.Handlers/"]
RUN dotnet restore "src/CFProxy.API/CFProxy.API.csproj"
COPY . .
WORKDIR "/src/src/CFProxy.API"
RUN dotnet build "CFProxy.API.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "CFProxy.API.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "CFProxy.API.dll"]