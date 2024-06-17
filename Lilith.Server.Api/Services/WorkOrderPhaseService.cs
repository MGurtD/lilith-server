using Lilith.Server.Contracts.Responses;
using Lilith.Server.Entities;
using Lilith.Server.Repositories;
using System.Text.Json;

namespace Lilith.Server.Services;

public interface IWorkOrderPhaseService
{
    Task<WorkOrderPhaseResponse?> GetWorkOrderPhaseById(Guid workOrderPhaseId);
    Task<bool> SetPhaseToWorkcenter(Guid workOrderPhaseId, Guid workcenterId);
}
public class WorkOrderPhaseService:IWorkOrderPhaseService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IWorkOrderPhaseRepository _workOrderPhaseRepository;
    private readonly IWorkcenterService _workcenterService;

    public WorkOrderPhaseService(IHttpClientFactory httpClientFactory, IWorkOrderPhaseRepository workOrderPhaseRepository, IWorkcenterService workcenterService)
    {
        _httpClientFactory = httpClientFactory;
        _workOrderPhaseRepository = workOrderPhaseRepository;
        _workcenterService = workcenterService;

    }

    public async Task<WorkOrderPhaseResponse?> GetWorkOrderPhaseById(Guid workOrderPhaseId)
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"https://localhost:7284/api/WorkOrder/Phase/{workOrderPhaseId}");
            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var result = JsonSerializer.Deserialize<WorkOrderPhaseResponse>(responseString, options);                
                return result;
            }
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return null;
        }
    }

    public async Task<WorkOrderResponse?> GetWorkOrderById(Guid workOrderId)
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"https://localhost:7284/api/WorkOrder/{workOrderId}");
            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var result = JsonSerializer.Deserialize<WorkOrderResponse>(responseString, options);
                return result;
            }
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return null;
        }
    }

    public async Task<ReferenceResponse?> GetReferenceById(Guid referenceId)
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"https://localhost:7284/api/Reference/{referenceId}");
            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var result = JsonSerializer.Deserialize<ReferenceResponse>(responseString, options);
                return result;
            }
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return null;
        }
    }

    public async Task<bool> SetPhaseToWorkcenter(Guid workOrderPhaseId, Guid workcenterId)
    {
        var phase = await GetWorkOrderPhaseById(workOrderPhaseId);
        if (phase == null) { return false; }
        var workorder = await GetWorkOrderById(phase.WorkOrderId);
        if (workorder == null) { return false; }
        var reference = await GetReferenceById(workorder.ReferenceId);
        if (reference == null) { return false; }
        var wo = new WorkOrderPhase
        {
            WorkOrderCode = workorder.Code,
            ReferenceCode = reference.Code,
            ReferenceDescription = reference.Description,
            PhaseCode = phase.Code,
            PhaseDescription = phase.Description
        };
        if(!await _workOrderPhaseRepository.SetSetPhaseToWorkcenter(wo, workcenterId))
        {
            return false;
        }
        await _workcenterService.LoadWorkcenterCache();
        return true;
    }
}
