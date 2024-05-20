using Lilith.Server.Entities;
using Lilith.Server.Repositories;

namespace Lilith.Server.Services
{
    public interface IShiftService
    {
        Task<ShiftDetail> GetShiftDetail(Guid shiftId, TimeOnly currentTime);
    }
    public class ShiftService : IShiftService
    {
        private readonly IShiftRepository _shiftRepository;

        public ShiftService(IShiftRepository shiftRepository)
        {
            _shiftRepository = shiftRepository;
        }

        public async Task<ShiftDetail> GetShiftDetail(Guid shiftId, TimeOnly currentTime)
        {
            return await _shiftRepository.GetShiftDetail(shiftId, currentTime); 
        }
    }
}
