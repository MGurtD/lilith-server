using Dapper;
using Lilith.Server.Entities;
using Lilith.Server.Helpers.Database;
using System.Data;

namespace Lilith.Server.Repositories;

public interface IWorkcenterRepository
{
    Task<IEnumerable<Workcenter>> GetAllWorkcenters();    
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

    public async Task<IEnumerable<Workcenter>> GetAllWorkcenters()
    {        
        using var connection = _context.CreateConnection();
        var sql = """
     SELECT wc."WorkcenterId", wc."WorkcenterName", wc."WorkcenterDescription", wc."AreaId", 
            wc."AreaName", wc."AreaDescription", wc."CurrentDay", wc."CurrentTime", wc."ShiftId", wc."ShiftName", wc."ShiftDetailId", 
            wc."ShiftStartTime", wc."ShiftEndTime",wc."WorkcenterDataId", wc."StatusName", wc."StatusDescription", 
            wc."StatusColor", wc."StatusStartTime", wc."StatusEndTime", 
            op."OperatorId", op."OperatorCode", op."OperatorName", op."OperatorTypeName", op."OperatorTypeDescription",
            op."StartTime", op."EndTime",
            wo."WorkOrderCode", wo."ReferenceCode", wo."ReferenceDescription", 
            wo."PhaseId", wo."PhaseCode", wo."PhaseDescription", wo."PhaseStartTime", wo."PhaseEndTime",
            wo."CounterOk", wo."CounterKo"	   
     FROM realtime."Workcenters" wc
         LEFT JOIN realtime."Operators" op ON wc."WorkcenterId" = op."WorkcenterId"
         LEFT JOIN realtime."WorkOrders" wo ON wc."WorkcenterId" = wo."WorkcenterId"
     """;

        var workcenterDict = new Dictionary<Guid, Workcenter>();

        var result = await connection.QueryAsync<Workcenter, Operator, WorkOrderPhase, Workcenter>(
            sql,
            (workcenter, _operator, workOrderPhase) =>
            {
                if (!workcenterDict.TryGetValue(workcenter.WorkcenterId, out var currentWorkcenter))
                {
                    currentWorkcenter = workcenter;
                    currentWorkcenter.Operator = new List<Operator>();
                    currentWorkcenter.WorkOrderPhase = new List<WorkOrderPhase>();
                    workcenterDict.Add(currentWorkcenter.WorkcenterId, currentWorkcenter);
                }

                if (_operator != null && !currentWorkcenter.Operator.Any(o => o.OperatorId == _operator.OperatorId))
                {
                    currentWorkcenter.Operator.Add(_operator);
                }

                if (workOrderPhase != null && !currentWorkcenter.WorkOrderPhase.Any(wp => wp.PhaseId == workOrderPhase.PhaseId))
                {
                    currentWorkcenter.WorkOrderPhase.Add(workOrderPhase);
                }

                return currentWorkcenter;
            },
            splitOn: "OperatorId,PhaseId"
        );

        return result.Distinct().ToList();
    }


    public async Task<bool> KeepAliveWorkcenter(Guid workcenterId, DateTime timestamp)
    {
        using var connection = _context.CreateConnection();
        var sql = """
            SET TIMEZONE='Europe/Madrid';
            UPDATE realtime."Workcenters"
            SET "CurrentTime" = @Timestamp
            WHERE "WorkcenterId" = @WorkcenterId;
            UPDATE realtime."Operators"
            SET "EndTime" = @Timestamp
            WHERE "WorkcenterId" = @WorkcenterId;
            UPDATE realtime."WorkOrders"
            SET "PhaseEndTime" = @Timestamp
            WHERE "WorkcenterId" = @WorkcenterId;
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
                SELECT wc."WorkcenterId", wc."WorkcenterName", wc."WorkcenterDescription", wc."AreaId", 
                       wc."AreaName", wc."AreaDescription", wc."CurrentDay", wc."CurrentTime", wc."ShiftId", wc."ShiftName", wc."ShiftDetailId", 
                       wc."ShiftStartTime", wc."ShiftEndTime",wc."WorkcenterDataId", wc."StatusName", wc."StatusDescription", 
                       wc."StatusColor", wc."StatusStartTime", wc."StatusEndTime", 
                       op."OperatorId", op."OperatorCode", op."OperatorName", op."OperatorTypeName", op."OperatorTypeDescription",
                       op."StartTime", op."EndTime",
                       wo."WorkOrderCode", wo."ReferenceCode", wo."ReferenceDescription", 
                       wo."PhaseId", wo."PhaseCode", wo."PhaseDescription", wo."PhaseStartTime", wo."PhaseEndTime",
                       wo."CounterOk", wo."CounterKo"	   
                FROM realtime."Workcenters" wc
                    LEFT JOIN realtime."Operators" op ON wc."WorkcenterId" = op."WorkcenterId"
                    LEFT JOIN realtime."WorkOrders" wo ON wc."WorkcenterId" = wo."WorkcenterId"
                WHERE wc."WorkcenterId" = @Id
                """;
        var result = connection.QuerySingle<Workcenter>(sql, new { Id = workcenterId });
        return result;
    }
}
