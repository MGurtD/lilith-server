using Dapper;
using Lilith.Server.Entities;
using Lilith.Server.Helpers.Database;

namespace Lilith.Server.Repositories;

public interface IHistoricRowRespository
{
    Task<HistoricRow> GetById(int id);
    Task Create(HistoricRow historicRow);
    Task Update(HistoricRow historicRow);
}

public class HistoricRowRespository : IHistoricRowRespository
{
    private DataContext _context;

    public HistoricRowRespository(DataContext context)
    {
        _context = context;
    }

    public async Task Create(HistoricRow historicRow)
    {
        using var connection = _context.CreateConnection();
        var sql = $"""
        INSERT INTO data."HistorialRow" ("Id", "WorkcenterId", "ShiftId", "StartTime", "EndTime")
            VALUES (@Id, @WorkcenterId, @ShiftId, @StartTime, @EndTime)
        """;
        await connection.ExecuteAsync(sql, historicRow);
    }

    public Task<HistoricRow> GetById(int id)
    {
        throw new NotImplementedException();
    }

    public Task Update(HistoricRow historicRow)
    {
        throw new NotImplementedException();
    }
}
