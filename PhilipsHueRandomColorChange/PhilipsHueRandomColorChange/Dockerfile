# 1. Run dotnet publish
# 2. Copy Dockerfile to ..\bin\Debug\netcoreapp2.0\publish
# 3. Run docker build command 
# 4. Run docker run -e GTWURI=<gateway_uri> -e USERNAME=<user_name> -e DEVICEID=<device_id> <image>
FROM microsoft/dotnet:2.0-runtime-nanoserver-1709 AS base
WORKDIR /app
COPY . .
ENTRYPOINT ["dotnet", "PhilipsHueRandomColorChange.dll"]