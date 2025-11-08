# OrgnizadorMottu API (.NET 9)

API RESTful em .NET 9 (Minimal API) com boas práticas REST para os modelos de dados definidos.

## Nomes dos integrantes

- João Vitor Broggine Lopes - RM557129
- João Victor Rocha Cândido - RM554727
- Eduardo Augusto Pelegrino Einsfeldt - RM556460

## Justificativa da arquitetura

Optamos pela arquitetura Minimal API do .NET 9 por sua simplicidade e performance, facilitando o desenvolvimento de APIs RESTful modernas e enxutas. Utilizamos boas práticas REST, como HATEOAS, status codes adequados e paginação. O acesso a dados é feito via Oracle, conforme requisitos do projeto.

Para a Sprint 4, arquitetura foi transformada de Minimal API para a tradicional arquitetura do Asp.Net, para a solução de problemas em relação ao versioning.
## Instruções de execução da API

1. Configure a connection string Oracle em `appsettings.json`:
   ```json
   "ConnectionStrings": {
     "Oracle": "User Id=YOUR_USER;Password=YOUR_PASSWORD;Data Source=YOUR_HOST:1521/YOUR_SERVICE"
   }
   ```
2. Execute os comandos abaixo:
   ```bash
   dotnet build
   dotnet run --project src/Mottu.Api
   ```
3. Acesse o Swagger UI para explorar a API:  
   [http://localhost:5000/swagger](http://localhost:5000/swagger)  
   *(ou porta exibida no console)*

## Exemplos de uso dos endpoints

### Usuários

- Listar usuários (paginado):
  ```http
  GET /api/usuarios?page=1&pageSize=10
  ```
- Buscar por CPF:
  ```http
  GET /api/usuarios/{cpf}
  ```
- Criar novo usuário:
  ```http
  POST /api/usuarios
  ```
- Atualizar usuário:
  ```http
  PUT /api/usuarios/{cpf}
  ```
- Remover usuário:
  ```http
  DELETE /api/usuarios/{cpf}
  ```

### Motos

- Listar motos (paginado):
  ```http
  GET /api/motos?page=1&pageSize=10
  ```
- Buscar por placa:
  ```http
  GET /api/motos/{placa}
  ```
- Criar nova moto:
  ```http
  POST /api/motos
  ```
- Atualizar moto:
  ```http
  PUT /api/motos/{placa}
  ```
- Remover moto:
  ```http
  DELETE /api/motos/{placa}
  ```

### Endereços

- Listar endereços (paginado):
  ```http
  GET /api/enderecos?page=1&pageSize=10
  ```
- Buscar por CEP:
  ```http
  GET /api/enderecos/{nrCep}
  ```
- Criar novo endereço:
  ```http
  POST /api/enderecos
  ```
- Atualizar endereço:
  ```http
  PUT /api/enderecos/{nrCep}
  ```
- Remover endereço:
  ```http
  DELETE /api/enderecos/{nrCep}
  ```
  
---

> **Observações de modelagem**
>
> - `CPF` e `NR_CEP` são `string` para preservar zeros à esquerda.
> - Relacionamentos entre `Usuario` e `Moto` respeitam o diagrama proposto.
> - O banco Oracle deve existir previamente; não há migrations automáticas.
> - HATEOAS disponível nas respostas de coleção e recurso.
