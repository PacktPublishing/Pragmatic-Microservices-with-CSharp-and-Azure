#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Codebreaker.GameAPIs/Codebreaker.GameAPIs.csproj", "Codebreaker.GameAPIs/"]
COPY ["Codebreaker.ServiceDefaults/Codebreaker.ServiceDefaults.csproj", "Codebreaker.ServiceDefaults/"]
#Following copy not needed anymore when we don't use the .NET Aspire daily build
COPY ["nuget.config", "nuget.config"]
RUN dotnet restore "./Codebreaker.GameAPIs/Codebreaker.GameAPIs.csproj"
COPY . .
WORKDIR "/src/Codebreaker.GameAPIs"
RUN dotnet build "./Codebreaker.GameAPIs.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Codebreaker.GameAPIs.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Codebreaker.GameAPIs.dll"]