FROM microsoft/dotnet-nightly:2.0-sdk

ENV DATAINGESTION_BACKEND http://aspnetcore-backend-2.0:8080

EXPOSE 8080

WORKDIR /dotnetapp

# copy project.json and restore as distinct layers
COPY 2.0/*.csproj .
COPY 2.0/NuGet.config .
RUN dotnet restore

# copy and build everything else
COPY . .
RUN dotnet publish -c Release -o out

ENTRYPOINT ["dotnet", "out/dotnetapp.dll"]
