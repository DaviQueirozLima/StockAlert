# StockAlert

API back-end para monitoramento de ações da bolsa brasileira com disparo automático de alertas por e-mail. Desenvolvido como MVP para obtenção da Certificação Intermediária de **Programador Back-End (PBE)** — Edital UniFacema Nº 006/2026.

---

## Sobre o projeto

O StockAlert permite que usuários cadastrem ações da bolsa e definam regras de alerta baseadas em preço-alvo ou variação percentual. Um serviço em segundo plano monitora continuamente as cotações em tempo real via [Brapi](https://brapi.dev) e envia um e-mail automaticamente quando a condição configurada é atingida.

### Funcionalidades

- Autenticação via Google OAuth com geração de token JWT
- Cadastro de ações monitoradas por usuário
- Criação, edição e exclusão de regras de alerta
- Monitoramento automático de cotações em segundo plano (BackgroundService)
- Disparo de alertas por e-mail com controle de cooldown e opção de disparo único
- Histórico de notificações persistido no banco de dados
- Tratamento de erros centralizado com respostas padronizadas

---

## Arquitetura

O projeto segue os princípios da **Clean Architecture**, com responsabilidades bem separadas em camadas independentes:

```
StockAlert/
├── src/
│   ├── StockAlert.API            # Controllers, Workers, Filters, configuração da aplicação
│   ├── StockAlert.Application    # Use Cases, Validators (regras de negócio)
│   ├── StockAlert.Domain         # Entidades, Interfaces, Enums (núcleo da aplicação)
│   ├── StockAlert.Infrastructure # EF Core, Repositórios, JWT, Brapi, Email
│   ├── StockAlert.Communication  # DTOs de Request e Response
│   └── StockAlert.Exception      # Exceções de domínio customizadas
└── test/
    └── StockAlert.Tests          # Testes unitários (xUnit + Moq + FluentAssertions)
```

A dependência sempre flui de fora para dentro: `API → Application → Domain ← Infrastructure`. O domínio não conhece nenhuma camada externa.

---

## Tecnologias utilizadas

| Tecnologia | Uso |
|---|---|
| .NET 10 / ASP.NET Core | Framework principal |
| Entity Framework Core 10 | ORM e migrações de banco de dados |
| PostgreSQL | Banco de dados relacional |
| JWT Bearer | Autenticação e autorização |
| Google.Apis.Auth | Validação de tokens Google OAuth |
| FluentValidation | Validação de requests |
| Polly | Resiliência com retry exponencial nas chamadas HTTP |
| Brapi API | Cotações em tempo real da bolsa brasileira |
| SMTP (Gmail) | Envio de alertas por e-mail |
| Docker | Containerização da aplicação |
| xUnit + Moq + FluentAssertions | Testes unitários |
| Swagger / OpenAPI | Documentação interativa da API |

---

## Pré-requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL](https://www.postgresql.org/) (local ou via Docker)
- [Docker](https://www.docker.com/) (opcional)
- Conta no Google Cloud com um **Client ID** OAuth configurado
- Token da [Brapi](https://brapi.dev) (gratuito)
- Conta de e-mail Gmail com **senha de app** gerada

---

## Configuração

### 1. Clone o repositório

```bash
git clone https://github.com/DaviQueirozLima/StockAlert.git
cd StockAlert
git checkout refactor/final-cleanup-and-adjustments
```

### 2. Configure o `appsettings.json`

Edite o arquivo `src/StockAlert.API/appsettings.json` com suas credenciais:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=StockAlert;Username=seu_usuario;Password=sua_senha"
  },
  "Settings": {
    "Jwt": {
      "SigningKey": "uma_chave_secreta_longa_e_aleatoria_aqui",
      "ExpirationInMinutes": 1440
    }
  },
  "Google": {
    "ClientId": "seu_client_id.apps.googleusercontent.com"
  },
  "Brapi": {
    "BaseUrl": "https://brapi.dev/api",
    "Token": "seu_token_brapi"
  },
  "EmailSettings": {
    "Host": "smtp.gmail.com",
    "Port": 587,
    "UserName": "seu_email@gmail.com",
    "Password": "sua_senha_de_app_gmail"
  }
}
```

> **Dica:** Para gerar a senha de app do Gmail, acesse Conta Google → Segurança → Verificação em duas etapas → Senhas de app.

### 3. Execute as migrações

```bash
cd src/StockAlert.API
dotnet ef database update --project ../StockAlert.Infrastructure
```

### 4. Execute a aplicação

```bash
dotnet run --project src/StockAlert.API
```

A API estará disponível em `https://localhost:7000` e o Swagger em `https://localhost:7000/swagger`.

---

## Executando com Docker

```bash
docker build -t stockalert -f src/StockAlert.API/Dockerfile .
docker run -p 8080:8080 --env-file .env stockalert
```

---

## Endpoints

Após iniciar a aplicação, acesse o **Swagger UI** para explorar e testar todos os endpoints com autenticação JWT integrada.

| Método | Rota | Descrição | Auth |
|---|---|---|---|
| POST | `/api/auth/google` | Login com token Google, retorna JWT | Não |
| POST | `/api/stocks/register` | Cadastra uma ação para monitoramento | Sim |
| POST | `/api/alertrules` | Cria uma regra de alerta | Sim |
| PUT | `/api/alertrules/{id}` | Atualiza uma regra de alerta | Sim |
| DELETE | `/api/alertrules/{id}` | Remove uma regra de alerta | Sim |

---

## Como funciona o monitoramento

O `StockMonitorWorker` é um `BackgroundService` que executa continuamente em segundo plano:

1. Busca todas as regras de alerta ativas no banco
2. Para cada regra, consulta o preço atual da ação na API da Brapi
3. Avalia a condição configurada (ex: preço > alvo)
4. Verifica o cooldown — evita disparar o mesmo alerta repetidamente em curto intervalo (padrão: 15 minutos)
5. Se a condição foi atingida e o cooldown passou, envia o e-mail de alerta
6. Registra o evento no histórico de notificações (`NotificationHistories`)
7. Se a regra for `NotifyOnce`, desativa-a automaticamente após o primeiro disparo

O worker aguarda **10 segundos** entre cada ciclo de verificação.

---

## Modo de teste local (FakeEmailService)

Para testar o funcionamento do worker sem configurar um servidor SMTP real, o projeto inclui um `FakeEmailService`. Em vez de enviar o e-mail, ele imprime o conteúdo completo no console da aplicação.

Para ativá-lo, basta trocar o registro no `Program.cs`:

```csharp
// Produção (SMTP real):
builder.Services.AddScoped<IEmailService, SmtpEmailService>();

// Desenvolvimento (sem SMTP, saída no console):
builder.Services.AddScoped<IEmailService, FakeEmailService>();
```

---

## Testes

```bash
dotnet test
```

Os testes cobrem o `RegisterAlertRuleUseCase` e o `RegisterAlertRuleValidator`, validando cenários de sucesso e de falha com dados inválidos ou ação não cadastrada.

---

## Autor

**Davi Queiroz Lima**  
Curso Superior de Tecnologia em Análise e Desenvolvimento de Sistemas  
Centro Universitário UniFacema — Campus Caxias/MA