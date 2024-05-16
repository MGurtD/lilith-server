using Lilith.Server.Repositories;

namespace Lilith.Server.Services;

public interface IWorkcenterDataService
{
    Task<int> OpenWorkcenterData(Guid workcenterId);
    Task<bool> KeepAliveWorkcenterData(int id, DateTime currentTime);
    Task<bool> CloseWorkcenterData(int id, DateTime endTime);

}
public class WorkcenterDataService : IWorkcenterDataService
{
    private readonly IWorkcenterDataRepository _workcenterDataRepository;

    public WorkcenterDataService(IWorkcenterDataRepository workcenterDataRepository)
    {
        _workcenterDataRepository = workcenterDataRepository;
    }

    public async Task<int> OpenWorkcenterData(Guid workcenterId)
    {
        return await _workcenterDataRepository.OpenWorkcenterData(workcenterId); 
    }
    public async Task<bool> KeepAliveWorkcenterData(int id, DateTime currentTime)
    {
        return await _workcenterDataRepository.KeepAliveWorkcenterData(id, currentTime);
    }
    public async Task<bool> CloseWorkcenterData(int id, DateTime endTime)
    {
        return await _workcenterDataRepository.CloseWorkcenterData(id, endTime);
    }
}
