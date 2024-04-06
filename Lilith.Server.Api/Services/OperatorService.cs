using Lilith.Server.Entities;
using Lilith.Server.Repositories;

namespace Lilith.Server.Services
{
    public interface IOperatorService
    {
        Task<bool> ClockIn(Guid OperatorId, Guid WorkcenterId);
        Task<bool> ClockOut(Guid OperatorId, Guid WorkcenterId);
    }

    public class OperatorService(IHistoricRowRespository historicRowRespository) : IOperatorService
    {
        private readonly IHistoricRowRespository _historicRowRespository = historicRowRespository;

        public async Task<bool> ClockIn(Guid OperatorId, Guid WorkcenterId)
        {
            await _historicRowRespository.Create(new HistoricRow
            {
                WorkcenterId = WorkcenterId,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now,
                Id = Guid.NewGuid()
            });

            return true;
        }

        public async Task<bool> ClockOut(Guid OperatorId, Guid WorkcenterId)
        {
            return true;
        }

    }
}