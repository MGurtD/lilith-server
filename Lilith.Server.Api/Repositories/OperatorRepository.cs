using Dapper;
using Lilith.Server.Entities;
using Lilith.Server.Helpers.Database;

namespace Lilith.Server.Repositories;

public interface IOperatorRepository
{
    Task<bool> SetOperatorToWorkcenter(Operator _operator, Guid workcenterId);
    Task<bool> UnsetOperatorFromWorkcenter(Operator _operator, Guid workcenterId);
}
public class OperatorRepository : IOperatorRepository
{
    private readonly DataContext _context;

    public OperatorRepository(DataContext context)
    {
        _context = context;
    }
    public async Task<bool> SetOperatorToWorkcenter(Operator _operator, Guid workcenterId)
    {
        try
        {
            using var connection = _context.CreateConnection();
            var sql = """
            SET TIMEZONE='Europe/Madrid';
            INSERT INTO realtime."Operators"("WorkcenterId", "OperatorId",  "OperatorCode", "OperatorName", "OperatorTypeName", "OperatorTypeDescription", "StartTime", "EndTime")
            VALUES(@WorkcenterId, @OperatorId, @OperatorCode, @OperatorName, @OperatorTypeName, @OperatorTypeDescription, NOW(), NOW())
            """;
            var affectedRows = await connection.ExecuteAsync(sql, new
            {
                WorkcenterId = workcenterId,
                _operator.OperatorId,
                _operator.OperatorCode,
                _operator.OperatorName,
                _operator.OperatorTypeName,
                _operator.OperatorTypeDescription
            });

            return affectedRows > 0;
        }
        catch (Exception ex) { 
            Console.WriteLine(ex.ToString());
            return false;
        }
    }

    public async Task<bool> UnsetOperatorFromWorkcenter(Operator _operator, Guid workcenterId)
    {
        try
        {
            using var connection = _context.CreateConnection();
            var sql = """
            DELETE FROM realtime."Operators" WHERE "WorkcenterId" = @WorkcenterId AND "OperatorId" = @OperatorId
            """;
            var affectedRows = await connection.ExecuteAsync(sql, new
            {
                WorkcenterId = workcenterId,
                _operator.OperatorId
            });

            return affectedRows > 0;
        }catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return false;
        }
    }
}
