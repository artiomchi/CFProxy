FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine AS build
WORKDIR /src
COPY ["src/CFProxy.API/CFProxy.API.csproj", "src/CFProxy.API/"]
COPY ["src/libs/CFProxy.API.Handlers/CFProxy.API.Handlers.csproj", "src/libs/CFProxy.API.Handlers/"]
RUN dotnet restore "src/CFProxy.API/CFProxy.API.csproj"
COPY . .
WORKDIR "/src/src/CFProxy.API"
RUN dotnet build "CFProxy.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CFProxy.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CFProxy.API.dll"]