namespace Camping.Entities
{
    public class Review
    {
        public int ReviewID { get; set; }
        public int SpotID { get; set; }
        public int UserID  { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateOnly DatePosted { get; set; }

        public Review()
        {
            
        }
    }
}
