FROM microsoft/dotnet:2.0-runtime-nanoserver-1709 AS base
WORKDIR /app

FROM microsoft/dotnet:2.0-sdk-nanoserver-1709 AS build
WORKDIR /src
COPY *.sln ./
COPY GatewayConsole/GatewayConsole.csproj GatewayConsole/
COPY PrintMdule/PrintMdule.csproj PrintMdule/
COPY Daenet.IotGateway.BluetoothLEModule/Daenet.IotGateway.BluetoothLEModule.csproj Daenet.IotGateway.BluetoothLEModule/
RUN dotnet restore
COPY . .
WORKDIR /src/GatewayConsole
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "GatewayConsole.dll"]
