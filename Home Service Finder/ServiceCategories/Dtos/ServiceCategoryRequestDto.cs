using System.ComponentModel.DataAnnotations;

namespace Home_Service_Finder.ServiceCategories.Dtos
{
    public class ServiceCategoryRequestDto
    {
        [Required]
        public string Name { get; set; }

        public string? Description { get; set; }

        /// <summary>
        /// Bound from multipart/form-data
        /// </summary>
        public IFormFile? CategoryImageFile { get; set; }


    }
}
