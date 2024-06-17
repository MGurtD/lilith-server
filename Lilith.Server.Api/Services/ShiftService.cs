using Lilith.Server.Entities;
using Lilith.Server.Repositories;

namespace Lilith.Server.Services
{
    public interface IShiftService
    {
        Task<ShiftDetail> GetCurrentShiftDetail(Guid shiftId, TimeOnly currentTime);
        Task<bool> SetShiftDetailToWorkcenter(ShiftDetail shiftdetail, Guid workcenterid);
    }
    public class ShiftService : IShiftService
    {
        private readonly IShiftRepository _shiftRepository;

        public ShiftService(IShiftRepository shiftRepository)
        {
            _shiftRepository = shiftRepository;
        }

        public async Task<ShiftDetail> GetCurrentShiftDetail(Guid shiftId, TimeOnly currentTime)
        {
            return await _shiftRepository.GetCurrentShiftDetail(shiftId, currentTime); 
        }

        public async Task<bool> SetShiftDetailToWorkcenter(ShiftDetail shiftdetail, Guid workcenterid)
        {
            return await _shiftRepository.SetShiftDetailToWorkcenter(shiftdetail, workcenterid);
        }
    }
}
