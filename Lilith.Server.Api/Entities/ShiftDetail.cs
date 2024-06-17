namespace Lilith.Server.Entities
{
    public class ShiftDetail:Entity
    {
        public Guid ShiftId { get; set; }
        public Guid ShiftDetailId { get; set; }
        public String ShiftName { get; set; }
        public DateTime Day {  get; set; }
        public DateTime ShiftStartTime { get; set; }
        public DateTime ShiftEndTime { get; set; }
    }
}
