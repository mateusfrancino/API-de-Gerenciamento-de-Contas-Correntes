using Dapper;
using MediatR;
using Microsoft.Data.Sqlite;
using Questao5.Infrastructure.Sqlite;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

public class CriarMovimentoHandler : IRequestHandler<CriarMovimentoRequest, CriarMovimentoResponse>
{
    private readonly DatabaseConfig _databaseConfig;

    public CriarMovimentoHandler(DatabaseConfig databaseConfig)
    {
        _databaseConfig = databaseConfig;
    }

    public async Task<CriarMovimentoResponse> Handle(CriarMovimentoRequest request, CancellationToken cancellationToken)
    {
        // Validações de negócio
        if (request.IdContaCorrente != null && IsValidAccount(request.IdContaCorrente))
            return new CriarMovimentoResponse { Success = false, ErrorMessage = "INVALID_ACCOUNT: Conta corrente inválida", MovementId = "0" };

        if (request.IdContaCorrente != null && IsActiveAccount(request.IdContaCorrente))
            return new CriarMovimentoResponse { Success = false, ErrorMessage = "INACTIVE_ACCOUNT: Conta corrente inativa", MovementId = "0" };

        if (request.Valor <= 0)
            return new CriarMovimentoResponse { Success = false, ErrorMessage = "INVALID_VALUE: Valor inválido", MovementId = "0" };

        if (request.TipoMovimento != "D" && request.TipoMovimento != "C")
            return new CriarMovimentoResponse { Success = false, ErrorMessage = "INVALID_TYPE: Tipo de movimento inválido.", MovementId = "0" };

        // Persistir o movimento no banco de dados
        var movementId = await SaveMovement(request);

        return new CriarMovimentoResponse { Success = true, ErrorMessage = null, MovementId = movementId };
    }

    private async Task<string> SaveMovement(CriarMovimentoRequest request)
    {
        using (var connection = new SqliteConnection(_databaseConfig.Name))
        {
            await connection.OpenAsync();
            var parameters = new
            {
                IdMovimento = request.IdMovimento,
                IdContaCorrente = request.IdContaCorrente,
                DataMovimento = request.DataMovimento,
                TipoMovimento = request.TipoMovimento,
                Valor = request.Valor

            };

            var query = @"
                        INSERT INTO MOVIMENTO (idmovimento, idcontacorrente, datamovimento, tipomovimento, valor)
                        VALUES (@IdMovimento, @IdContaCorrente, @DataMovimento, @TipoMovimento, @Valor);
                        SELECT last_insert_rowid();";

            _ = await connection.ExecuteScalarAsync<bool>(query, parameters, commandType: CommandType.Text);

            return request.IdMovimento ?? string.Empty;

        }
    }

    private bool IsValidAccount(string accountId)
    {
        using (var connection = new SqliteConnection(_databaseConfig.Name))
        {
            connection.Open();

            var query = "SELECT EXISTS(SELECT 1 FROM contacorrente WHERE idcontacorrente = @AccountId)";
            var command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@AccountId", accountId);

            var result = command.ExecuteScalar();

            return Convert.ToBoolean(result);
        }
    }

    private bool IsActiveAccount(string accountId)
    {
        using (var connection = new SqliteConnection(_databaseConfig.Name))
        {
            connection.Open();

            var query = "SELECT EXISTS(SELECT 1 FROM contacorrente WHERE idcontacorrente = @AccountId AND ativo = 1)";
            var command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@AccountId", accountId);

            var result = command.ExecuteScalar();

            return Convert.ToBoolean(result);
        }
    }
}
