# OrganizadorMottu API (.NET 9) — Instruções para rodar localmente e acessar Swagger

API RESTful em .NET 9 (Asp.Net tradicional) com boas práticas REST para os modelos de dados definidos.

## Nomes dos integrantes

- João Vitor Broggine Lopes - RM557129  
- João Victor Rocha Cândido - RM554727  
- Eduardo Augusto Pelegrino Einsfeldt - RM556460

## Observação sobre arquitetura
A solução foi desenvolvida originalmente como Minimal API e, na Sprint 4, migrada para a arquitetura tradicional do ASP.NET para resolver questões de versioning.

---

## Pré-requisitos
- .NET SDK (versão compatível com .NET 9). Verifique com:
  dotnet --info
- (Opcional, mas recomendado) dotnet-watch para hot-reload:
  dotnet tool install --global dotnet-watch
- Se usar HTTPS local, execute:
  dotnet dev-certs https --trust

## Configuração da connection string Oracle
Edite o arquivo de configuração do projeto principal (ex.: `OrganizadorMottu/appsettings.json`) e ajuste sua connection string Oracle:

```json
"ConnectionStrings": {
  "Oracle": "User Id=YOUR_USER;Password=YOUR_PASSWORD;Data Source=YOUR_HOST:1521/YOUR_SERVICE"
}
```

Observação: o banco Oracle deve existir previamente; este projeto não cria migrations automáticas.

## Passo a passo para rodar e acessar o Swagger

1. No diretório raiz do repositório, restaure dependências:
   ```bash
   dotnet restore
   ```

2. (Opcional) Compile para checar erros:
   ```bash
   dotnet build
   ```

3. Verifique qual é o arquivo de projeto (.csproj) dentro da pasta do projeto principal:
   - Linux/mac:
     ```bash
     ls OrganizadorMottu/*.csproj
     ```
   - Windows (PowerShell):
     ```powershell
     dir .\OrganizadorMottu\*.csproj
     ```

4. Rode a API apontando para o projeto correto:
   - Exemplo (quando o projeto estiver em OrganizadorMottu):
     ```bash
     dotnet run --project ./OrganizadorMottu
     ```
     ou, explicitando o arquivo:
     ```bash
     dotnet run --project ./OrganizadorMottu/OrganizadorMottu.csproj
     ```

   - Se preferir hot-reload (recomendo para dev):
     ```bash
     dotnet watch run --project ./OrganizadorMottu
     ```

5. Se o Swagger só é habilitado em ambiente Development, execute definindo a variável de ambiente:
   - Linux/mac:
     ```bash
     export ASPNETCORE_ENVIRONMENT=Development
     dotnet run --project ./OrganizadorMottu
     ```
   - Windows (PowerShell):
     ```powershell
     $env:ASPNETCORE_ENVIRONMENT = 'Development'
     dotnet run --project .\OrganizadorMottu
     ```

6. Depois que a aplicação subir, o console geralmente exibe algo como:
   Now listening on: http://localhost:5000
   Now listening on: https://localhost:5001

   Abra no navegador:
   - http://localhost:5000/swagger
   - ou https://localhost:5001/swagger

   Use a URL/porta exibida no console se for diferente.

## Verificando se o Swagger está habilitado (no código)
Abra `OrganizadorMottu/Program.cs` (ou `Startup.cs`) e verifique a presença das chamadas:

```csharp
// registrar serviços
builder.Services.AddSwaggerGen();

// no pipeline
app.UseSwagger();
app.UseSwaggerUI();
```

Se essas linhas não existirem, adicione-as (ou peça para eu sugerir o trecho exato). Normalmente em ambientes de produção o Swagger pode estar condicionado a `if (app.Environment.IsDevelopment())`.

## Executar testes
- Rodar todos os testes da solução:
  ```bash
  dotnet test
  ```
- Ou executar apenas o projeto de testes:
  ```bash
  dotnet test ./OrganizadorMottuTests
  ```

## Troubleshooting rápido
- Porta diferente da esperada: use a URL mostrada no console.
- Erro de certificado HTTPS local: execute `dotnet dev-certs https --trust`.
- Conexão Oracle falhando: verifique user/password/host/service e se a rede/instância estão acessíveis.
- Swagger não aparece: cheque `ASPNETCORE_ENVIRONMENT`, presença de AddSwaggerGen/UseSwagger e mensagens no console.

---

Boas práticas:
- Não comitar secrets (connection strings com credenciais). Use variáveis de ambiente ou arquivos de configuração seguros.
- Teste com ambiente `Development` local para acessar documentação via Swagger.
