namespace Home_Service_Finder.Ratings.Dtos
{
    public class RatingStatsDto
    {
        public Guid ServiceProviderId { get; set; }
        public int Count { get; set; }
        public int Sum { get; set; }
        public double Average { get; set; }
       
    }
}
