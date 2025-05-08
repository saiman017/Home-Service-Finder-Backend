//using Azure;
//using Home_Service_Finder.Data.Contracts;
//using Home_Service_Finder.ServiceCategories.Contracts;
//using Home_Service_Finder.ServiceCategories.Dtos;
//using Home_Service_Finder.Users.Users;

//namespace Home_Service_Finder.ServiceCategories
//{
//    public class ServiceCategoryService : IServiceCategoryService
//    {

//        private readonly IUnitOfWork _db;

//        public ServiceCategoryService(IUnitOfWork unitOfWork)
//        {
//            _db = unitOfWork;

//        }
//        public async Task<APIResponse> AddServiceCategoryAsync(ServiceCategoryRequestDto dto)
//        {
//            // 1) check for duplicate name
//            var checkCategory = await _db.ServiceCategories.GetByServiceCategoryName(dto.Name);
//            if (checkCategory != null)
//                return ResponseHandler.GetBadRequestResponse("Service category name already exists");

//            // 2) if image was uploaded, validate & save
//            string? relativePath = null;
//            if (dto.CategoryImageFile != null && dto.CategoryImageFile.Length > 0)
//            {
//                var ext = Path.GetExtension(dto.CategoryImageFile.FileName).ToLowerInvariant();
//                var allowed = new[] { ".jpg", ".jpeg", ".png", ".gif" };
//                if (!allowed.Contains(ext))
//                    return ResponseHandler.GetBadRequestResponse("Only JPG/PNG/GIF files are allowed.");

//                var webRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
//                var uploadsDir = Path.Combine(webRoot, "uploads", "ServiceCategories");
//                Directory.CreateDirectory(uploadsDir);

//                var fileName = $"{Guid.NewGuid()}{ext}";
//                var physicalPath = Path.Combine(uploadsDir, fileName);
//                using (var stream = new FileStream(physicalPath, FileMode.Create))
//                {
//                    await dto.CategoryImageFile.CopyToAsync(stream);
//                }

//                relativePath = $"/uploads/ServiceCategories/{fileName}";
//            }

//            // 3) create the entity
//            var serviceCategory = new ServiceCategory
//            {
//                Name = dto.Name,
//                Description = dto.Description,
//                CategoryImage = relativePath,
//                CreatedAt = DateTime.UtcNow
//            };
//            await _db.ServiceCategories.AddAsync(serviceCategory);

//            // 4) persist
//            await _db.SaveChangesAsync();

//            // 5) build response DTO
//            var response = new ServiceCategoryResponseDto
//            {
//                Id = serviceCategory.Id,
//                Name = serviceCategory.Name,
//                Description = serviceCategory.Description,
//                CategoryImage = serviceCategory.CategoryImage,
//                CreatedAt = serviceCategory.CreatedAt
//            };

//            return ResponseHandler.GetSuccessResponse(response, "Service Category successfully added.");
//        }

//        public async Task<APIResponse> DeleteServiceCategoryAsync(Guid id)
//        {

//            var entity = await _db.ServiceCategories.GetByIdAsync(id);

//            if(entity == null)
//            {
//                return ResponseHandler.GetNotFoundResponse($"Service categpry of Id {id} not found");
//            }

//            entity.IsDeleted = true; // soft delete

//          entity =  _db.ServiceCategories.UpdateAsync(entity);



//           string result = await _db.SaveChangesAsync();


//           return ResponseHandler.GetSuccessResponse("Service Category Deleted Successfully");


//        }

//        public async Task<APIResponse> GetAllServiceCategoryAsync()
//        {

//            var serviceCategories = (await _db.ServiceCategories.GetAllAsync()).Where(u => !u.IsDeleted)
//                .ToList();
//            List<ServiceCategoryResponseDto> response = new List<ServiceCategoryResponseDto>();
//            foreach(var serviceCategory in serviceCategories)
//            {

//                response.Add(new ServiceCategoryResponseDto
//                {
//                    Id = serviceCategory.Id,
//                    Name = serviceCategory.Name,
//                    Description = serviceCategory.Description,
//                    CategoryImage = serviceCategory.CategoryImage,
//                    CreatedAt = serviceCategory.CreatedAt,
//                    ModifiedAt = serviceCategory.ModifiedAt
//                });

//            }
//           return ResponseHandler.GetSuccessResponse(response, "Service categories retrieved successfully");
//        }

//        public async Task<APIResponse> GetByIdServiceCategoryAsync(Guid id)
//        {
//            var serviceCategories = await _db.ServiceCategories.GetByIdAsync(id);
//            if (serviceCategories == null || serviceCategories.IsDeleted)
//            {
//                return ResponseHandler.GetNotFoundResponse("Service category not found");
//            }

//            ServiceCategoryResponseDto respone = new ServiceCategoryResponseDto()
//            {
//                Id = serviceCategories.Id,
//                Name = serviceCategories.Name,
//                Description = serviceCategories.Description,
//                CategoryImage = serviceCategories.CategoryImage,
//                CreatedAt = serviceCategories.CreatedAt,
//                ModifiedAt = serviceCategories.ModifiedAt
//            };

//            return ResponseHandler.GetSuccessResponse(respone);

//        }

//        public async Task<APIResponse> UpdateServiceCategoryAsync(Guid id, ServiceCategoryRequestDto serviceCategoryRequestDto)
//        {
//            var serviceCategories = await _db.ServiceCategories.GetByIdAsync(id);
//            if (serviceCategories == null || serviceCategories.IsDeleted)
//            {
//                return ResponseHandler.GetNotFoundResponse("Service category not found");
//            }

//            serviceCategories.Name = serviceCategoryRequestDto.Name;
//            serviceCategories.Description = serviceCategoryRequestDto.Description;
//            serviceCategories.CategoryImage = serviceCategoryRequestDto.CategoryImage;

//            if(serviceCategories.Name != serviceCategoryRequestDto.Name)
//            {
//                var checkCategoryName = await _db.ServiceCategories.GetByServiceCategoryName(serviceCategoryRequestDto.Name);
//                if (checkCategoryName != null)
//                {
//                    return ResponseHandler.GetBadRequestResponse("Service category name already exists");
//                }
//            }

//           serviceCategories =  _db.ServiceCategories.UpdateAsync(serviceCategories);

//           string result = await _db.SaveChangesAsync();

//            var respone = new ServiceCategoryResponseDto()
//            {
//                Id = serviceCategories.Id,
//                Name = serviceCategories.Name,
//                Description = serviceCategories.Description,
//                CategoryImage = serviceCategories.CategoryImage,
//                CreatedAt = serviceCategories.CreatedAt,
//                ModifiedAt = DateTime.UtcNow
//            };

//            return ResponseHandler.GetSuccessResponse(respone, "Service category Updated Successfully");





//        }

//        public async Task<APIResponse> UploadCategoryImageAsync(Guid categoryId, IFormFile file)
//        {
//            // 1) Validate file
//            if (file == null || file.Length == 0)
//                return ResponseHandler.GetBadRequestResponse("No file uploaded.");

//            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
//            var allowed = new[] { ".jpg", ".jpeg", ".png", ".gif" };
//            if (!allowed.Contains(ext))
//                return ResponseHandler.GetBadRequestResponse("Only JPG/PNG/GIF files are allowed.");

//            // 2) Fetch category
//            var category = await _db.ServiceCategories.GetByIdAsync(categoryId);
//            if (category == null || category.IsDeleted)
//                return ResponseHandler.GetNotFoundResponse("Service category not found.");

//            // 3) Prepare the uploads folder under wwwroot
//            var webRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
//            var uploadsDir = Path.Combine(webRoot, "uploads", "ServiceCategories");
//            Directory.CreateDirectory(uploadsDir);

//            // 4) Generate a unique filename
//            var fileName = $"{Guid.NewGuid()}{ext}";
//            var physicalPath = Path.Combine(uploadsDir, fileName);

//            // 5) Save the new file
//            using (var stream = new FileStream(physicalPath, FileMode.Create))
//                await file.CopyToAsync(stream);

//            // 6) Delete old image if it exists
//            if (!string.IsNullOrWhiteSpace(category.CategoryImage))
//            {
//                // CategoryImage is like "/uploads/ServiceCategories/oldname.png"
//                var oldRelative = category.CategoryImage.TrimStart('/');
//                var oldPhysical = Path.Combine(webRoot, oldRelative.Replace('/', Path.DirectorySeparatorChar));
//                if (System.IO.File.Exists(oldPhysical))
//                    System.IO.File.Delete(oldPhysical);
//            }

//            // 7) Update the database with the new relative URL
//            category.CategoryImage = $"/uploads/ServiceCategories/{fileName}";

//            // **Make sure to await here so EF Core tracks the change before SaveChangesAsync()**
//             _db.ServiceCategories.UpdateAsync(category);
//            await _db.SaveChangesAsync();

//            return ResponseHandler.GetSuccessResponse(category.CategoryImage, "Category image uploaded successfully");
//        }



//    }
//}
// ServiceCategoryService.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Home_Service_Finder.Data.Contracts;
using Home_Service_Finder.ServiceCategories.Contracts;
using Home_Service_Finder.ServiceCategories.Dtos;
using Microsoft.AspNetCore.Http;

namespace Home_Service_Finder.ServiceCategories
{
    public class ServiceCategoryService : IServiceCategoryService
    {
        private readonly IUnitOfWork _db;
        private static readonly string[] _allowedExt = { ".jpg", ".jpeg", ".png", ".gif" };

        public ServiceCategoryService(IUnitOfWork unitOfWork)
            => _db = unitOfWork;

        public async Task<APIResponse> AddServiceCategoryAsync(ServiceCategoryRequestDto dto)
        {
            // Duplicate-name check
            if (await _db.ServiceCategories.GetByServiceCategoryName(dto.Name) != null)
                return ResponseHandler.GetBadRequestResponse("Service category name already exists");

            // Save image if provided
            var imagePath = await SaveImageFileAsync(dto.CategoryImageFile);

            // Create entity
            var entity = new ServiceCategory
            {
                Name = dto.Name,
                Description = dto.Description,
                CategoryImage = imagePath,
                CreatedAt = DateTime.UtcNow
            };
            await _db.ServiceCategories.AddAsync(entity);
            await _db.SaveChangesAsync();

            // Map to response DTO
            var resp = new ServiceCategoryResponseDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                CategoryImage = entity.CategoryImage,
                CreatedAt = entity.CreatedAt
            };
            return ResponseHandler.GetSuccessResponse(resp, "Service category successfully added.");
        }

        public async Task<APIResponse> UpdateServiceCategoryAsync(Guid id, ServiceCategoryRequestDto dto)
        {
            var entity = await _db.ServiceCategories.GetByIdAsync(id);
            if (entity == null || entity.IsDeleted)
                return ResponseHandler.GetNotFoundResponse("Service category not found");

            // Name-change duplicate check
            if (!string.Equals(entity.Name, dto.Name, StringComparison.OrdinalIgnoreCase)
                && await _db.ServiceCategories.GetByServiceCategoryName(dto.Name) != null)
            {
                return ResponseHandler.GetBadRequestResponse("Service category name already exists");
            }

            // Update fields
            entity.Name = dto.Name;
            entity.Description = dto.Description;

            // Replace image if new file provided
            if (dto.CategoryImageFile != null && dto.CategoryImageFile.Length > 0)
            {
                // Delete old
                if (!string.IsNullOrWhiteSpace(entity.CategoryImage))
                {
                    var oldPhysical = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot",
                        entity.CategoryImage.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                    if (File.Exists(oldPhysical)) File.Delete(oldPhysical);
                }
                // Save new
                entity.CategoryImage = await SaveImageFileAsync(dto.CategoryImageFile);
            }

            _db.ServiceCategories.UpdateAsync(entity);
            await _db.SaveChangesAsync();

            var resp = new ServiceCategoryResponseDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                CategoryImage = entity.CategoryImage,
                CreatedAt = entity.CreatedAt,
                ModifiedAt = DateTime.UtcNow
            };
            return ResponseHandler.GetSuccessResponse(resp, "Service category updated successfully.");
        }

        public async Task<APIResponse> DeleteServiceCategoryAsync(Guid id)
        {
            var entity = await _db.ServiceCategories.GetByIdAsync(id);
            if (entity == null)
                return ResponseHandler.GetNotFoundResponse($"Service category with Id {id} not found");

            entity.IsDeleted = true;
            _db.ServiceCategories.UpdateAsync(entity);
            await _db.SaveChangesAsync();
            return ResponseHandler.GetSuccessResponse("Service category deleted successfully.");
        }

        public async Task<APIResponse> GetAllServiceCategoryAsync()
        {
            var list = (await _db.ServiceCategories.GetAllAsync())
                       .Where(x => !x.IsDeleted)
                       .ToList();

            var resp = list.Select(x => new ServiceCategoryResponseDto
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                CategoryImage = x.CategoryImage,
                CreatedAt = x.CreatedAt,
                ModifiedAt = x.ModifiedAt
            }).ToList();

            return ResponseHandler.GetSuccessResponse(resp, "Service categories retrieved successfully.");
        }

        public async Task<APIResponse> GetByIdServiceCategoryAsync(Guid id)
        {
            var x = await _db.ServiceCategories.GetByIdAsync(id);
            if (x == null || x.IsDeleted)
                return ResponseHandler.GetNotFoundResponse("Service category not found");

            var resp = new ServiceCategoryResponseDto
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                CategoryImage = x.CategoryImage,
                CreatedAt = x.CreatedAt,
                ModifiedAt = x.ModifiedAt
            };
            return ResponseHandler.GetSuccessResponse(resp);
        }

        // private helper
        private async Task<string?> SaveImageFileAsync(IFormFile? file)
        {
            if (file == null || file.Length == 0) return null;

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_allowedExt.Contains(ext))
                throw new InvalidOperationException("Only JPG/PNG/GIF files are allowed.");

            var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(),
                                          "wwwroot", "uploads", "ServiceCategories");
            Directory.CreateDirectory(uploadsDir);

            var fname = $"{Guid.NewGuid()}{ext}";
            var ppath = Path.Combine(uploadsDir, fname);
            using var stream = new FileStream(ppath, FileMode.Create);
            await file.CopyToAsync(stream);

            return $"/uploads/ServiceCategories/{fname}";
        }
    }
}

