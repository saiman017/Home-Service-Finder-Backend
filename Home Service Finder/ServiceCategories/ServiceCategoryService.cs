using Azure;
using Home_Service_Finder.Data.Contracts;
using Home_Service_Finder.ServiceCategories.Contracts;
using Home_Service_Finder.ServiceCategories.Dtos;
using Home_Service_Finder.Users.Users;

namespace Home_Service_Finder.ServiceCategories
{
    public class ServiceCategoryService : IServiceCategoryService
    {

        private readonly IUnitOfWork _db;

        public ServiceCategoryService(IUnitOfWork unitOfWork)
        {
            _db = unitOfWork;
            
        }
        public async Task<APIResponse> AddServiceCategoryAsync(ServiceCategoryRequestDto serviceCategoryRequestDto)
        {
            var checkCategoryName = await _db.ServiceCategories.GetByServiceCategoryName(serviceCategoryRequestDto.Name);
            if(checkCategoryName != null)
            {
                return ResponseHandler.GetBadRequestResponse("Service category name already exists");
            }

            ServiceCategory serviceCategory = new ServiceCategory()
            {
                Name = serviceCategoryRequestDto.Name,
                Description = serviceCategoryRequestDto.Description,
                CategoryImage = serviceCategoryRequestDto.CategoryImage,
                CreatedAt = DateTime.UtcNow,
            };

            await _db.ServiceCategories.AddAsync(serviceCategory);

            ServiceCategoryResponseDto response = new ServiceCategoryResponseDto()
            {
                Id = serviceCategory.Id,
                Name = serviceCategory.Name,
                Description = serviceCategory.Description,
                CategoryImage = serviceCategory.CategoryImage,
                CreatedAt = serviceCategory.CreatedAt
                
            };

            string result = await _db.SaveChangesAsync();

            return ResponseHandler.GetSuccessResponse(response, "Service Category successfully added.");
        }

        public async Task<APIResponse> DeleteServiceCategoryAsync(Guid id)
        {
            
            var entity = await _db.ServiceCategories.GetByIdAsync(id);

            if(entity == null)
            {
                return ResponseHandler.GetNotFoundResponse($"Service categpry of Id {id} not found");
            }

            entity.IsDeleted = true; // soft delete

          entity =  _db.ServiceCategories.UpdateAsync(entity);



           string result = await _db.SaveChangesAsync();


           return ResponseHandler.GetSuccessResponse("Service Category Deleted Successfully");

            
        }

        public async Task<APIResponse> GetAllServiceCategoryAsync()
        {
            
            var serviceCategories = (await _db.ServiceCategories.GetAllAsync()).Where(u => !u.IsDeleted)
                .ToList();
            List<ServiceCategoryResponseDto> response = new List<ServiceCategoryResponseDto>();
            foreach(var serviceCategory in serviceCategories)
            {

                response.Add(new ServiceCategoryResponseDto
                {
                    Id = serviceCategory.Id,
                    Name = serviceCategory.Name,
                    Description = serviceCategory.Description,
                    CategoryImage = serviceCategory.CategoryImage,
                    CreatedAt = serviceCategory.CreatedAt,
                    ModifiedAt = serviceCategory.ModifiedAt
                });

            }
           return ResponseHandler.GetSuccessResponse(response, "Service categories retrieved successfully");
        }

        public async Task<APIResponse> GetByIdServiceCategoryAsync(Guid id)
        {
            var serviceCategories = await _db.ServiceCategories.GetByIdAsync(id);
            if (serviceCategories == null || serviceCategories.IsDeleted)
            {
                return ResponseHandler.GetNotFoundResponse("Service category not found");
            }

            ServiceCategoryResponseDto respone = new ServiceCategoryResponseDto()
            {
                Id = serviceCategories.Id,
                Name = serviceCategories.Name,
                Description = serviceCategories.Description,
                CategoryImage = serviceCategories.CategoryImage,
                CreatedAt = serviceCategories.CreatedAt,
                ModifiedAt = serviceCategories.ModifiedAt
            };

            return ResponseHandler.GetSuccessResponse(respone);

        }

        public async Task<APIResponse> UpdateServiceCategoryAsync(Guid id, ServiceCategoryRequestDto serviceCategoryRequestDto)
        {
            var serviceCategories = await _db.ServiceCategories.GetByIdAsync(id);
            if (serviceCategories == null || serviceCategories.IsDeleted)
            {
                return ResponseHandler.GetNotFoundResponse("Service category not found");
            }

            serviceCategories.Name = serviceCategoryRequestDto.Name;
            serviceCategories.Description = serviceCategoryRequestDto.Description;
            serviceCategories.CategoryImage = serviceCategoryRequestDto.CategoryImage;

            if(serviceCategories.Name != serviceCategoryRequestDto.Name)
            {
                var checkCategoryName = await _db.ServiceCategories.GetByServiceCategoryName(serviceCategoryRequestDto.Name);
                if (checkCategoryName != null)
                {
                    return ResponseHandler.GetBadRequestResponse("Service category name already exists");
                }
            }

           serviceCategories =  _db.ServiceCategories.UpdateAsync(serviceCategories);

           string result = await _db.SaveChangesAsync();

            var respone = new ServiceCategoryResponseDto()
            {
                Id = serviceCategories.Id,
                Name = serviceCategories.Name,
                Description = serviceCategories.Description,
                CategoryImage = serviceCategories.CategoryImage,
                CreatedAt = serviceCategories.CreatedAt,
                ModifiedAt = DateTime.UtcNow
            };

            return ResponseHandler.GetSuccessResponse(respone, "Service category Updated Successfully");





        }
    }
}
