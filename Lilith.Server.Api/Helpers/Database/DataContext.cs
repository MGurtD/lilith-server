namespace Lilith.Server.Helpers.Database;

using System.Data;
using Dapper;
using Npgsql;

public class DataContext
{
    private string _connectionString;

    public DataContext()
    {
        var configurationLoader = new ConfigurationLoader();
        IConfiguration configuration = configurationLoader.LoadConfiguration();

        _connectionString = configuration["ConnectionStrings:Default"] ?? "";
    }

    public IDbConnection CreateConnection()
    {
        return new NpgsqlConnection(_connectionString);
    }

    public async Task Init()
    {
        await _initSchemas();
        await _initTables();
    }

    private async Task _initSchemas()
    {
        // create schema if it doesn't exist
        using var connection = CreateConnection();

        var sql = $"CREATE SCHEMA IF NOT EXISTS  \"realtime\"";
        await connection.ExecuteAsync(sql);

        sql = $"CREATE SCHEMA IF NOT EXISTS  \"data\"";
        await connection.ExecuteAsync(sql);
    }

    private async Task _initTables()
    {
        // create tables if they don't exist
        using var connection = CreateConnection();
        await _initDataModel(connection);
        await _initRealtimeModel(connection);
    }

    private async Task _initDataModel(IDbConnection connection)
    {
        
        var sql = """
            CREATE TABLE IF NOT EXISTS data."WorkcenterShift" (
            "Id" int GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
            "Day" date NOT NULL,
            "WorkcenterId" uuid NOT NULL,
            "WorkcenterName" varchar(50) NOT NULL,            
            "AreaId" uuid NOT NULL,
            "AreaName" varchar(50) NOT NULL,
            "ShiftId" uuid NOT NULL,
            "ShiftName" varchar(50) NOT NULL,
            "ShiftDetailId" uuid NOT NULL,
            "IsProductiveTime" boolean,
            "StartTime" timestamp without time zone NOT NULL,
            "EndTime" timestamp without time zone NOT NULL
            );
            """;
        await connection.ExecuteAsync(sql);

        sql = """
            CREATE TABLE IF NOT EXISTS data."WorkcenterStatus" (
            "Id" int GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
            "WorkcenterDataId" int,
            "StatusId" uuid NOT NULL,
            "StatusName" varchar(50),
            "StatusDescription" varchar(250),
            "StatusColor" varchar(20),
            "StartTime" timestamp without time zone,   
            "EndTime" timestamp without time zone,
            "WorkcenterStatusHourlyCost" decimal(10,2)
            );
            """;
        await connection.ExecuteAsync(sql);
    }

    private async Task _initRealtimeModel(IDbConnection connection)
    {
        var sql = """
            CREATE TABLE IF NOT EXISTS realtime."Workcenters" (
            "WorkcenterId" uuid NOT NULL PRIMARY KEY,
            "WorkcenterName" varchar(50) NOT NULL,
            "WorkcenterDescription" varchar(250) NOT NULL,
            "AreaId" uuid NOT NULL,
            "AreaName" varchar(50) NOT NULL,
            "AreaDescription" varchar(250) NOT NULL,
            "CurrentDay" date NOT NULL,
            "ShiftId" uuid NOT NULL,
            "ShiftName" varchar(50) NOT NULL,  
            "ShiftDetailId" uuid,
            "ShiftStartTime" timestamp without time zone,
            "ShiftEndTime" timestamp without time zone,
            "WorkcenterDataId" int,
            "StatusName" varchar(50),
            "StatusDescription" varchar(250),
            "StatusColor" varchar(20),
            "StatusStartTime" timestamp without time zone,   
            "StatusEndTime" timestamp without time zone,
            "WorkOrderCode" varchar(50),
            "ReferenceCode" varchar(50),
            "ReferenceDescription" varchar(250),
            "PhaseCode" varchar(50),
            "PhaseDescription" varchar(250),
            "PhaseStartTime" timestamp without time zone,
            "PhaseEndTime" timestamp without time zone,
            "CounterOk" decimal(15,5),
            "CounterKo" decimal(15,5)
            );
            """;
        await connection.ExecuteAsync(sql);
        //Actualitzar schema realtime
        sql = """
            SET TIMEZONE='Europe/Madrid';
            INSERT INTO realtime."Workcenters"("WorkcenterId","WorkcenterName", "WorkcenterDescription", 
                                                "AreaId", "AreaName", "AreaDescription",
                                                "CurrentDay","ShiftId", "ShiftName", "ShiftDetailId", "ShiftStartTime", "ShiftEndTime")
            SELECT wc."Id" as "WorkcenterId", wc."Name" as "WorkcenterName", wc."Description" as "WorkcenterDescription",
                   ar."Id" as "AreaId", ar."Name" as "AreaName", ar."Description" as "AreaDescription",
                   CURRENT_DATE as "Day", sh."Id" as "ShiftId", sh."Name" as "ShiftName", sd."Id" as "ShiftDetailId", NOW() as "ShiftStartTime", NOW() as "ShiftEndTime"
            FROM public."Workcenters" wc
            INNER JOIN public."Areas" ar ON wc."AreaId" = ar."Id"
            INNER JOIN public."Shifts" sh ON wc."ShiftId" = sh."Id"
            INNER JOIN public."ShiftDetails" sd ON sh."Id" = sd."ShiftId" AND CURRENT_TIME BETWEEN sd."StartTime" AND sd."EndTime"
            WHERE wc."Disabled" = false
            ON CONFLICT("WorkcenterId")
            DO NOTHING;
            DELETE FROM realtime."Workcenters" WHERE "WorkcenterId" IN
            (SELECT "Id" FROM public."Workcenters" WHERE "Disabled" = true);
            """;
        await connection.ExecuteAsync(sql);

        //Actualitzar schema data
    }
}