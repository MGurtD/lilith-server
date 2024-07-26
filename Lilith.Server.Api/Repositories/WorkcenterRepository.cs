using Dapper;
using Lilith.Server.Entities;
using Lilith.Server.Helpers.Database;
using System.Data;

namespace Lilith.Server.Repositories;

public interface IWorkcenterRepository
{
    Task<IEnumerable<Workcenter>> GetAllWorkcenters();    
    Task<bool> KeepAliveWorkcenter(Workcenter workcenter, DateTime timestamp);       
    Task<Workcenter?>GetWorkcenterById(Guid workcenterId);
    Task<int>CreateWorkcenterData(Workcenter workcenter);
    Task<bool> SetDataToWorkcenter(Workcenter workcenter);
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
                    currentWorkcenter.Operators = new List<Operator>();
                    currentWorkcenter.WorkOrderPhases = new List<WorkOrderPhase>();
                    workcenterDict.Add(currentWorkcenter.WorkcenterId, currentWorkcenter);
                }

                if (_operator != null && !currentWorkcenter.Operators.Any(o => o.OperatorId == _operator.OperatorId))
                {
                    currentWorkcenter.Operators.Add(_operator);
                }

                if (workOrderPhase != null && !currentWorkcenter.WorkOrderPhases.Any(wp => wp.PhaseId == workOrderPhase.PhaseId))
                {
                    currentWorkcenter.WorkOrderPhases.Add(workOrderPhase);
                }

                return currentWorkcenter;
            },
            splitOn: "OperatorId,PhaseId"
        );

        return result.Distinct().ToList();
    }


    public async Task<bool> KeepAliveWorkcenter(Workcenter workcenter, DateTime timestamp)
    {
        using var connection = _context.CreateConnection();
        var sql = """
            SET TIMEZONE='Europe/Madrid';
            UPDATE realtime."Workcenters"
            SET "CurrentTime" = @Timestamp
            WHERE "WorkcenterId" = @WorkcenterId;

            UPDATE data."WorkcenterShift"
            SET "EndTime" = @Timestamp
            WHERE "Id" = @WorkcenterDataId
            
            """;
        try
        {
            var affectedRows = await connection.ExecuteAsync(sql, 
                new { Timestamp = timestamp, 
                    WorkcenterId = workcenter.WorkcenterId, 
                    WorkcenterDataId = workcenter.WorkcenterDataId });
            if (affectedRows > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }catch(Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return false;
        }

    }

    public async Task<Workcenter?> GetWorkcenterById(Guid workcenterId)
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

    public async Task<int> CreateWorkcenterData(Workcenter workcenter)
    {
        using var connection = _context.CreateConnection();
        var sql = """
                SET TIMEZONE='Europe/Madrid';
                INSERT INTO data."WorkcenterShift"("Day", "WorkcenterId", "WorkcenterName", "AreaId", "AreaName", "ShiftId", "ShiftName", 
                                                   "ShiftDetailId", "IsProductiveTime", "StartTime", "EndTime")
                VALUES(@Day, @WorkcenterId, @WorkcenterName, @AreaId, @AreaName, @ShiftId, @ShiftName,
                        @ShiftDetailId, @IsProductiveTime, @StartTime, NOW() )
                RETURNING "Id";
            """;
        var result = await connection.QuerySingleAsync<int>(sql, new
        {
            Day = workcenter.CurrentDay,
            WorkcenterId = workcenter.WorkcenterId,
            WorkcenterName = workcenter.WorkcenterName,
            AreaId = workcenter.AreaId,
            AreaName = workcenter.AreaName,
            ShiftId = workcenter.ShiftId,
            ShiftName = workcenter.ShiftName,
            ShiftDetailId = workcenter.ShiftDetailId,
            IsProductiveTime = workcenter.IsProductiveTime,
            StartTime = workcenter.ShiftStartTime            
        });
        return result;
    }

    public async Task<bool> SetDataToWorkcenter(Workcenter workcenter)
    {
        using var connection = _context.CreateConnection();
        var sql = """
                SET TIMEZONE='Europe/Madrid';
                UPDATE realtime."Workcenters"
                SET "ShiftId" = @ShiftId, 
                    "ShiftName" = @ShiftName, 
                    "ShiftDetailId" = @ShiftDetailId, 
                    "ShiftStartTime" = @ShiftStartTime, 
                    "ShiftEndTime" = @ShiftEndTime,
                    "CurrentDay" = @CurrentDay,
                    "CurrentTime" = @CurrentTime,
                    "WorkcenterDataId" = @WorkcenterDataId
                WHERE "WorkcenterId" = @WorkcenterId
            """;
        try
        {                        
            var affectedRows = await connection.ExecuteAsync(sql, new
            {
                ShiftId = workcenter.ShiftId,
                ShiftName = workcenter.ShiftName,
                ShiftDetailId = workcenter.ShiftDetailId,
                ShiftStartTime = workcenter.ShiftStartTime,
                ShiftEndTime = workcenter.ShiftEndTime,
                CurrentDay = workcenter.CurrentDay,
                CurrentTime = workcenter.CurrentTime,
                WorkcenterDataId = workcenter.WorkcenterDataId,
                WorkcenterId = workcenter.WorkcenterId
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
        catch(Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return false;
        }
    }
}
