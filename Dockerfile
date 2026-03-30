# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY PrevodCore/PrevodCore.csproj PrevodCore/
COPY PrevodWeb/PrevodWeb.csproj   PrevodWeb/
RUN dotnet restore PrevodWeb/PrevodWeb.csproj

COPY PrevodCore/ PrevodCore/
COPY PrevodWeb/  PrevodWeb/

WORKDIR /src/PrevodWeb
RUN dotnet publish PrevodWeb.csproj -c Release -o /app/publish --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

EXPOSE 5388
ENV ASPNETCORE_URLS=http://+:5388

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "PrevodWeb.dll"]
