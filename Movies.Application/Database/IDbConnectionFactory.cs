using System.Data;
using Npgsql;

namespace Movies.Application.Database;

public interface IDbConnectionFactory
{
    public Task<IDbConnection> CreateConnectionAsync(CancellationToken token = default);
}

public class NpgSqlConnectionFactory(string connectionString) : IDbConnectionFactory
{
    private readonly string _connectionString = connectionString;


    public async Task<IDbConnection> CreateConnectionAsync(CancellationToken token = default)
    {
        var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken: token);
        return connection;
    }
}