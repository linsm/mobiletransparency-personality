# mobiletransparency-personality

## Reference Google Trillian protobuf files
To communicate with the Google Trillian Log Server (grpc), the dotnet core project needs to reference certain profobuf references to consume the grpc service. Dotnet requires additional references to support grpc calls: 

```shell
dotnet add package Grpc.Net.Client
dotnet add package Google.Protobuf
dotnet add package Grpc.Tools
```

Install the following dotnet extension to install protobuf references:
```shell
dotnet tool install -g dotnet-grpc
```

The references of the Google Trillian Log Server can be added by running the following command in the project's root directory:

```shell
dotnet-grpc add-url -o trillian.proto -p personality.csproj -s Client https://raw.githubusercontent.com/linsm/trillian/master/trillian.proto
dotnet-grpc add-url -o trillian_log_api.proto -p personality.csproj -s Client https://raw.githubusercontent.com/linsm/trillian/master/trillian_log_api.proto
dotnet-grpc add-url -o trillian_admin_api.proto -p personality.csproj -s Client https://raw.githubusercontent.com/linsm/trillian/master/trillian_admin_api.proto
dotnet-grpc add-url -o google/rpc/status.proto -p personality.csproj -s Client https://raw.githubusercontent.com/linsm/trillian/master/third_party/googleapis/google/rpc/status.proto
```
