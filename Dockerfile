FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

WORKDIR /app

COPY ./*.csproj ./

RUN dotnet restore

COPY . ./

RUN dotnet publish -c Release -o /app/out

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime

WORKDIR /app

COPY --from=build /app/out .

COPY Dockerfile .

EXPOSE 80

ENTRYPOINT ["dotnet", "MyBackend_MongoDB_CSharp.dll"]
