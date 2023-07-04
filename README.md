# API de Gerenciamento de Movimentos Financeiros

## Descrição
Este projeto consiste em uma API desenvolvida em .NET Core para gerenciar movimentos financeiros em contas correntes. A API permite criar novos movimentos, consultar o saldo atual da conta corrente e realizar outras operações relacionadas a movimentos financeiros.

## Tecnologias Utilizadas
- .NET Core
- Entity Framework Core
- SQLite


## Requisitos
- .NET Core SDK
- SQLite (banco de dados embutido)

## Instalação e Execução
1. Clone este repositório em sua máquina local.
2. Abra o projeto em um editor de código ou IDE de sua preferência.
3. Execute o comando `dotnet restore` para restaurar as dependências do projeto.
4. Configure a conexão com o banco de dados no arquivo `appsettings.json`.
5. Execute o comando `dotnet ef database update` para aplicar as migrações do Entity Framework e criar o banco de dados.
6. Execute o comando `dotnet run` para iniciar a API.

## Endpoints
- `POST /api/movimento`: Cria um novo movimento financeiro.
- `GET /api/saldo`: Retorna o saldo atual da conta corrente.

## Contribuição
Contribuições são bem-vindas! Sinta-se à vontade para enviar pull requests ou relatar problemas encontrados.


