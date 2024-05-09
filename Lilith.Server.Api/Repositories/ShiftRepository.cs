using Dapper;
using Lilith.Server.Entities;
using Lilith.Server.Helpers.Database;

namespace Lilith.Server.Repositories;


    public interface IShiftRepository
    {
        Task<ShiftDetail>GetShiftDetailId(Guid shiftId, TimeOnly currentTime);
    }

    public class ShiftRepository:IShiftRepository
    {
        private readonly DataContext _context;

        public ShiftRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<ShiftDetail> GetShiftDetailId(Guid shiftId, TimeOnly currentTime)
        {
            using var connection = _context.CreateConnection();
            var sql = """
                    SET TIMEZONE='Europe/Madrid';
                    SELECT "Id" as ShiftDetailId, CURRENT_DATE::date AS "Day", CURRENT_DATE::date + "StartTime"::time AS "ShiftStartTime"
                    FROM public."ShiftDetails"
                    WHERE ShiftId = @shiftId
                """;
            var result = await connection.QueryAsync<ShiftDetail>(sql, shiftId);
            return result.FirstOrDefault();
        }
    }

