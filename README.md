# StockAlert

API back-end para monitoramento de ações da bolsa brasileira com disparo automático de alertas por e-mail. Desenvolvido como MVP para obtenção da Certificação Intermediária de **Programador Back-End (PBE)** — Edital UniFacema Nº 006/2026.

---

## Sobre o projeto

O StockAlert permite que usuários cadastrem ações da bolsa e definam regras de alerta baseadas em preço-alvo ou variação percentual. Um serviço em segundo plano monitora continuamente as cotações em tempo real via Brapi e envia um e-mail automaticamente quando a condição configurada é atingida.

> Observação: o sistema normaliza automaticamente os símbolos das ações brasileiras, convertendo entradas como `PETR4` para `PETR4.SA` ao consultar a Brapi.

---

## Funcionalidades

* Autenticação via Google OAuth com geração de token JWT
* Cadastro de ações monitoradas por usuário
* Criação, edição e exclusão de regras de alerta
* Monitoramento automático em segundo plano (BackgroundService)
* Disparo de alertas por e-mail com mensagem detalhada
* Controle de cooldown para evitar notificações repetidas
* Histórico de notificações persistido no banco de dados
* Tratamento de erros centralizado

---

## Arquitetura

O projeto segue os princípios da **Clean Architecture**, com responsabilidades bem separadas em camadas independentes:

```
StockAlert/
├── src/
│   ├── StockAlert.API
│   ├── StockAlert.Application
│   ├── StockAlert.Domain
│   ├── StockAlert.Infrastructure
│   ├── StockAlert.Communication
│   └── StockAlert.Exception
└── test/
    └── StockAlert.Tests
```

---

## 📌 Camadas do sistema

### 🔹 API

* Controllers (endpoints HTTP)
* Workers (BackgroundService)
* Filtros de exceção
* Configuração da aplicação (DI, JWT, Swagger)

👉 Porta de entrada do sistema

---

### 🔹 Application

* Casos de uso (UseCases)
* Validações (FluentValidation)
* Orquestração da lógica de negócio

👉 Onde a regra do sistema é executada

---

### 🔹 Domain

* Entidades
* Interfaces
* Enums

👉 Núcleo do sistema (independente)

---

### 🔹 Infrastructure

* Banco de dados (EF Core)
* Repositórios
* Integrações externas (Brapi)
* Envio de e-mail (SMTP)
* Segurança (JWT)

👉 Implementação de tudo que é externo

---

### 🔹 Communication

* DTOs de Request
* DTOs de Response

👉 Transporte de dados

---

### 🔹 Exception

* Exceções customizadas
* Padronização de erros

---

## 🔄 Fluxo de dependência

```text
API → Application → Domain ← Infrastructure
```

### Regras de dependência entre camadas

* **API**

  * Pode acessar: Application, Infrastructure, Communication, Exception

* **Application**

  * Pode acessar: Domain, Communication, Exception

* **Domain**

  * Não acessa nenhuma camada

* **Infrastructure**

  * Pode acessar: Domain

* **Communication**

  * Não acessa nenhuma camada

* **Exception**

  * Não acessa nenhuma camada

---

👉 O domínio nunca depende de camadas externas, garantindo baixo acoplamento e alta manutenibilidade.

---

## Tecnologias utilizadas

* .NET 10 / ASP.NET Core
* Entity Framework Core 10
* PostgreSQL
* JWT Bearer
* Google OAuth
* FluentValidation
* Polly (retry automático)
* Brapi API
* SMTP (Gmail)
* Docker
* xUnit + Moq + FluentAssertions
* Swagger

---

## Configuração

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=YOUR_HOST;Port=5432;Database=StockAlert;Username=YOUR_USERNAME;Password=YOUR_PASSWORD"
  },
  "Settings": {
    "Jwt": {
      "SigningKey": "YOUR_SECURE_RANDOM_LONG_KEY_HERE",
      "ExpirationInMinutes": 1440
    }
  },
  "Google": {
    "ClientId": "YOUR_GOOGLE_CLIENT_ID.apps.googleusercontent.com"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "Brapi": {
    "BaseUrl": "https://brapi.dev/api/",
    "Token": "YOUR_BRAPI_API_TOKEN"
  },
  "EmailSettings": {
    "Host": "smtp.gmail.com",
    "Port": 587,
    "UserName": "YOUR_EMAIL_ADDRESS@gmail.com",
    "Password": "YOUR_APP_SPECIFIC_PASSWORD"
  },
  "WorkerSettings": {
    "IntervalSeconds": 10
  }
}
```

---

## Execução

```bash
dotnet run --project src/StockAlert.API
```

Swagger disponível em:

```
https://localhost:7100/swagger
```

---

## Como funciona o monitoramento

O sistema utiliza um `BackgroundService` que:

1. Busca regras ativas
2. Consulta preço na Brapi
3. Verifica condição
4. Aplica cooldown
5. Envia e-mail
6. Salva histórico
7. Atualiza regra

O intervalo de execução é configurável via `WorkerSettings.IntervalSeconds`.

---

## Testes

```bash
dotnet test
```

---

## Melhorias futuras

* Relatórios em PDF
* Integração com WhatsApp
* Dashboard com gráficos
* Notificações em tempo real

---

## Autor

Davi Queiroz Lima
ADS — UniFacema
