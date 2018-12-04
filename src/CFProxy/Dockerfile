FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["../src/CFProxy/CFProxy.csproj", "../src/CFProxy/"]
RUN dotnet restore "../src/CFProxy/CFProxy.csproj"
COPY . .
WORKDIR "/src/../src/CFProxy"
RUN dotnet build "CFProxy.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "CFProxy.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "CFProxy.dll"]