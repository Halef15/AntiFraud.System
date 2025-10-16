# 💻 Módulo Antifraude


## 🛠️ Instruções

* O desafio é válido para diversos níveis, portanto não se preocupe se não conseguir resolver por completo.
* A aplicação só será avaliada se estiver rodando; se necessário, crie um passo a passo para execução.
* Faça um clone do repositório em seu Git pessoal para iniciar o desenvolvimento e não cite nada relacionado à Empresa.
* Após concluir, envie o link do repositório por e-mail ao responsável de RH.

---

## 🧾 Conteúdo
- [Requisitos](#requisitos-nao-funcionais)
- [Endpoints](#endpoints)
- [Estrutura do Projeto](#estrutura-do-projeto)
- [Executando no Docker](#executando-no-docker)

---

## ⚙️ Requisitos Não Funcionais

* **Plataforma:** .NET (C#)
* **Banco de dados:** Postgree
* **Mensageria:** RabbitMQ 

---

### 📄 Aplicação a ser desenvolvida

Você deverá desenvolver um **módulo de avaliação antifraude** para processar transações financeiras. Esse módulo deve receber transações, aplicar um conjunto de regras predefinidas e devolver uma decisão (`APPROVED`, `REJECTED`, `REVIEW`). O sistema precisa ser **idempotente**, **resiliente** e **auditável**.

O processamento da análise deve ser **assíncrono**. A API receberá a transação, a colocará em uma fila para processamento e retornará imediatamente uma resposta de aceite (`202 Accepted`), permitindo que o resultado seja consultado posteriormente.

⚠️ **Atenção:** a avaliação será feita de forma **automatizada**. Portanto, é essencial que:

- As **rotas e contratos** (endpoints, parâmetros e respostas) estejam exatamente conforme descritos neste documento.
- Os **payloads JSON de entrada e saída** sigam rigorosamente os formatos apresentados.
- Os erros retornados estejam padronizados no seguinte formato:

### 🔁 Padrão de erro
  - `400 Bad Request`
    ```json
    {
      "errors": [ "Dados inválidos." ]
    }
    ```
  - `500 Internal Server Error` 
    ```json
    {
      "errors": [ "Ocorreu um erro inesperado durante o processamento da sua solicitação." ]
    }
    ```
---

### 🧾 Regras de Análise de Risco

A análise de uma transação deve aplicar as seguintes regras. Uma transação que se enquadre em qualquer regra de `REJECTED` ou `REVIEW` deve receber o status correspondente. Se múltiplas regras de `REVIEW` forem ativadas, o status final ainda será `REVIEW`.

| Condição                                                     | Status da Decisão |
| ------------------------------------------------------------ | ----------------- |
| Valor da transação superior a R$ 5.000,00                    | `REVIEW`          |
| Mais de 3 transações do mesmo `ipAddress` em menos de 1 hora | `REVIEW`          |
| Transação originada de um país de alto risco (lista pré-definida: `AF`, `IR`, `KP`) | `REVIEW`          |
| `cardNumber` consta em uma blocklist interna                | `REJECTED`        |
| Nenhuma das condições acima                                  | `APPROVED`        |

---

## 🌐 Endpoints

### 💳 1. Transações.
**Descrição:** Permitir o envio de transações para análise assíncrona, a consulta dos resultados e a revisão manual de transações que necessitam de atenção.

- **POST** `/api/transactions`  
  - **Dados obrigatórios:** - Header: `Idempotency-Key: <UUID>`
    ```json
    {
    "amount": 10,
    "cardHolder": "Halef Basso",
    "cardNumber": "5416 8531 1699 5712",
    "ipAddress": "IP54654H",
    "location": "Curitiba",
    "transactionDate": "2025-10-14T21:07:38.624Z"
    }
    ```
  - **Ações:** 1. Validar o header de idempotência para evitar processamento duplicado.  
    2. Publicar o evento `TransactionReceived` em um sistema de mensageria.  
    3. Um `Worker` consumirá o evento, aplicará as regras e persistirá o resultado.
  - **Respostas:**
    - `202 Accepted`
        - Header `Location: /api/transactions/{transactionId}/analysis`

- **GET** `/api/transactions/{transactionId}/analysis`  
  - **Respostas:**
    - `200 OK`
      ```json
      {
        "transactionId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "status": "string",
        "createdAt": "2025-10-16T01:07:00.051Z",
        "updatedAt": "2025-10-16T01:07:00.051Z"
      }
      ```

- **PUT** `/api/transactions/{transactionId}/review`
  - **Dados obrigatórios:** 
  ```json
    {
      "transactionId": "f7b4c7e3-0b5a-4b6e-8e3a-2c6f6e2b4a5d"
      "Status": "APPROVED",
    }
    ```

---

## 🚀 Diferenciais.  
- Testes unitários;
- Testes integração;
- EF Core;
- Docker + Docker Compose;
- Design Patterns (Repository, CQRS);
- Swagger / OpenAPI;
- Clean Architecture;
- Código em inglês, limpo e organizado;
- Logs estruturados (Serilog);
- Observabilidade (metrics, traces);

---

## 🎯 Objetivo do teste:

O objetivo deste desafio é avaliar de forma completa a capacidade técnica do candidato no desenvolvimento de soluções backend modernas, utilizando boas práticas de arquitetura, mensageria e persistência de dados.

Durante a correção, serão analisados os seguintes critérios:

- 📦 Modelagem de entidades de domínio (Transação, Análise, Regras)
- 📊 Implementação correta das regras de negócio (análise de risco)
- ✉️ Uso adequado de mensageria para processamento assíncrono (evento `TransactionReceived`)
- 🧪 Cobertura de testes (unitários e integração)
- 🧱 Organização do código e separação de responsabilidades (Domain, Application, Infrastructure)
- 🧰 Adoção de boas práticas como uso de CQRS, Repositórios e Design Patterns
- 🧼 Clareza e padronização de código (nomenclaturas, validações, mensagens de erro)
- ⚙️ Configuração e documentação (Swagger/OpenAPI, Docker Compose, README funcional)

> Avaliações automatizadas validarão se os contratos REST estão sendo respeitados, incluindo rotas, parâmetros, estrutura de payloads e mensagens de erro.

---

## Estrutura do Projeto

### Nível Raiz da Solução
- **docker/**: Contém arquivos `docker-compose.yaml` que configuram serviços da aplicação no ambiente Docker.
- **docker/.env**: Armazena variáveis de ambiente para configurar serviços Docker sem expor informações sensíveis.
- **Directory.Build.props**: Define configurações globais de build, como versões de pacotes, para todos os projetos.
- **NuGet.config**: Gerencia pacotes e fontes de dependências do projeto.

### Estrutura de Diretórios

**src/**:
- **AntiFraud.System.Api**: API RESTful que expõe os endpoints da aplicação.
- **AntiFraud.System.Application**: Contém a lógica de negócios e casos de uso.
- **AntiFraud.System.Domain**: Modelo de domínio, entidades e regras de negócio.
- **AntiFraud.System.Infrastructure**: Gestão de infraestrutura, banco de dados e serviços externos.
- **AntiFraud.System.Worker**: Serviços de background e processamento assíncrono.

**tests/**:
- **Fortress.AntiFraud.Integration.Test**: Testes de integração entre componentes.
- **Fortress.AntiFraud.Unit.Test**: Testes unitários para validar a lógica de classes e métodos.

---

## Executando no Docker

### Variáveis de Ambiente (.env)
O arquivo `.env` contém as variáveis de ambiente para configurar os serviços.
Aqui estão os principais itens:

- **POSTGRES Server Configurações**:
  - `POSTGRES_DB=AntiFraudDb_Dev`
  - `POSTGRES_PASSWORD=AntiFraud@123`
  - `POSTGRES_USER=postgres`
  - `POSTGRES_HOST=postgresql`
  - `POSTGRES_PORT=5432`

- **RabbitMQ Configurações**:
  - `RABBITMQ_USERNAME=admin`
  - `RABBITMQ_PASSWORD=AntiFraud@123`
  - `RABBITMQ_DEFAULT_USER=admin`
  - `RABBITMQ_DEFAULT_PASS=AntiFraud@123`
  - `RABBITMQ_HOST=rabbitmq`

### Comandos para Executar
Para executar a aplicação, utilize os seguintes comandos no diretório raiz:

```bash
# Construir e executar os serviços no Docker
docker compose -f docker/docker-compose.yaml up -d

### Acessando os Serviços

- **API**: [localhost:5000/swagger](http://localhost:5000/swagger/index.html)

- **Aspire Dashboard**: [localhost:18888](http://localhost:18888)

- **Prometheus**: [localhost:9090](http://localhost:9090)

- **RabbitMQ**: [localhost:15672](http://localhost:15672/#/)  
  **Usuário**: `admin`  
  **Senha**: `AntiFraud@123`

- **PostgreSQL**:  
  **Usuário**: `postgres`  
  **Senha**: `AntiFraud@123`  
  **Banco de Dados**: `AntiFraudDb_Dev`
