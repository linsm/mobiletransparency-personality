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

## Publication

This repository is part of the following publication. 

[M. Lins, R. Mayrhofer, M. Roland, and A. R. Beresford: “Mobile App Distribution Transparency (MADT): Design and evaluation of a system to mitigate necessary trust in mobile app distribution systems”, in Secure IT Systems. 28th Nordic Conference, NordSec 2023, Oslo, Norway, LNCS, vol. 14324/2024, Springer, pp. 185–​203, 2023. https://doi.org/10.1007/978-3-031-47748-5_11](https://doi.org/10.1007/978-3-031-47748-5_11)

## Acknowledgment

This work has been carried out within the scope of Digidow, the Christian Doppler Laboratory for Private Digital Authentication in the Physical World and has partially been supported by the LIT Secure and Correct Systems Lab. 
We gratefully acknowledge financial support by the Austrian Federal Ministry of Labour and Economy, the National Foundation for Research, Technology and Development, the Christian Doppler Research Association, 3 Banken IT GmbH, ekey biometric systems GmbH, Kepler Universitätsklinikum GmbH, NXP Semiconductors Austria GmbH & Co KG, Österreichische Staatsdruckerei GmbH, and the State of Upper Austria.

## LICENSE

Licensed under the EUPL, Version 1.2 or – as soon they will be approved by
the European Commission - subsequent versions of the EUPL (the "Licence").
You may not use this work except in compliance with the Licence.

**License**: [European Union Public License v1.2](https://joinup.ec.europa.eu/software/page/eupl)
