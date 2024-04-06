namespace Lilith.Server.Entities;

public class HistoricRow : Entity
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public Guid WorkcenterId { get; set; }
    public Guid ShiftId { get; set; }
}

