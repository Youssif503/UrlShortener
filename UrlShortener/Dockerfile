FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /app

COPY UrlShortener.csproj .

RUN dotnet restore

COPY . .

RUN dotnet publish -c Release -o /app/output 

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime

WORKDIR /app

COPY --from=build /app/output .

EXPOSE 8080

ENTRYPOINT ["dotnet", "UrlShortener.dll"]