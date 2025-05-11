﻿namespace Home_Service_Finder.ServiceCategories.Dtos
{
    public class ServiceCategoryResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string CategoryImage { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }


    }
}
