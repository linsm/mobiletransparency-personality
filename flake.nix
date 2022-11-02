{
  description = "mobiletransparency-personality";

  inputs = {
    flock-of-flakes = {
      type = "git";
      url = "https://git.ins.jku.at/proj/digidow/flock-of-flakes.git";
    };
    trillian-google = {
      url = github:google/trillian;
      flake = false;
    };
    googleapis = {
      url = github:googleapis/googleapis/c81bb70;
      flake = false;
    };
    nix-filter-lib.url = github:numtide/nix-filter;
    nixpkgs.follows = "flock-of-flakes/nixpkgs";
  };

  outputs = { self, nixpkgs, flock-of-flakes, trillian-google, googleapis, nix-filter-lib }:
  let
    system = "x86_64-linux";
    pkgs = import nixpkgs { inherit system; };
    dotnet-sdk = pkgs.dotnetCorePackages.sdk_6_0;
    dotnet-runtime = pkgs.dotnetCorePackages.aspnetcore_6_0;
    libstdcxx = pkgs.stdenv.cc.cc.lib;
    nix-filter = nix-filter-lib.lib;
    # protobuf-defs copies the trillian proto files and the status.proto in one directory
    protobuf-defs = pkgs.symlinkJoin {
      name = "protobuf-defs";
      paths = [  
     ( nix-filter {
    root = googleapis;
    include = [
      "google/rpc/status.proto"
      ];
    }) (
      nix-filter {
        root = trillian-google;
        include = [
          "trillian.proto"
          "trillian_log_api.proto"
          "trillian_admin_api.proto"        
        ];
      }
    )
    ];
    };
  in {

    devShell = pkgs.mkShell {
      buildInputs = [ dotnet-sdk ];
      # TODO: patch ELF binaries from nuget at runtime
    };

    packages.${system} = rec {
      personality = pkgs.buildDotnetModule {
        dontStrip = true;
        dontAutoPatchelf = true;
        pname = "fdroid-personality";
        version = "0.0.1";
        src = ./personality;
        projectFile = "personality.csproj";
        TRILLIAN_PATH = trillian-google;
        GOOGLEAPIS_PATH = googleapis;
        PROTOBUF_DEFS = protobuf-defs;

        # File generated with `nix build .#packages.x86_64-linux.personality.passthru.fetch-deps`.
        nugetDeps = ./deps.nix;

        nativeBuildInputs = with pkgs; [
          # for patching interpreter paths of nuget packages
          # by callling autoPatchelf
          autoPatchelfHook
          libstdcxx
        ];

        inherit dotnet-sdk dotnet-runtime;
        packNupkg = true; # This packs the project as "foo-0.1.nupkg" at `$out/share`.
        preBuild = ''
          autoPatchelf /build
        '';
      };

      personality-image = pkgs.dockerTools.buildImage {
        name = "mobiletransparency-personality-image";
        # https://github.com/opencontainers/image-spec/blob/main/config.md#properties
        config = {
          Cmd = [ "${personality}/bin/personality" ];
          ExposedPorts = {"80/tcp" = {};};
          # Required because root filesystem of docker container is read only
          Env = ["COMPlus_EnableDiagnostics=0"];
        };
      };
      default = personality-image;
    };
  };
}


