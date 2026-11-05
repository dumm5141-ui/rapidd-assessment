{
  description = "Development environment for Rapid Technologies C# Assessment";

  inputs = {
    nixpkgs.url = "github:NixOS/nixpkgs/nixos-unstable";
  };

  outputs = { self, nixpkgs }:
    let
      supportedSystems = [ "x86_64-linux" "aarch64-linux" "x86_64-darwin" "aarch64-darwin" ];
      forEachSupportedSystem = f: nixpkgs.lib.genAttrs supportedSystems (system: f {
        pkgs = import nixpkgs {
          inherit system;
          config.allowUnfree = true;
        };
      });
    in
    {
      devShells = forEachSupportedSystem ({ pkgs }: {
        default = pkgs.mkShell {
          name = "rapidd-assessment-env";

          packages = with pkgs; [
            dotnet-sdk_8
            icu
            openssl
            zlib
            fontconfig
            dejavu_fonts
          ] ++ pkgs.lib.optionals (pkgs.stdenv.hostPlatform.isLinux && pkgs.stdenv.hostPlatform.isx86_64) [
            wine64
          ];

          shellHook = ''
            export DOTNET_ROOT="${pkgs.dotnet-sdk_8}"
            export PATH="$DOTNET_ROOT:$PATH"
            export DOTNET_CLI_TELEMETRY_OPTOUT=1
            export LD_LIBRARY_PATH="${pkgs.lib.makeLibraryPath [ pkgs.icu pkgs.openssl pkgs.zlib pkgs.fontconfig ]}:''${LD_LIBRARY_PATH:-}"

            echo "=== Rapid Technologies C# Assessment Shell Loaded ==="
            echo "Dotnet SDK: $(dotnet --version)"
            if command -v wine &> /dev/null; then
              echo "Wine: $(wine --version)"
            fi
          '';
        };
      });
    };
}
