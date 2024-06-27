using Lilith.Server.Contracts.Responses;
using Lilith.Server.Entities;
using Lilith.Server.Repositories;
using Microsoft.OpenApi.Models;
using System.Text.Json;

namespace Lilith.Server.Services;

public interface IOperatorService
{
    Task<bool> SetOperatorToWorkcenter(Guid operatorId, Guid workcenterId);
    Task<bool> UnsetOperatorFromWorkcenter(Guid operatorId, Guid workcenterId);
}

public class OperatorService : IOperatorService
{
    private readonly IOperatorRepository _operatorRepository;

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IWorkcenterService _workcenterService;
    private readonly IConfiguration _configuration;
    private readonly string _apiUrl;

    public OperatorService(IOperatorRepository operatorRepository, IHttpClientFactory httpClientFactory, IWorkcenterService workcenterService, IConfiguration configuration)
    {
        _operatorRepository = operatorRepository;
        _httpClientFactory = httpClientFactory;
        _workcenterService = workcenterService;
        _configuration = configuration;
        _apiUrl = _configuration["ExternalConnections:Default"] ?? throw new ArgumentNullException("ApiUrls");
    }

    public async Task<OperatorResponse?> GetOperatorById(Guid operatorId)
    {
        try
        {
            var client = _httpClientFactory.CreateClient();            
            var response = await client.GetAsync($"{_apiUrl}/Operator/{operatorId}");
            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var result = JsonSerializer.Deserialize<OperatorResponse>(responseString, options);
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

    public async Task<OperatorTypeResponse?> GetOperatorTypeById(Guid operatortypeId)
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_apiUrl}/OperatorType/{operatortypeId}");
            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var result = JsonSerializer.Deserialize<OperatorTypeResponse>(responseString, options);
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

    private async Task<Operator>CreateOperator(Guid operatorId)
    {
        var _operator = new Operator { };
        var operatorResponse = await GetOperatorById(operatorId);
        if (operatorResponse == null) { return _operator; }
        var operatorTypeResponse = await GetOperatorTypeById(operatorResponse.OperatorTypeId);
        if (operatorTypeResponse == null) { return _operator; }
        _operator = new Operator
        {
            OperatorId = operatorResponse.Id,
            OperatorCode = operatorResponse.Code,
            OperatorName = operatorResponse.Surname + ", " + operatorResponse.Name,
            OperatorTypeName = operatorTypeResponse.Code,
            OperatorTypeDescription = operatorTypeResponse.Description
        };
        return _operator;
    }

    public async Task<bool> SetOperatorToWorkcenter(Guid operatorId, Guid workcenterId)
    {
        var _operator = await CreateOperator(operatorId);
        if(! await _operatorRepository.SetOperatorToWorkcenter(_operator, workcenterId))
        {
            return false;
        }
        await _workcenterService.LoadWorkcenterCache();
        return true;
    }

    public async Task<bool> UnsetOperatorFromWorkcenter(Guid operatorId, Guid workcenterId)
    {
        var _operator = await CreateOperator(operatorId);
        if (!await _operatorRepository.UnsetOperatorFromWorkcenter(_operator, workcenterId))
        {
            return false;
        }
        await _workcenterService.LoadWorkcenterCache();
        return true;
    }

}