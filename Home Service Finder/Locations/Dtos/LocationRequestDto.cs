namespace Home_Service_Finder.Locations.Dtos
{
    public class LocationRequestDto
    {

        public Guid UserId { get; set; }
        public string Address { get; set; }
        public string? City { get; set; }
        public string? PostalCode { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

    }
}
