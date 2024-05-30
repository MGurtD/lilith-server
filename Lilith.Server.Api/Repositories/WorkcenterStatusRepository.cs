using Lilith.Server.Entities;
using Lilith.Server.Helpers.Database;

namespace Lilith.Server.Repositories;

public interface IWorkcenterStatusDataRepository
{
    Task<int> OpenWorkcenterStatusData(int workcenterdataId, Guid statusId, DateTime currentTime);
    Task<bool> KeepAliveWorkcenterStatus(int workcenterdataId, DateTime currentTime);
    Task<bool> CloseWorkcenterStatus(int workcenterdataId, DateTime currentTime);
    Task<WorkcenterStatusData> GetCurrentWorkcenterStatus(int workcenterdataId);
}
public class WorkcenterStatusRepository : IWorkcenterStatusDataRepository
{
    private readonly DataContext _context;
    public WorkcenterStatusRepository(DataContext context)
    {
        _context = context;
    }
    public async Task<int> OpenWorkcenterStatusData(int workcenterdataId, Guid statusId, DateTime currentTime)
    {
        try
        {
            return 1;
        }catch (Exception ex)
        {
            return -1;
        }
    }
    public async Task<bool> KeepAliveWorkcenterStatus(int workcenterdataId, DateTime currentTime)
    {
        try
        {
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
    public async Task<bool> CloseWorkcenterStatus(int workcenterdataId, DateTime currentTime)
    {
        try
        {
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
    public async Task<WorkcenterStatusData> GetCurrentWorkcenterStatus(int workcenterdataId)
    {
        return null;
    }

}
