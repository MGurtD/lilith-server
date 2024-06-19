using Dapper;
using Lilith.Server.Contracts.Responses;
using Lilith.Server.Entities;
using Lilith.Server.Helpers.Database;

namespace Lilith.Server.Repositories;

public interface IWorkOrderPhaseRepository
{
    Task<bool> SetPhaseToWorkcenter(WorkOrderPhase workorderphase, Guid workcenterId);
    Task<bool> UnsetPhaseFromWorkcenter(WorkOrderPhase workorderphase, Guid workcenterId);
}
public class WorkOrderPhaseRepository : IWorkOrderPhaseRepository
{
    private readonly DataContext _context;

    public WorkOrderPhaseRepository(DataContext context)
    {
        _context = context;
    }

    public async Task<bool> SetPhaseToWorkcenter(WorkOrderPhase workorderphase, Guid workcenterId)
    {
        using var connection = _context.CreateConnection();
        var sql = """
            SET TIMEZONE='Europe/Madrid';
            INSERT INTO realtime."Workcenters"("WorkcenterId", "WorkOrderCode", "ReferenceCode", "ReferenceDescription", 
                                                "PhaseId", "PhaseCode", "PhaseDescription", "PhaseStartTime", "PhaseEndTime",
                                                "CounterOk", "CounterKo")
            VALUES(@WorkcenterId, @WorkOrderCode, @ReferenceCode, @ReferenceDescription,
                    @PhaseId, @PhaseCode, @PhaseDescription, NOW(), NOW(), 0, 0)
            """;
        var affectedRows = await connection.ExecuteAsync(sql, new
        {
            WorkOrderCode = workorderphase.WorkOrderCode,
            ReferenceCode = workorderphase.ReferenceCode,
            ReferenceDescription = workorderphase.ReferenceDescription,
            PhaseId = workorderphase.PhaseId,
            PhaseCode = workorderphase.PhaseCode,
            PhaseDescription = workorderphase.PhaseDescription,
            workcenterId = workcenterId
        }) ;
        if (affectedRows > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public async Task<bool> UnsetPhaseFromWorkcenter(WorkOrderPhase workorderphase, Guid workcenterId)
    {
        using var connection = _context.CreateConnection();
        var sql = """
            DELETE FROM realtime."Workcenters" WHERE "WorkcenterId" = @WorkcenterId AND "PhaseId" = @PhaseId
            """;
        var affectedRows = await connection.ExecuteAsync(sql, new
        {
            workcenterId = workcenterId,
            PhaseId = workorderphase.PhaseId

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
