
# This Dockerfile is intended for development. It will not produce production ready artifacts.

FROM mcr.microsoft.com/dotnet/sdk:10.0 as dev_builder

WORKDIR /app
COPY . .

RUN dotnet publish Letterbook/Letterbook.csproj -c Debug -o publish


FROM mcr.microsoft.com/dotnet/sdk:10.0 as dev

WORKDIR /app
COPY --from=dev_builder /app/publish .

ENTRYPOINT ["dotnet", "exec", "Letterbook.dll"]