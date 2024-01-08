#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER app
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Chess.Engine.LichessBot/Chess.Engine.LichessBot.csproj", "Chess.Engine.LichessBot/"]
COPY ["ChessLibrary/ChessLibrary.csproj", "ChessLibrary/"]
RUN dotnet restore "./Chess.Engine.LichessBot/./Chess.Engine.LichessBot.csproj"
COPY . .
WORKDIR "/src/Chess.Engine.LichessBot"
RUN dotnet build "./Chess.Engine.LichessBot.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Chess.Engine.LichessBot.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
ENV LICHESSAPIKEY=notset
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Chess.Engine.LichessBot.dll"]