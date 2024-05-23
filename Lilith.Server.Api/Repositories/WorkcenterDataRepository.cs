using Dapper;
using Lilith.Server.Entities;
using Lilith.Server.Helpers.Database;

namespace Lilith.Server.Repositories;

//Open data
//KeepAlive data
//Close data

public interface IWorkcenterDataRepository
{
    Task<int> OpenWorkcenterData(Guid workcenterId);
    Task<bool> KeepAliveWorkcenterData(int id, DateTime currentTime);
    Task<bool> CloseWorkcenterData(int id, DateTime endTime);
    Task<WorkcenterData> GetWorkcenterData(int id);
}
public class WorkcenterDataRepository : IWorkcenterDataRepository
{
    private readonly DataContext _context;

    public WorkcenterDataRepository(DataContext context)
    {
        _context = context;
    }

    public async Task<int> OpenWorkcenterData(Guid workcenterId)
    {
        try
        {
            using var connection = _context.CreateConnection();
            var sql = """
            INSERT INTO data."WorkcenterShift"("Day","WorkcenterId", "WorkcenterName", "AreaId", "AreaName", "ShiftId","ShiftName", "ShiftDetailId","IsProductiveTime", "StartTime","EndTime")
            SELECT rwc."CurrentDay" as "Day", rwc."WorkcenterId", rwc."WorkcenterName", rwc."AreaId", rwc."AreaName", 
                    rwc."ShiftId",rwc."ShiftName", rwc."ShiftDetailId",sd."IsProductiveTime", rwc."ShiftStartTime" as "StartTime",rwc."ShiftEndTime" as "EndTime"
            FROM realtime."Workcenters" rwc
                INNER JOIN public."ShiftDetails" sd ON rwc."ShiftDetailId" = sd."Id"                
            WHERE "WorkcenterId" = @WorkcenterId
            RETURNING "Id";
            """;
            var id = connection.QuerySingle<int>(sql, new { WorkcenterId = workcenterId });
            return id;
        }catch(Exception ex)
        {
            return -1;
        }
        
    }
    public async Task<bool> KeepAliveWorkcenterData(int id, DateTime currentTime)
    {
        using var connection = _context.CreateConnection();
        var sql = """
            UPDATE data."WorkcenterShift"
            SET "EndTime" = @CurrentTime
            WHERE "Id" = @Id
            """;
        var affectedRows = await connection.ExecuteAsync(sql, new { CurrentTime = currentTime, Id = id });
        if (affectedRows > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public async Task<bool> CloseWorkcenterData(int Id, DateTime endTime)
    {
        return await KeepAliveWorkcenterData(Id, endTime);
    }

    public async Task<WorkcenterData> GetWorkcenterData(int id)
    {
        using var connection = _context.CreateConnection();
        var sql ="""

            """
    }
}
