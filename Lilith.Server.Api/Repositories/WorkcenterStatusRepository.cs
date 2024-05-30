using Lilith.Server.Entities;
using Lilith.Server.Helpers.Database;

namespace Lilith.Server.Repositories;

public interface IWorkcenterStatusDataRepository
{
    Task<int> OpenWorkcenterStatus(int workcenterdataId, Guid statusId, DateTime currentTime);
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
            
        }catch (Exception ex)
        {
            return -1;
        }
    }

}
