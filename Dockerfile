# Etapa 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copia a solução e os arquivos de projeto com os caminhos corretos
COPY ["AntiFraud.System.sln", "AntiFraud.System.sln"]
COPY ["src/AntiFraud.System.Api/AntiFraud.System.Api.csproj", "src/AntiFraud.System.Api/"]
COPY ["src/AntiFraud.System.Application/AntiFraud.System.Application.csproj", "src/AntiFraud.System.Application/"]
COPY ["src/AntiFraud.System.Domain/AntiFraud.System.Domain.csproj", "src/AntiFraud.System.Domain/"]
COPY ["src/AntiFraud.System.Infrastructure/AntiFraud.System.Infrastructure.csproj", "src/AntiFraud.System.Infrastructure/"]
COPY ["src/AntiFraud.System.BulidingBlocks/AntiFraud.System.BulidingBlocks.csproj", "src/AntiFraud.System.BulidingBlocks/"]

# === ADICIONE ESTAS DUAS LINHAS ===
COPY ["tests/AntiFraud.System.Integration.Test/AntiFraud.System.Integration.Test.csproj", "tests/AntiFraud.System.Integration.Test/"]
COPY ["tests/AntiFraud.System.Unit.Test/AntiFraud.System.Unit.Test.csproj", "tests/AntiFraud.System.Unit.Test/"]
# ==================================

# Restaura as dependências
RUN dotnet restore "AntiFraud.System.sln"

# Copia o resto do código-fonte
COPY . .

# Publica a aplicação
RUN dotnet publish "src/AntiFraud.System.Api/AntiFraud.System.Api.csproj" -c Release -o /app/publish

# Etapa 2: Imagem Final
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "AntiFraud.System.Api.dll"]