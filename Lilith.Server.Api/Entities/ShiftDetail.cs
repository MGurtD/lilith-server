namespace Lilith.Server.Entities
{
    public class ShiftDetail:Entity
    {
        public Guid ShiftDetailId { get; set; }
        public DateTime Day {  get; set; }
        public DateTime ShiftStartTime { get; set; }
    }
}
