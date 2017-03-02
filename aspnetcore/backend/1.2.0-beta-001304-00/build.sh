cp 1.2.0-beta-001304-00/NuGet.config .
dotnet restore /p:TargetFramework=netcoreapp2.0 /p:RuntimeFrameworkVersion=1.2.0-beta-001304-00
dotnet build -c Release /p:TargetFramework=netcoreapp2.0 /p:RuntimeFrameworkVersion=1.2.0-beta-001304-00
rm NuGet.config
