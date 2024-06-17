using Dapper;
using Lilith.Server.Contracts.Responses;
using Lilith.Server.Entities;
using Lilith.Server.Helpers.Database;

namespace Lilith.Server.Repositories;

public interface IWorkOrderPhaseRepository
{
    Task<bool> SetSetPhaseToWorkcenter(WorkOrderPhase workorderphase, Guid workcenterId);
}
public class WorkOrderPhaseRepository : IWorkOrderPhaseRepository
{
    private readonly DataContext _context;

    public WorkOrderPhaseRepository(DataContext context)
    {
        _context = context;
    }

    public async Task<bool> SetSetPhaseToWorkcenter(WorkOrderPhase workorderphase, Guid workcenterId)
    {
        using var connection = _context.CreateConnection();
        var sql = """
            SET TIMEZONE='Europe/Madrid';
            UPDATE realtime."Workcenters"
            SET "WorkOrderCode" = @WorkOrderCode,
                "ReferenceCode" = @ReferenceCode,
                "ReferenceDescription" = @ReferenceDescription,
                "PhaseCode" = @PhaseCode,
                "PhaseDescription" = @PhaseDescription,
                "PhaseStartTime" = NOW(),
                "PhaseEndTime" = NOW(),
                "CounterOk" = 0,
                "CounterKo" = 0
            WHERE "WorkcenterId" = @WorkcenterId 
            """;
        var affectedRows = await connection.ExecuteAsync(sql, new
        {
            WorkOrderCode = workorderphase.WorkOrderCode,
            ReferenceCode = workorderphase.ReferenceCode,
            ReferenceDescription = workorderphase.ReferenceDescription,
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
}
