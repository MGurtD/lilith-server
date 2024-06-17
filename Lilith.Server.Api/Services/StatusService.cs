using Lilith.Server.Contracts.StatusResponse;
using Lilith.Server.Repositories;
using System.Text.Json;

namespace Lilith.Server.Services;

public interface IStatusService
{
    Task<StatusResponse?> GetStatusById(Guid statusId);    
    Task<bool> SetStatusToWorkcenter(Guid statusId, Guid workcenterId);
}
public class StatusService:IStatusService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IStatusRepository _statusRepository;
    private readonly IWorkcenterService _workcenterService;
    public StatusService(IHttpClientFactory httpClientFactory, IStatusRepository statusRepository, IWorkcenterService workcenterService)
    {
        _httpClientFactory = httpClientFactory;
        _statusRepository = statusRepository;
        _workcenterService = workcenterService;
    }
    public async Task<StatusResponse?> GetStatusById(Guid statusId)
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"https://localhost:7284/api/MachineStatus/{statusId}");
            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var status = JsonSerializer.Deserialize<StatusResponse>(responseString, options);
                return status;
            }
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return null;
        }
    }
    public async Task<bool> SetStatusToWorkcenter(Guid statusId, Guid workcenterId)
    {
        var status = await GetStatusById(statusId);
        if(status == null) { return false; }
        if(!await _statusRepository.SetStatusToWorkcenter(status, workcenterId))
        {
            return false;
        }
        await _workcenterService.LoadWorkcenterCache();
        return true;
    }
}
