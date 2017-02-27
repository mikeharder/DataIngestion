FROM microsoft/dotnet:1.1-sdk-projectjson

EXPOSE 8080

WORKDIR /dotnetapp

# copy project.json and restore as distinct layers
COPY 1.1/project.json .
RUN dotnet restore

# copy and build everything else
COPY . .
RUN dotnet publish -c Release -o out

ENTRYPOINT ["dotnet", "out/dotnetapp.dll"]
