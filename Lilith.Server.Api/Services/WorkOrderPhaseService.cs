using Lilith.Server.Contracts.Responses;
using Lilith.Server.Entities;
using Lilith.Server.Repositories;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace Lilith.Server.Services;

public interface IWorkOrderPhaseService
{
    Task<WorkOrderPhaseResponse?> GetWorkOrderPhaseById(Guid workOrderPhaseId);
    Task<WorkOrderResponse?> GetWorkOrderById(Guid workOrderId);
    Task<bool> SetPhaseToWorkcenter(Guid workOrderPhaseId, Guid workcenterId);
    Task<bool> UnsetPhaseFromWorkcenter(Guid workOrderPhaseId, Guid workcenterId);
    Task<ReferenceResponse?> GetReferenceById(Guid referenceId);
}
public class WorkOrderPhaseService:IWorkOrderPhaseService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IWorkOrderPhaseRepository _workOrderPhaseRepository;
    private readonly IWorkcenterService _workcenterService;
    private readonly IConfiguration _configuration;
    private readonly string _apiUrl;

    public WorkOrderPhaseService(IHttpClientFactory httpClientFactory, IWorkOrderPhaseRepository workOrderPhaseRepository, IWorkcenterService workcenterService, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _workOrderPhaseRepository = workOrderPhaseRepository;
        _workcenterService = workcenterService;
        _configuration = configuration;
        _apiUrl = _configuration["ExternalConnections:Default"] ?? throw new ArgumentNullException("ApiUrls");

    }

    public async Task<WorkOrderPhaseResponse?> GetWorkOrderPhaseById(Guid workOrderPhaseId)
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_apiUrl}/WorkOrder/Phase/{workOrderPhaseId}");
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
            var response = await client.GetAsync($"{_apiUrl}/WorkOrder/{workOrderId}");
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
            var response = await client.GetAsync($"{_apiUrl}/Reference/{referenceId}");
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

    private async Task<WorkOrderPhase>CreatePhase(Guid workOrderPhaseId)
    {
        var wo = new WorkOrderPhase { };
        var phase = await GetWorkOrderPhaseById(workOrderPhaseId);
        if (phase == null) { return wo; }
        var workorder = await GetWorkOrderById(phase.WorkOrderId);
        if (workorder == null) { return wo; }
        var reference = await GetReferenceById(workorder.ReferenceId);
        if (reference == null) { return wo; }
        wo = new WorkOrderPhase
        {
            WorkOrderCode = workorder.Code,
            ReferenceCode = reference.Code,
            ReferenceDescription = reference.Description,
            PhaseId = workOrderPhaseId,
            PhaseCode = phase.Code,
            PhaseDescription = phase.Description
        };
        return wo;
    }
    public async Task<bool> SetPhaseToWorkcenter(Guid workOrderPhaseId, Guid workcenterId)
    {
        var wo = await CreatePhase(workOrderPhaseId);
        if(!await _workOrderPhaseRepository.SetPhaseToWorkcenter(wo, workcenterId))
        {
            return false;
        }
        await _workcenterService.LoadWorkcenterCache();
        return true;
    }
    public async Task<bool> UnsetPhaseFromWorkcenter(Guid workOrderPhaseId, Guid workcenterId)
    {
        var wo = await CreatePhase(workOrderPhaseId);
        if (!await _workOrderPhaseRepository.UnsetPhaseFromWorkcenter(wo, workcenterId))
        {
            return false;
        }
        await _workcenterService.LoadWorkcenterCache();
        return true;
    }
}
