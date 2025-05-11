using Home_Service_Finder.Data.Contracts;
using Home_Service_Finder.Roles.Dtos;
using Home_Service_Finder.Roles;
using Home_Service_Finder.ServicesList.Contracts;
using Home_Service_Finder.ServicesList.Dtos;
using Home_Service_Finder.ServiceCategories;

namespace Home_Service_Finder.ServicesList
{
    public class ServiceListService : IServiceListService
    {
        private readonly IUnitOfWork _dbContext;

        public ServiceListService(IUnitOfWork unitOfWork)
        {
            _dbContext = unitOfWork;
        }
        public async Task<APIResponse> AddServiceList(ServiceListRequestDto serviceListRequestDto)
        {
            var entity = await _dbContext.ServiceLists.FindByNameAsync(serviceListRequestDto.Name);
            if (entity != null)
            {
                return ResponseHandler.GetBadRequestResponse($"Service of name {serviceListRequestDto.Name} already exists");
            }

            var serviceCategory = await _dbContext.ServiceCategories.GetByIdAsync(serviceListRequestDto.ServiceCategoryId);
            if(serviceCategory == null)
            {
                return ResponseHandler.GetNoContentResponse($"Service Category of id {serviceListRequestDto.ServiceCategoryId} not found");
            }
            ServiceList serviceList = new ServiceList()
            {
                Name = serviceListRequestDto.Name,
                ServiceCategoryId =serviceListRequestDto.ServiceCategoryId,
                CreatedAt = DateTime.UtcNow,
                //ModifiedAt = DateTime.UtcNow,


            };

            await _dbContext.ServiceLists.AddAsync(serviceList);

            string result = await _dbContext.SaveChangesAsync();

            ServiceListResponseDto response = new ServiceListResponseDto()
            {
                Id = serviceList.Id,
                Name = serviceList.Name,
                ServiceCategoryId = serviceList.ServiceCategoryId,
                CreatedAt = serviceList.CreatedAt,
                ModifiedAt = serviceList.ModifiedAt
                
            };
            return ResponseHandler.GetSuccessResponse(response, "Service Category  added successfully");
        }

        public async Task<APIResponse> DeleteServiceList(Guid id)
        {
            var serviceList = await _dbContext.ServiceLists.GetByIdAsync(id);
            if(serviceList == null)
            {
                return ResponseHandler.GetNoContentResponse($"Service list of Id {id} not found");
            }
            await _dbContext.ServiceLists.DeleteAsync(id);

            string result = await _dbContext.SaveChangesAsync();

            ServiceListResponseDto response = new ServiceListResponseDto()
            {
                Id = serviceList.Id,
                Name = serviceList.Name,
                ServiceCategoryId = serviceList.ServiceCategoryId,
                CreatedAt = serviceList.CreatedAt,
                ModifiedAt = serviceList.ModifiedAt

            };

            return ResponseHandler.GetSuccessResponse(response, "Service List deleted sucessfully");

        }

        public async Task<APIResponse> GetAllServiceList()
        {
            var serviceLists = await _dbContext.ServiceLists.GetAllAsync();
            List<ServiceListResponseDto> response = new List<ServiceListResponseDto>();
            foreach (var serviceList in serviceLists)
            {
                response.Add(new ServiceListResponseDto
                {
                    Id = serviceList.Id,
                    Name = serviceList.Name,
                    ServiceCategoryId = serviceList.ServiceCategoryId,
                    CreatedAt = serviceList.CreatedAt,
                    ModifiedAt = serviceList.ModifiedAt
                });
            }
            return ResponseHandler.GetSuccessResponse(response);
        }

        public async Task<APIResponse> GetServiceListById(Guid id)
        {
            var serviceList = await _dbContext.ServiceLists.GetByIdAsync(id);
            if (serviceList == null)
            {
                return ResponseHandler.GetNoContentResponse($"Service list of Id {id} not found.");
            }
            ServiceListResponseDto response = new ServiceListResponseDto()
            {
                Id = serviceList.Id,
                Name = serviceList.Name,
                ServiceCategoryId = serviceList.ServiceCategoryId,
                CreatedAt = serviceList.CreatedAt,
                ModifiedAt = serviceList.ModifiedAt
            };
            return ResponseHandler.GetSuccessResponse(response);
        }

        public async Task<APIResponse> UpdateServiceList(Guid id, ServiceListRequestDto serviceListRequestDto)
        {
            var serviceList = await _dbContext.ServiceLists.GetByIdAsync(id);
            if (serviceList == null)
            {
                return ResponseHandler.GetNotFoundResponse($"Service List of Id {id} not found");
            }

            if (serviceListRequestDto.Name.ToLower() != serviceList.Name.ToLower())
            {
                var checkServiceList = await _dbContext.ServiceLists.FindByNameAsync(serviceListRequestDto.Name);
                if (checkServiceList != null)
                {
                    return ResponseHandler.GetBadRequestResponse($"Service List of name {serviceListRequestDto.Name} already exists");
                }
            }

            serviceList.Name = serviceListRequestDto.Name;
            serviceList.ServiceCategoryId = serviceListRequestDto.ServiceCategoryId;
            serviceList.ModifiedAt = DateTime.UtcNow;

             _dbContext.ServiceLists.UpdateAsync(serviceList);
            string result = await _dbContext.SaveChangesAsync();

            ServiceListResponseDto response = new ServiceListResponseDto()
            {
                Id = serviceList.Id,
                Name = serviceList.Name,
                ServiceCategoryId = serviceList.ServiceCategoryId,
                CreatedAt = serviceList.CreatedAt,
                ModifiedAt = serviceList.ModifiedAt
            };

            return ResponseHandler.GetSuccessResponse(response, "Service List updated successfully");
        }

        public async Task<APIResponse> GetServiceListByCategoryId(Guid categoryId)
        {
            var serviceLists = await _dbContext.ServiceLists.GetServiceListByCategoryId(categoryId);

            if (!serviceLists.Any())
            {
                return ResponseHandler.GetNoContentResponse($"No services found for category ID {categoryId}");
            }

            var response = serviceLists.Select(serviceList => new ServiceListResponseDto
            {
                Id = serviceList.Id,
                Name = serviceList.Name,
                ServiceCategoryId = serviceList.ServiceCategoryId,
                CreatedAt = serviceList.CreatedAt,
                ModifiedAt = serviceList.ModifiedAt
            }).ToList();

            return ResponseHandler.GetSuccessResponse(response);
        }
    }
}
