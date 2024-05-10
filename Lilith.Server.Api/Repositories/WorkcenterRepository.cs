using Dapper;
using Lilith.Server.Entities;
using Lilith.Server.Helpers.Database;

namespace Lilith.Server.Repositories;

public interface IWorkcenterRepository
{
    Task<List<Workcenter>> GetAllWorkcentersAsync();
    Task<bool> UpdateWorkcenterShift(Guid workcenterId, Guid shiftDetailId, DateTime startTime);
    Task<bool> KeepAliveWorkcenter(Guid workcenterId, DateTime timestamp);
}
public class WorkcenterRepository: IWorkcenterRepository
{
    private readonly DataContext _context;

    public WorkcenterRepository(DataContext context)
    {
        _context = context;
    }

    public async Task<List<Workcenter>> GetAllWorkcentersAsync()
    {
        using var connection = _context.CreateConnection();
        var sql = """
    SELECT "WorkcenterId", "WorkcenterName", "WorkcenterDescription", "AreaId", 
           "AreaName", "AreaDescription", "ShiftId", "ShiftName", "ShiftDetailId", 
           "ShiftStartTime", "ShiftEndTime", "StatusName", "StatusDescription", 
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
            UPDATE realtime."Workcenters"
            SET "ShiftEndTime" = @Timestamp
            WHERE "WorkcenterId" = @WorkcenterId
            """;
        var affectedRows = await connection.ExecuteAsync(sql, new { Timestamp = timestamp, WorkcenterId = workcenterId });
        if(affectedRows > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
        
    }

    public async Task<bool> UpdateWorkcenterShift(Guid workcenterId, Guid shiftDetailId, DateTime timestamp)
    {
        using var connection = _context.CreateConnection();
        var sql = """
            UPDATE realtime."Workcenters"
            SET "ShiftDetailId" = @ShiftDetailId,
                 "ShiftStartTime" = @Timestamp,
                 "ShiftEndTime" = NOW()
            WHERE "WorkcenterId" = @WorkcenterId
            """;
        var affectedRows = await connection.ExecuteAsync(sql, new {ShiftDetailId = shiftDetailId, Timestamp = timestamp, WorkcenterId = workcenterId});
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
