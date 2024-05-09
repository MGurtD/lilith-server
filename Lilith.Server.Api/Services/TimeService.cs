namespace Lilith.Server.Services
{
    public interface ITimeService
    {
        Task<bool> CheckShiftDetail(Guid ShiftDetailId, TimeOnly timestamp );
        Task<bool> ChangeShiftDetail(Guid WorkcenterId, Guid ShiftDetailId);
    }

    public class TimeService : ITimeService
    {
        private readonly ITimeService _timeService;

        public TimeService( ITimeService timeService )
        {
            _timeService = timeService;
        }

        public async Task<bool> CheckShiftDetail(Guid ShitDetailId, TimeOnly timestamp)
        {
            return true;
        }

        public async Task<bool> ChangeShiftDetail(Guid WorkcenterId, Guid ShiftDetailId)
        {
            return true;
        }
    }


}
