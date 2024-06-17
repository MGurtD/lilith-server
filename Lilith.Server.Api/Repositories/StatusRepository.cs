using Dapper;
using Lilith.Server.Contracts.Responses;
using Lilith.Server.Helpers.Database;

namespace Lilith.Server.Repositories;

public interface IStatusRepository
{    
    Task<bool> SetStatusToWorkcenter(StatusResponse statusResponse, Guid workcenterId);
}
public class StatusRepository:IStatusRepository
{
    private readonly DataContext _context;

    public StatusRepository(DataContext context)
    {
        _context = context;
    }

    public async Task<bool> SetStatusToWorkcenter(StatusResponse statusResponse, Guid workcenterId)
    {
        using var connection = _context.CreateConnection();
        var sql = """
            SET TIMEZONE='Europe/Madrid';
            UPDATE realtime."Workcenters"
            SET "StatusName" = @StatusName,
                "StatusDescription" = @StatusDescription,
                "StatusColor" = @StatusColor,
                "StatusStartTime" = NOW(),
                "StatusEndTime" = NOW()
            WHERE "WorkcenterId" = @WorkcenterId       
            """;
        var affectedRows = await connection.ExecuteAsync(sql, new
        {
            StatusName = statusResponse.Name, 
            StatusDescription = statusResponse.Description,
            StatusColor = statusResponse.Color,
            WorkcenterId = workcenterId
        });
        if (affectedRows > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
