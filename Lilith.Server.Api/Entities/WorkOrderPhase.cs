namespace Lilith.Server.Entities;

public class WorkOrderPhase
{
    public string WorkOrderCode {  get; set; }
    public string ReferenceCode { get; set; }
    public string ReferenceDescription { get; set; }
    public string PhaseCode { get; set; }
    public string PhaseDescription { get; set; }
    public DateTime PhaseStartTime { get; set; }
    public DateTime PhaseEndTime { get; set; }
    public int CounterOk { get; set; }
    public int CounterKo {  get; set; }
}
