using Lilith.Server.Entities;
using Lilith.Server.Repositories;

namespace Lilith.Server.Services
{
    public interface IWorkcenterService
    {
        Task LoadWorkcenterCache();
        Task<List<Workcenter>> GetAllWorkcenters();
        Task<bool> KeepAliveWorkcenter(Guid workcenterId, DateTime timestamp);
        Task<Workcenter> GetWorkcenterById(Guid workcenterId);
    }

    public class WorkcenterService : IWorkcenterService
    {
        private readonly IWorkcenterRepository _workcenterRepository;        
        public WorkcenterService(IWorkcenterRepository workcenterRepository)
        {
            _workcenterRepository = workcenterRepository;
        }

        public async Task LoadWorkcenterCache()
        {
            WorkcenterCache.WorkcenterRT = await _workcenterRepository.GetAllWorkcenters();
        }

        public async Task<List<Workcenter>> GetAllWorkcenters()
        {
            return WorkcenterCache.WorkcenterRT;
        }

        public async Task<bool> KeepAliveWorkcenter(Guid workcenterId, DateTime timestamp)
        {
            return await _workcenterRepository.KeepAliveWorkcenter(workcenterId, timestamp);
        }
        public Task<Workcenter>GetWorkcenterById(Guid workcenterId)
        {
            return _workcenterRepository.GetWorkcenterById(workcenterId);
        }
    }
}
