namespace Lilith.Server.Entities
{
    public class WorkcenterStatusData
    {
        public int Id { get; set; }
        public int WorkcenterDataId {  get; set; }
        public Guid StatusId { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public string StatusDescription { get; set; } = string.Empty;
        public string StatusColor { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public decimal WorkcenterStatusHourlyCost { get; set; } = decimal.Zero;
    }
}
