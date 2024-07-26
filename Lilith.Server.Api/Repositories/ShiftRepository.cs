using Dapper;
using Lilith.Server.Entities;
using Lilith.Server.Helpers.Database;

namespace Lilith.Server.Repositories;


    public interface IShiftRepository
    {
        Task<ShiftDetail>GetCurrentShiftDetail(Guid shiftId, TimeOnly currentTime);
        Task<bool> SetShiftDetailToWorkcenter(ShiftDetail shiftdetail, Guid workcenterid);
        
    }

    public class ShiftRepository:IShiftRepository
    {
        private readonly DataContext _context;

        public ShiftRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<ShiftDetail> GetCurrentShiftDetail(Guid shiftId, TimeOnly currentTime)
        {
            using var connection = _context.CreateConnection();
            var sql = """
                    SET TIMEZONE='Europe/Madrid';
                    SELECT  s."Id" as "ShiftId", sd."Id" as ShiftDetailId,s."Name" as "ShiftName", 
                            CURRENT_DATE::date AS "Day", CURRENT_DATE::date + sd."StartTime"::time AS "ShiftStartTime",
                            CURRENT_DATE::date + sd."EndTime"::time AS "ShiftEndTime",
                            sd."IsProductiveTime"
                    FROM public."ShiftDetails" sd
                        INNER JOIN public."Shifts" s ON sd."ShiftId" = s."Id"
                    WHERE sd."ShiftId" = @ShiftId 
                    AND 
                    (
                    (@CurrentTime::TIME BETWEEN sd."StartTime" AND sd."EndTime")
                    OR
                    (sd."StartTime" > sd."EndTime" AND (@CurrentTime::TIME >= sd."StartTime" OR @CurrentTime::TIME <= sd."EndTime"))
                    );
                """;
            var result = await connection.QueryAsync<ShiftDetail>(sql, new { ShiftId = shiftId , CurrentTime = currentTime.ToString("HH:mm:ss") });
            return result.FirstOrDefault();
        }
        public async Task<bool> SetShiftDetailToWorkcenter(ShiftDetail shiftdetail, Guid workcenterid)
        {
        try
        {
            using var connection = _context.CreateConnection();
            var sql = """
                SET TIMEZONE='Europe/Madrid';
                UPDATE realtime."Workcenters"
                SET "ShiftId" = @ShiftId, 
                    "ShiftName" = @ShiftName, 
                    "ShiftDetailId" = @ShiftDetailId, 
                    "ShiftStartTime" = @ShiftStartTime, 
                    "ShiftEndTime" = @ShiftEndTime
                WHERE "WorkcenterId" = @WorkcenterId
            """;
            var affectedRows = await connection.ExecuteAsync(sql, new
            {
                ShiftId = shiftdetail.ShiftId,
                ShiftName = shiftdetail.ShiftName,
                ShiftDetailId = shiftdetail.ShiftDetailId,
                ShiftStartTime = shiftdetail.ShiftStartTime,
                ShiftEndTime = shiftdetail.ShiftEndTime,
                WorkcenterId = workcenterid
            });
            if (affectedRows > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return false;
        }

    }


}

