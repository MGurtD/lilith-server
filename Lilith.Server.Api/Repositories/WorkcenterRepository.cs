using Dapper;
using Lilith.Server.Entities;
using Lilith.Server.Helpers.Database;

namespace Lilith.Server.Repositories;

public interface IWorkcenterRepository
{
    Task<List<Workcenter>> GetAllWorkcenters();    
    Task<bool> KeepAliveWorkcenter(Guid workcenterId, DateTime timestamp);       
    Task<Workcenter>GetWorkcenterById(Guid workcenterId);
}
public class WorkcenterRepository : IWorkcenterRepository
{
    private readonly DataContext _context;

    public WorkcenterRepository(DataContext context)
    {
        _context = context;
    }

    public async Task<List<Workcenter>> GetAllWorkcenters()
    {
        using var connection = _context.CreateConnection();
        var sql = """
    SELECT "WorkcenterId", "WorkcenterName", "WorkcenterDescription", "AreaId", 
           "AreaName", "AreaDescription", "CurrentDay", "CurrentTime", "ShiftId", "ShiftName", "ShiftDetailId", 
           "ShiftStartTime", "ShiftEndTime","WorkcenterDataId", "StatusName", "StatusDescription", 
           "StatusColor", "StatusStartTime", "StatusEndTime", "WorkOrderCode", 
           "ReferenceCode", "ReferenceDescription", "PhaseCode", "PhaseDescription", 
           "PhaseStartTime", "PhaseEndTime", "CounterOk", "CounterKo"
    FROM realtime."Workcenters"
    """;

        var result = await connection.QueryAsync<Workcenter>(sql);
        return result.ToList();
    }

    public async Task<bool> KeepAliveWorkcenter(Guid workcenterId, DateTime timestamp)
    {
        using var connection = _context.CreateConnection();
        var sql = """
            SET TIMEZONE='Europe/Madrid';
            UPDATE realtime."Workcenters"
            SET "CurrentTime" = @Timestamp,
                "StatusEndTime" = CASE WHEN "StatusName" IS NULL THEN NULL ELSE @Timestamp END,
                "PhaseEndTime" = CASE WHEN "PhaseCode" IS NULL THEN NULL ELSE @Timestamp END
            WHERE "WorkcenterId" = @WorkcenterId
            """;
        var affectedRows = await connection.ExecuteAsync(sql, new { Timestamp = timestamp, WorkcenterId = workcenterId });
        if (affectedRows > 0)
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    

    public async Task<Workcenter> GetWorkcenterById(Guid workcenterId)
    {
        using var connection = _context.CreateConnection();
        var sql = """
                SELECT "WorkcenterId", "WorkcenterName", "WorkcenterDescription", "AreaId", 
                       "AreaName", "AreaDescription", "CurrentDay", "CurrentTime", "ShiftId", "ShiftName", "ShiftDetailId", 
                       "ShiftStartTime", "ShiftEndTime","WorkcenterDataId", "StatusName", "StatusDescription", 
                       "StatusColor", "StatusStartTime", "StatusEndTime", "WorkOrderCode", 
                       "ReferenceCode", "ReferenceDescription", "PhaseCode", "PhaseDescription", 
                       "PhaseStartTime", "PhaseEndTime", "CounterOk", "CounterKo"
                FROM realtime."Workcenters"
                WHERE "WorkcenterId" = @Id
                """;
        var result = connection.QuerySingle<Workcenter>(sql, new { Id = workcenterId });
        return result;
    }
}
