# OrgnizadorMottu API (.NET 9)

API RESTful em .NET 9 (Minimal API) com boas práticas REST sobre o modelo de dados do enunciado:

- Entidades: `T_MT_Usuario`, `T_MT_Moto`, `T_MT_Endereco` (+ `T_MT_Funcionario` como extra).
- CRUD completo para as 3 entidades principais.
- Paginação em todos os `GET` de coleção (`page`, `pageSize`).
- HATEOAS em respostas (links `self`, `next`, `prev`, `update`, `delete`).
- Códigos HTTP adequados: `200`, `201`, `204`, `404`, `409`.
- Swagger/OpenAPI com exemplos.

## Como rodar

1. Configure a connection string Oracle em `appsettings.json`:
   ```
   "ConnectionStrings": {
     "Oracle": "User Id=YOUR_USER;Password=YOUR_PASSWORD;Data Source=YOUR_HOST:1521/YOUR_SERVICE"
   }
   ```
2. `dotnet build`
3. `dotnet run --project src/Mottu.Api`

Acesse: [Swagger UI](http://localhost:5000/swagger) (ou porta exibida no console).

## Endpoints

- `GET /api/usuarios?page=1&pageSize=10`
- `GET /api/usuarios/{cpf}`
- `POST /api/usuarios`
- `PUT /api/usuarios/{cpf}`
- `DELETE /api/usuarios/{cpf}`

- `GET /api/motos?page=1&pageSize=10`
- `GET /api/motos/{placa}`
- `POST /api/motos`
- `PUT /api/motos/{placa}`
- `DELETE /api/motos/{placa}`

- `GET /api/enderecos?page=1&pageSize=10`
- `GET /api/enderecos/{nrCep}`
- `POST /api/enderecos`
- `PUT /api/enderecos/{nrCep}`
- `DELETE /api/enderecos/{nrCep}`

## Observações de modelagem

- Para simplificação, o `CPF` foi mapeado como `string` para preservar zeros à esquerda, e `NR_CEP` também como `string`.
- O diagrama mostra chaves cruzadas entre `Usuario` e `Moto` (`CD_PLACA` e `CD_CPF`). O mapeamento inclui:
  - `Usuario` 1:1 opcional com `Moto` via `CD_PLACA`.
  - `Moto` N:1 opcional com `Usuario` via `CD_CPF`.
- O banco deve existir previamente; o projeto não aplica migrações.

## Próximos passos (sugestões)

- Validações de domínio mais ricas (CPF/CEP).
- Filtro e ordenação nos `GET`.
- Autenticação/Autorização (Bearer/Keycloak).
- Testes de integração com `WebApplicationFactory`.