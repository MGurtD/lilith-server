using Lilith.Server.Entities;
using Lilith.Server.Repositories;

namespace Lilith.Server.Services
{
    public interface IWorkcenterService
    {
        Task LoadWorkcenterCache();
        Task<List<Workcenter>> GetAllWorkcenters();
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
            WorkcenterCache.WorkcenterRT = await _workcenterRepository.GetAllWorkcentersAsync();
        }

        public async Task<List<Workcenter>> GetAllWorkcenters()
        {
            return WorkcenterCache.WorkcenterRT;
        }
    }
}
