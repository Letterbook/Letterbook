
# This Dockerfile is intended for development. It will not produce production ready artifacts.

FROM mcr.microsoft.com/dotnet/sdk:8.0 as builder

WORKDIR /app
COPY . .

RUN dotnet publish Letterbook/Letterbook.csproj -c Debug -o publish


FROM mcr.microsoft.com/dotnet/sdk:8.0 as dev

WORKDIR /app
COPY --from=builder /app/publish .

ENTRYPOINT ["dotnet", "exec", "Letterbook.dll"]