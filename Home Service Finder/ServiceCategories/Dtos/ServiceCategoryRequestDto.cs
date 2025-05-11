using System.ComponentModel.DataAnnotations;

namespace Home_Service_Finder.ServiceCategories.Dtos
{
    public class ServiceCategoryRequestDto
    {
        [Required]
        public string Name { get; set; }

        public string? Description { get; set; }
        public IFormFile? CategoryImageFile { get; set; }


    }
}
