namespace Camping.Entities
{
    public class Campingspot
    {
        public int SpotID { get; set; }
        public int OwnerID { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Availability { get; set; }
        public Campingspot() { }
    }
}
