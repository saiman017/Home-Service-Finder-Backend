using Home_Service_Finder.ServiceCategories;

namespace Home_Service_Finder.ServicesList.Dtos
{
    public class ServiceListResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid ServiceCategoryId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; } 

    }
}
