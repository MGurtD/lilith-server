using Lilith.Server.Entities;
using Lilith.Server.Repositories;

namespace Lilith.Server.Services
{
    public interface IWorkcenterService
    {
        Task LoadWorkcenterCache();
        Task<IEnumerable<Workcenter>> GetAllWorkcenters();
        Task<bool> KeepAliveWorkcenter(Guid workcenterId, DateTime timestamp);
        Workcenter? GetWorkcenterById(Guid workcenterId);
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

        public async Task<IEnumerable<Workcenter>> GetAllWorkcenters()
        {
            return WorkcenterCache.WorkcenterRT;
        }

        public async Task<bool> KeepAliveWorkcenter(Guid workcenterId, DateTime timestamp)
        {
            return await _workcenterRepository.KeepAliveWorkcenter(workcenterId, timestamp);
        }
        public Workcenter? GetWorkcenterById(Guid workcenterId)
        {
            var workcenter = WorkcenterCache.WorkcenterRT.FirstOrDefault(w => w.WorkcenterId == workcenterId);
            return workcenter;
        }
    }
}
