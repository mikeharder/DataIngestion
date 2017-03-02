FROM microsoft/dotnet:1.1.0-sdk-msbuild-rc4

ENV ASPNETCORE_URLS http://+:8080

EXPOSE 8080

WORKDIR /dotnetapp

# copy csproj and restore as distinct layers
COPY *.csproj .
RUN dotnet restore /p:TargetFramework=netcoreapp1.1 /p:RuntimeFrameworkVersion=1.1.0

# copy and build everything else
COPY . .
RUN dotnet publish -c Release -o out /p:TargetFramework=netcoreapp1.1 /p:RuntimeFrameworkVersion=1.1.0

ENTRYPOINT ["dotnet", "out/backend.dll"]
