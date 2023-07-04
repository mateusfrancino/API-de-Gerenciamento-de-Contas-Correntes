using Dapper;
using MediatR;
using Microsoft.Data.Sqlite;
using Questao5.Infrastructure.Sqlite;
using System;
using System.Threading;
using System.Threading.Tasks;

public class SaldoHandler : IRequestHandler<GetSaldoRequest, GetSaldoResponse>
{
    private readonly DatabaseConfig _databaseConfig;

    public SaldoHandler(DatabaseConfig databaseConfig)
    {
        _databaseConfig = databaseConfig;
    }

    public async Task<GetSaldoResponse> Handle(GetSaldoRequest request, CancellationToken cancellationToken)
    {
        // Validações de negócio
        if (request.IdContaCorrente != null && ValidaConta(request.IdContaCorrente))
            return new GetSaldoResponse { Success = false, ErrorMessage = "INVALID_ACCOUNT: Conta corrente inválida" };

        if (request.IdContaCorrente != null && ValidaContaAtiva(request.IdContaCorrente))
            return new GetSaldoResponse { Success = false, ErrorMessage = "INACTIVE_ACCOUNT: Conta corrente inativa" };

        
        decimal saldo = await CalcularSaldo(request.IdContaCorrente);

        // Retorno
        return new GetSaldoResponse
        {
            Success = true,
            NumeroConta = request.IdContaCorrente,
            TitularConta = GetTitularConta(request.IdContaCorrente),
            ResponseDateTime = DateTime.Now,
            Saldo = saldo
        };
    }

    private async Task<decimal> CalcularSaldo(string idContaCorrente)
    {
        using (var connection = new SqliteConnection(_databaseConfig.Name))
        {
            await connection.OpenAsync();

            var query = @"
            SELECT SUM(CASE WHEN tipomovimento = 'C' THEN valor ELSE -valor END) AS saldo
            FROM MOVIMENTO
            WHERE idcontacorrente = @IdContaCorrente;
        ";

            var saldo = await connection.ExecuteScalarAsync<decimal>(query, new { IdContaCorrente = idContaCorrente });

            return saldo;
        }
    }

    private bool ValidaConta(string accountId)
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

    private bool ValidaContaAtiva(string accountId)
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
    private string GetTitularConta(string accountId)
    {
        using (var connection = new SqliteConnection(_databaseConfig.Name))
        {
            connection.Open();

            var query = "SELECT nome FROM contacorrente WHERE idcontacorrente = @AccountId";
            var command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@AccountId", accountId);

            var titular = command.ExecuteScalar() as string;

            return titular ?? string.Empty;
        }
    }

}
