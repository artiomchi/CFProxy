FROM mcr.microsoft.com/dotnet/aspnet:5.0-alpine AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine AS build
WORKDIR /src
COPY ["CFProxy.API/CFProxy.API.csproj", "CFProxy.API/"]
COPY ["libs/CFProxy.API.Handlers/CFProxy.API.Handlers.csproj", "libs/CFProxy.API.Handlers/"]
RUN dotnet restore "CFProxy.API/CFProxy.API.csproj"
COPY . .
WORKDIR "/src/CFProxy.API"
RUN dotnet build "CFProxy.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CFProxy.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CFProxy.API.dll"]