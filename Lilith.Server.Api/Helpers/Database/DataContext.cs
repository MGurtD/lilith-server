namespace Lilith.Server.Helpers.Database;

using System.Data;
using Dapper;
using Npgsql;

public class DataContext
{
    private string _connectionString;

    public DataContext()
    {
        var configurationLoader = new ConfigurationLoader();
        IConfiguration configuration = configurationLoader.LoadConfiguration();

        _connectionString = configuration["ConnectionStrings:Default"] ?? "";
    }

    public IDbConnection CreateConnection()
    {
        return new NpgsqlConnection(_connectionString);
    }

    public async Task Init()
    {
        await _initSchemas();
        await _initTables();
    }

    private async Task _initSchemas()
    {
        // create schema if it doesn't exist
        using var connection = CreateConnection();

        var sql = $"CREATE SCHEMA IF NOT EXISTS  \"realtime\"";
        await connection.ExecuteAsync(sql);

        sql = $"CREATE SCHEMA IF NOT EXISTS  \"data\"";
        await connection.ExecuteAsync(sql);
    }

    private async Task _initTables()
    {
        // create tables if they don't exist
        using var connection = CreateConnection();
        await _initDataModel(connection);
        await _initRealtimeModel(connection);
    }

    private async Task _initDataModel(IDbConnection connection)
    {
        var sql = """
                CREATE TABLE IF NOT EXISTS data."HistorialRow" (
                    "Id" uuid NOT NULL,
                    "ShiftId" uuid NOT NULL,
                    "WorkcenterId" uuid NOT NULL,
                    "StartTime" timestamp without time zone NOT NULL DEFAULT now(),
                    "EndTime" timestamp without time zone NOT NULL DEFAULT now(),
                    CONSTRAINT "PK_HistorialRow" PRIMARY KEY ("Id")
                );
            """;
        await connection.ExecuteAsync(sql);
    }

    private async Task _initRealtimeModel(IDbConnection connection)
    {
        var sql = """SELECT 1""";
        await connection.ExecuteAsync(sql);
    }
}