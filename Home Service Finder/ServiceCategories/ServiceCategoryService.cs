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
            var existing = await _db.ServiceCategories
                                    .FindByNameIncludingDeletedAsync(dto.Name);

            if (existing != null)
            {
                if (!existing.IsDeleted)
                {
                    return ResponseHandler.GetBadRequestResponse("Service category name already exists");
                }
                existing.IsDeleted = false;
                existing.Description = dto.Description;
                existing.ModifiedAt = DateTime.UtcNow;
                if (dto.CategoryImageFile != null && dto.CategoryImageFile.Length > 0)
                {

                    if (!string.IsNullOrWhiteSpace(existing.CategoryImage))
                    {
                        var oldPhysical = Path.Combine(
                            Directory.GetCurrentDirectory(),
                            "wwwroot",
                            existing.CategoryImage.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                        if (File.Exists(oldPhysical))
                            File.Delete(oldPhysical);
                    }
                    existing.CategoryImage = await SaveImageFileAsync(dto.CategoryImageFile);
                }

                _db.ServiceCategories.UpdateAsync(existing);
                await _db.SaveChangesAsync();

                var resp = new ServiceCategoryResponseDto
                {
                    Id = existing.Id,
                    Name = existing.Name,
                    Description = existing.Description,
                    CategoryImage = existing.CategoryImage,
                    CreatedAt = existing.CreatedAt,
                    ModifiedAt = existing.ModifiedAt
                };
                return ResponseHandler.GetSuccessResponse(resp, "Service category restored successfully.");
            }

            // No existing (even soft-deleted) → create new
            var imagePath = await SaveImageFileAsync(dto.CategoryImageFile);

            var entity = new ServiceCategory
            {
                Name = dto.Name,
                Description = dto.Description,
                CategoryImage = imagePath,
                CreatedAt = DateTime.UtcNow,
            };

            await _db.ServiceCategories.AddAsync(entity);
            await _db.SaveChangesAsync();

            var newResp = new ServiceCategoryResponseDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                CategoryImage = entity.CategoryImage,
                CreatedAt = entity.CreatedAt
            };
            return ResponseHandler.GetSuccessResponse(newResp, "Service category successfully added.");
        }


        public async Task<APIResponse> UpdateServiceCategoryAsync(Guid id, ServiceCategoryRequestDto dto)
        {
            var entity = await _db.ServiceCategories.GetByIdAsync(id);
            if (entity == null || entity.IsDeleted)
                return ResponseHandler.GetNotFoundResponse("Service category not found");

            if (!string.Equals(entity.Name, dto.Name, StringComparison.OrdinalIgnoreCase)
                && await _db.ServiceCategories.GetByServiceCategoryName(dto.Name) != null)
            {
                return ResponseHandler.GetBadRequestResponse("Service category name already exists");
            }

            entity.Name = dto.Name;
            entity.Description = dto.Description;

            if (dto.CategoryImageFile != null && dto.CategoryImageFile.Length > 0)
            {
                if (!string.IsNullOrWhiteSpace(entity.CategoryImage))
                {
                    var oldPhysical = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot",
                        entity.CategoryImage.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                    if (File.Exists(oldPhysical)) File.Delete(oldPhysical);
                }
 
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

