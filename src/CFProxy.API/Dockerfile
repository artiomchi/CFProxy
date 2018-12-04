FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["src/CFProxy.API/CFProxy.API.csproj", "src/CFProxy.API/"]
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