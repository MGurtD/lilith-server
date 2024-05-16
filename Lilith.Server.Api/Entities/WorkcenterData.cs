namespace Lilith.Server.Entities
{
    public class WorkcenterData
    {
        public int Id { get; set; }
        public DateTime Day { get; set; }
        public Guid WorkcenterId { get; set; }
        public string WorkcenterName { get; set; } = string.Empty;        
        public Guid AreaId { get; set; }
        public string AreaName { get; set; } = string.Empty;                
        public Guid? ShiftId { get; set; }
        public string ShiftName { get; set; } = string.Empty;
        public Guid ShiftDetailId { get; set; }
        public bool IsProductiveTime { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
