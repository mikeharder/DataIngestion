FROM microsoft/dotnet-nightly:2.0.0-beta-sdk

ENV DATAINGESTION_BACKEND http://aspnetcore-backend-2.0.0-beta:8080

EXPOSE 8080

WORKDIR /dotnetapp

# copy csproj and restore as distinct layers
COPY *.csproj .
COPY 2.0.0-beta/NuGet.config .
RUN dotnet restore /p:TargetFramework=netcoreapp2.0 /p:RuntimeFrameworkVersion=2.0.0-beta-*

# copy and build everything else
COPY . .
RUN dotnet publish -c Release -o out /p:TargetFramework=netcoreapp2.0 /p:RuntimeFrameworkVersion=2.0.0-beta-*

ENTRYPOINT ["dotnet", "out/frontend.dll"]
