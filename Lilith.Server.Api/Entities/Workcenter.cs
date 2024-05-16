namespace Lilith.Server.Entities
{
    public class Workcenter : Entity
    {
        public Guid WorkcenterId { get; set; }
        public string WorkcenterName { get; set; } = string.Empty;
        public string WorkcenterDescription { get; set; } = string.Empty;
        public Guid AreaId { get; set; }
        public string AreaName { get; set; } = string.Empty;
        public string AreaDescription { get; set; } = string.Empty;
        public DateTime CurrentDay { get; set; }
        public Guid? ShiftId { get; set; }
        public string ShiftName { get; set; } = string.Empty;
        public Guid ShiftDetailId { get; set; }
        public DateTime ShiftStartTime { get; set; }
        public DateTime ShiftEndTime { get; set; }
        public int WorkcenterDataId { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public string StatusDescription { get; set; } = string.Empty;
        public string StatusColor { get; set; } = string.Empty;
        public DateTime? StatusStartTime { get; set; }   
        public DateTime? StatusEndTime { get; set;}
        public string WorkOrderCode { get; set; } = string.Empty;
        public string ReferenceCode {  get; set; } = string.Empty;
        public string ReferenceDescription { get; set; } = string.Empty;
        public string PhaseCode { get; set; } = string.Empty;
        public string PhaseDescription { get; set; } = string.Empty;
        public DateTime? PhaseStartTime { get; set; }
        public DateTime? PhaseEndTime { get; set;}
        public decimal CounterOk { get; set; }
        public decimal CounterKo { get; set; }
    }
}
