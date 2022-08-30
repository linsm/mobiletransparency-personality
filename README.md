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
## Notes SSL support

openssl req -x509 -nodes -days 365 -newkey rsa:2048 -keyout personality.key -out personality.crt -config devcert.conf 
openssl pkcs12 -export -out personality.pfx -inkey personality.key -in personality.crt
docker run --rm -it -p 8000:80 -p 8001:443 -e Kestrel\_\_Certificates\_\_Default\_\_Path=/app/Infrastructure/Certificate/personality.pfx -e Kestrel\_\_Certificates\_\_Default\_\_Password=INSERTPASSWORD -e "ASPNETCORE_URLS=https://+;http://+" -v /PATHTOFOLDERWHERECERTISSTORED/:/app/Infrastructure/Certificate personality

