namespace Camping.Entities
{
    public class Booking
    {
        public int BookingID { get; set; }
        public int SpotID { get; set; }
        public int UserID { get; set; }
        public DateOnly BookingDate { get; set; }
        public int Duration { get; set; }
        public Booking() { }
    }
}
