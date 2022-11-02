# mobiletransparency-personality

## Docker
The personality can be hosted as docker container. To build the container run the following command in the project directory:
```shell
docker build -t personality personality/
```
Execute the following command to run the docker continer. Please consider to adapt the network and the trillian_url environment variable depending 
```shell
docker run -p 8080:80 --network deployment_default -e "trillian_url=http://172.20.0.4:8090" --name personality personality
```



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
dotnet-grpc add-url -o trillian.proto -p personality.csproj -s Client https://raw.githubusercontent.com/google/trillian/master/trillian.proto
dotnet-grpc add-url -o trillian_log_api.proto -p personality.csproj -s Client https://raw.githubusercontent.com/google/trillian/master/trillian_log_api.proto
dotnet-grpc add-url -o trillian_admin_api.proto -p personality.csproj -s Client https://raw.githubusercontent.com/google/trillian/master/trillian_admin_api.proto
dotnet-grpc add-url -o google/rpc/status.proto -p personality.csproj -s Client https://raw.githubusercontent.com/googleapis/googleapis/c81bb70/google/rpc/status.proto
```

Last tested commit of the Google Trillian github repository was 940d76c. 

## Create docker images with nix

```shell
nix build
docker load <./result
docker images #search for result
docker run -p8080:80 -e "trillian_url=URL_TO_GRPC_ENDPOINT" HASH_OF_DOCKER_IMAGE
```

## Notes SSL support

openssl req -x509 -nodes -days 365 -newkey rsa:2048 -keyout personality.key -out personality.crt -config devcert.conf 
openssl pkcs12 -export -out personality.pfx -inkey personality.key -in personality.crt
docker run --rm -it -p 8000:80 -p 8001:443 -e Kestrel\_\_Certificates\_\_Default\_\_Path=/app/Infrastructure/Certificate/personality.pfx -e Kestrel\_\_Certificates\_\_Default\_\_Password=INSERTPASSWORD -e "ASPNETCORE_URLS=https://+;http://+" -v /PATHTOFOLDERWHERECERTISSTORED/:/app/Infrastructure/Certificate personality

