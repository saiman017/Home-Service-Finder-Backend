namespace Home_Service_Finder.Ratings.Dtos
{
    public class RatingResponseDto
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public Guid ServiceProviderId { get; set; }
        public int Value { get; set; }
        public string? Comments { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
