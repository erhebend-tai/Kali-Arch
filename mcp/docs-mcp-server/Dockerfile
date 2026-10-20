# Build from repo root:
#   docker build -f mcp/docs-mcp-server/Dockerfile -t docs-mcp-server .
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish "mcp/docs-mcp-server/src/DocsMcpServer/DocsMcpServer.csproj" -c Release -o /app/publish
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "DocsMcpServer.dll"]
