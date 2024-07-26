using Lilith.Server.Entities;
using Lilith.Server.Repositories;

namespace Lilith.Server.Services
{
    public interface IWorkcenterService
    {
        Task LoadWorkcenterCache();
        Task<IEnumerable<Workcenter>> GetAllWorkcenters();
        Task<bool> KeepAliveWorkcenter(Workcenter workcenter, DateTime timestamp);
        Workcenter? GetWorkcenterById(Guid workcenterId);
        Task<int> CreateWorkcenterData(Workcenter workcenter);
        Task<bool> SetDataToWorkcenter(Workcenter workcenter);
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

        public async Task<bool> KeepAliveWorkcenter(Workcenter workcenter, DateTime timestamp)
        {
            return await _workcenterRepository.KeepAliveWorkcenter(workcenter, timestamp);
        }
        public Workcenter? GetWorkcenterById(Guid workcenterId)
        {
            var workcenter = WorkcenterCache.WorkcenterRT.FirstOrDefault(w => w.WorkcenterId == workcenterId);
            return workcenter;
        }

        public async Task<int> CreateWorkcenterData(Workcenter workcenter)
        {
            var id = await _workcenterRepository.CreateWorkcenterData(workcenter);
            return id;
        }

        public async Task<bool> SetDataToWorkcenter(Workcenter workcenter)
        {
            return await _workcenterRepository.SetDataToWorkcenter(workcenter);
        }

    
    }
}
