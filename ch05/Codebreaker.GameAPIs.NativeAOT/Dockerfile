#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0-preview AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0-preview AS build
RUN apt update
RUN apt install -y clang zlib1g-dev
WORKDIR /src
COPY ["Codebreaker.GameAPIs.NativeAOT/Codebreaker.GameAPIs.NativeAOT.csproj", "Codebreaker.GameAPIs.NativeAOT/"]
COPY ["Codebreaker.Data.Cosmos/Codebreaker.Data.Cosmos.csproj", "Codebreaker.Data.Cosmos/"]
COPY ["Codebreaker.GameAPIs.Models/Codebreaker.GameAPIs.Models.csproj", "Codebreaker.GameAPIs.Models/"]
COPY ["Codebreaker.Data.SqlServer/Codebreaker.Data.SqlServer.csproj", "Codebreaker.Data.SqlServer/"]
RUN dotnet restore "Codebreaker.GameAPIs.NativeAOT/Codebreaker.GameAPIs.NativeAOT.csproj"
COPY . .
WORKDIR "/src/Codebreaker.GameAPIs.NativeAOT"
RUN dotnet build "Codebreaker.GameAPIs.NativeAOT.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Codebreaker.GameAPIs.NativeAOT.csproj" -r linux-arm64 -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["Codebreaker.GameAPIs.exe"]