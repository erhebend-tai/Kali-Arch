# Build from repo root:
#   docker build -f services/VirtualMachineService/Dockerfile -t virtualmachineservice .
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish "services/VirtualMachineService/src/VirtualMachineService.API/VirtualMachineService.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "VirtualMachineService.API.dll"]
