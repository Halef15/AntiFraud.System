# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copia a solução e os arquivos de projeto respeitando o caminho que a .sln espera (src/src)
COPY AntiFraud.System.sln ./
COPY src/src/AntiFraud.System.Api/AntiFraud.System.Api.csproj src/src/AntiFraud.System.Api/
COPY src/src/AntiFraud.System.Domain/AntiFraud.System.Domain.csproj src/src/AntiFraud.System.Domain/
COPY src/src/AntiFraud.System.Application/AntiFraud.System.Application.csproj src/src/AntiFraud.System.Application/
COPY src/src/AntiFraud.System.Infrastructure/AntiFraud.System.Infrastructure.csproj src/src/AntiFraud.System.Infrastructure/
COPY src/src/AntiFraud.System.BulidingBlocks/AntiFraud.System.BulidingBlocks.csproj src/src/AntiFraud.System.BulidingBlocks/
COPY src/tests/AntiFraud.System.Unit.Test/AntiFraud.System.Unit.Test.csproj src/tests/AntiFraud.System.Unit.Test/
COPY src/tests/AntiFraud.System.Integration.Test/AntiFraud.System.Integration.Test.csproj src/tests/AntiFraud.System.Integration.Test/

# Restaura dependências
RUN dotnet restore "AntiFraud.System.sln"

# Copia o resto do código
COPY . .

# Compila e publica a API
WORKDIR /src/src/src/AntiFraud.System.Api
RUN dotnet publish -c Release -o /app/publish

# Etapa final (imagem enxuta)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "AntiFraud.System.Api.dll"]
