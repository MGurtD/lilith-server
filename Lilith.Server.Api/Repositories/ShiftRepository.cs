using Dapper;
using Lilith.Server.Entities;
using Lilith.Server.Helpers.Database;

namespace Lilith.Server.Repositories;


    public interface IShiftRepository
    {
        Task<ShiftDetail>GetShiftDetail(Guid shiftId, TimeOnly currentTime);
    }

    public class ShiftRepository:IShiftRepository
    {
        private readonly DataContext _context;

        public ShiftRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<ShiftDetail> GetShiftDetail(Guid shiftId, TimeOnly currentTime)
        {
            using var connection = _context.CreateConnection();
            var sql = """
                    SET TIMEZONE='Europe/Madrid';
                    SELECT "Id" as ShiftDetailId, CURRENT_DATE::date AS "Day", CURRENT_DATE::date + "StartTime"::time AS "ShiftStartTime"
                    FROM public."ShiftDetails"
                    WHERE "ShiftId" = @ShiftId 
                    AND 
                    (
                    (@CurrentTime::TIME BETWEEN "StartTime" AND "EndTime")
                    OR
                    ("StartTime" > "EndTime" AND (@CurrentTime::TIME >= "StartTime" OR @CurrentTime::TIME <= "EndTime"))
                    );
                """;
            var result = await connection.QueryAsync<ShiftDetail>(sql, new { ShiftId = shiftId , CurrentTime = currentTime.ToString("HH:mm:ss") });
            return result.FirstOrDefault();
        }
    }

