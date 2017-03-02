cp 2.0.0-beta/NuGet.config .
dotnet restore /p:TargetFramework=netcoreapp2.0 /p:RuntimeFrameworkVersion=2.0.0-beta-*
dotnet build -c Release /p:TargetFramework=netcoreapp2.0 /p:RuntimeFrameworkVersion=2.0.0-beta-*
rm NuGet.config
