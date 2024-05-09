using Dapper;
using Lilith.Server.Entities;
using Lilith.Server.Helpers.Database;

namespace Lilith.Server.Repositories;

public interface IWorkcenterRepository
{
    Task<List<Workcenter>> GetAllWorkcentersAsync();
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

    public async Task<bool> UpdateShift(Guid workcenterId, Guid shiftDetailId)
    {
        return true;
    }
}
