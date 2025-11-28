FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 9090

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS publish
WORKDIR /src

COPY . .
RUN dotnet publish "./SPARTA.API/SPARTA.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app

COPY --from=publish /app/publish .

# Configurar variables de entorno
ENV ASPNETCORE_URLS=http://+:9090
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "SPARTA.API.dll"]