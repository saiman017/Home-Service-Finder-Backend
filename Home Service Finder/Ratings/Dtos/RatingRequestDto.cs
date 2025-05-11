namespace Home_Service_Finder.Ratings.Dtos
{
    public class RatingRequestDto
    {
        public Guid CustomerId { get; set; }
        public Guid ServiceProviderId { get; set; }
        public int Value { get; set; }               
        public string? Comments { get; set; }
        public Guid? ServiceRequestId { get; set; }

    }
}
