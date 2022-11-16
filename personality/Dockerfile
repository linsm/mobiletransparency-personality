FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

COPY . ./
RUN dotnet restore
RUN dotnet add package Grpc.Net.Client
RUN dotnet add package Google.Protobuf
RUN dotnet add package Grpc.Tools
RUN dotnet tool install -g dotnet-grpc
RUN /root/.dotnet/tools/dotnet-grpc add-url -o trillian.proto -p personality.csproj -s Client https://raw.githubusercontent.com/google/trillian/master/trillian.proto
RUN /root/.dotnet/tools/dotnet-grpc add-url -o trillian_log_api.proto -p personality.csproj -s Client https://raw.githubusercontent.com/google/trillian/master/trillian_log_api.proto
RUN /root/.dotnet/tools/dotnet-grpc add-url -o trillian_admin_api.proto -p personality.csproj -s Client https://raw.githubusercontent.com/google/trillian/master/trillian_admin_api.proto
RUN /root/.dotnet/tools/dotnet-grpc add-url -o google/rpc/status.proto -p personality.csproj -s Client https://raw.githubusercontent.com/googleapis/googleapis/c81bb70/google/rpc/status.proto

# Build and publish a release
RUN dotnet publish --no-restore -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build-env /app/out .
EXPOSE 80
ENTRYPOINT ["dotnet", "personality.dll"]

