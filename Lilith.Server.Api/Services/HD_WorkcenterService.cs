using Lilith.Server.Entities;
using Lilith.Server.Repositories;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Lilith.Server.Services;

public interface IWorkcenterDataService
{
    Task<int> OpenWorkcenterData(Guid workcenterId);
    Task<bool> KeepAliveWorkcenterData(int id, DateTime currentTime);
    Task<bool> CloseWorkcenterData(int id, DateTime endTime);
    Task<HD_Workcenter> GetWorkcenterData(int id);

}
public class HD_WorkcenterService : IWorkcenterDataService
{
    private readonly IWorkcenterDataRepository _workcenterDataRepository;

    public HD_WorkcenterService(IWorkcenterDataRepository workcenterDataRepository)
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
    public async Task<HD_Workcenter> GetWorkcenterData(int id)
    {
        return await _workcenterDataRepository.GetWorkcenterData(id);
    }
}
