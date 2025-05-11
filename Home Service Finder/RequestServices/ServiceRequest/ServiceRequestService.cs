using Home_Service_Finder.Data;
using Home_Service_Finder.Data.Contracts;
using Home_Service_Finder.RequestServices.ServiceOffers;
using Home_Service_Finder.RequestServices.ServiceOffers.Dtos;
using Home_Service_Finder.RequestServices.ServiceRequest.Contracts;
using Home_Service_Finder.RequestServices.ServiceRequest.Dtos;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Home_Service_Finder.RequestServices.ServiceRequest
{
    public class ServiceRequestService : IServiceRequestService
    {
        private readonly IUnitOfWork _dbContext;
        private readonly TimeSpan _requestExpiration = TimeSpan.FromMinutes(10);
        private readonly IHubContext<ServiceRequestHub> _hubContext;

        public ServiceRequestService(IUnitOfWork unitOfWork, IHubContext<ServiceRequestHub> hubContext)
        {
            _dbContext = unitOfWork;
            _hubContext = hubContext;
           
        }      


        public async Task<APIResponse> CreateServiceRequestAsync(ServiceRequestRequestDto serviceRequestRequestDto)
        {
            try
            {
                var customer = await _dbContext.Users.GetByIdAsync(serviceRequestRequestDto.CustomerId);
                if (customer == null)
                    return ResponseHandler.GetBadRequestResponse("Customer not found.");

                var activeRequests = (await _dbContext.ServiceRequests.GetAllAsync())
                    .Where(sr => sr.CustomerId == serviceRequestRequestDto.CustomerId &&
                               (sr.Status == "Pending" || sr.Status == "Accepted"))
                    .ToList();

                if (activeRequests.Any())
                    return ResponseHandler.GetBadRequestResponse("You already have an active service request.");

                var location = await _dbContext.Locations.GetByIdAsync(serviceRequestRequestDto.LocationId);
                if (location == null)
                    return ResponseHandler.GetBadRequestResponse("Location not found.");

                var category = await _dbContext.ServiceCategories.GetByIdAsync(serviceRequestRequestDto.ServiceCategoryId);
                if (category == null)
                    return ResponseHandler.GetBadRequestResponse("Service category not found.");

                if (serviceRequestRequestDto.ServiceListIds?.Any() == true)
                {
                    foreach (var serviceListId in serviceRequestRequestDto.ServiceListIds)
                    {
                        if (await _dbContext.ServiceLists.GetByIdAsync(serviceListId) == null)
                            return ResponseHandler.GetBadRequestResponse($"Service list with ID {serviceListId} not found.");
                    }
                }

                var serviceRequest = new ServiceRequest
                {
                    Id = Guid.NewGuid(),
                    CustomerId = serviceRequestRequestDto.CustomerId,
                    LocationId = serviceRequestRequestDto.LocationId,
                    ServiceCategoryId = serviceRequestRequestDto.ServiceCategoryId,
                    Description = serviceRequestRequestDto.Description,
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.Add(_requestExpiration),
                    Status = "Pending",
                    LocationAddress = serviceRequestRequestDto.Address ?? location.Address,
                    LocationCity = serviceRequestRequestDto.City ?? location.City,
                    LocationPostalCode = serviceRequestRequestDto.PostalCode ?? location.PostalCode,
                    LocationLatitude = serviceRequestRequestDto.Latitude ?? location.Latitude,
                    LocationLongitude = serviceRequestRequestDto.Longitude ?? location.Longitude
                };

                await _dbContext.ServiceRequests.AddAsync(serviceRequest);

                if (serviceRequestRequestDto.ServiceListIds?.Any() == true)
                {
                    foreach (var serviceListId in serviceRequestRequestDto.ServiceListIds)
                    {
                        await _dbContext.ServiceRequestServiceLists.AddAsync(new ServiceRequestServiceList
                        {
                            Id = Guid.NewGuid(),
                            RequestId = serviceRequest.Id,
                            ServiceListId = serviceListId
                        });
                    }
                }

                await _dbContext.SaveChangesAsync();

                var responseDto = await MapToResponseDto(serviceRequest);
                await _hubContext.Clients.Group($"Category_{serviceRequest.ServiceCategoryId}").SendAsync("NewRequestCreated", responseDto);

                return ResponseHandler.GetSuccessResponse(serviceRequest.Id, "Service request created successfully");
            }
            catch (Exception ex)
            {
                return ResponseHandler.GetBadRequestResponse("Failed to create service request: " + ex.Message);
            }
        }

        public async Task<APIResponse> UpdateServiceRequestStatusAsync(Guid requestId, string status)
        {
                var serviceRequest = await _dbContext.ServiceRequests.GetByIdAsync(requestId);
                if (serviceRequest == null)
                    return ResponseHandler.GetNotFoundResponse("Service request not found.");

                var allowedStatuses = new[] { "Pending", "In_Progress", "Accepted", "Rejected", "Cancelled", "Completed", "Expired" };
                if (!allowedStatuses.Contains(status))
                    return ResponseHandler.GetBadRequestResponse($"Invalid status. Allowed values: {string.Join(", ", allowedStatuses)}");

                if (serviceRequest.Status == status)
                {
                    var dtoAlready = await MapToResponseDto(serviceRequest);
                    return ResponseHandler.GetSuccessResponse(dtoAlready, $"Service request is already {status}.");
                }

                var terminalStatuses = new[] { "Cancelled", "Completed", "Expired" };
                if (terminalStatuses.Contains(serviceRequest.Status) && serviceRequest.Status != status)
                {
                    return ResponseHandler.GetBadRequestResponse($"Cannot change status from {serviceRequest.Status} to {status}.");
                }

                serviceRequest.Status = status;

                if (status == "Cancelled")
                    serviceRequest.ExpiresAt = DateTime.UtcNow;

                await _dbContext.SaveChangesAsync();

                var dto = await MapToResponseDto(serviceRequest);
                return ResponseHandler.GetSuccessResponse(dto, $"Service request status updated to {status} successfully.");
            
        }




        public async Task<APIResponse> CancelServiceRequestAsync(Guid requestId, Guid customerId)
        {
            try
            {
                var serviceRequest = await _dbContext.ServiceRequests.GetByIdAsync(requestId);
                if (serviceRequest == null)
                    return ResponseHandler.GetNotFoundResponse("Service request not found.");

                if (serviceRequest.CustomerId != customerId)
                    return ResponseHandler.GetBadRequestResponse("You can only cancel your own service requests.");

                if (serviceRequest.Status == "Cancelled")
                    return ResponseHandler.GetBadRequestResponse("This service request is already cancelled.");

                if (serviceRequest.Status == "Completed")
                    return ResponseHandler.GetBadRequestResponse("Cannot cancel a completed service request.");

                if (serviceRequest.Status != "Pending")
                    return ResponseHandler.GetBadRequestResponse("Only pending requests can be cancelled");

                if (serviceRequest.Status == "Expired")
                    return ResponseHandler.GetBadRequestResponse("Cannot cancel an expired service request.");

                serviceRequest.Status = "Cancelled";
                serviceRequest.ExpiresAt = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync();

                await _hubContext.Clients.Group($"Category_{serviceRequest.ServiceCategoryId}")
           .SendAsync("RequestCancelled", new
           {
               RequestId = serviceRequest.Id,
               CategoryId = serviceRequest.ServiceCategoryId,
               Status = "CANCELLED",
               CancelledAt = DateTime.UtcNow
           });

                await _hubContext.Clients.Group($"Customer_{serviceRequest.CustomerId}")



                    .SendAsync("RequestCancelled", new
                    {
                        RequestId = serviceRequest.Id,
                        Status = "CANCELLED",
                        CancelledAt = DateTime.UtcNow,
                        Message = "Your service request has been cancelled."
                    });

                return ResponseHandler.GetSuccessResponse(null, "Service request cancelled successfully.");
            }
            catch (Exception ex)
            {
                return ResponseHandler.GetBadRequestResponse("Failed to cancel service request: " + ex.Message);
            }
        }

        public async Task<APIResponse> CancelAcceptedRequestAsync(Guid requestId, CancelRequestDto dto)
        {
            var sr = await _dbContext.ServiceRequests.GetByIdAsync(requestId);
            if (sr == null)
                return ResponseHandler.GetNotFoundResponse("Service request not found.");
            if (sr.CustomerId != dto.CustomerId)
                return ResponseHandler.GetBadRequestResponse("You can only cancel your own request.");
            if (sr.Status != "Accepted")
                return ResponseHandler.GetBadRequestResponse("Only accepted requests require a reason.");

            sr.Status = "Cancelled";
            sr.CancelReason = dto.Reason;
            sr.ExpiresAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();

            var responseDto = await MapToResponseDto(sr);
            await _hubContext.Clients.Group($"Category_{sr.ServiceCategoryId}")
                .SendAsync("RequestCancelled", responseDto);
            await _hubContext.Clients.Group($"Customer_{sr.CustomerId}")
                .SendAsync("RequestCancelled", responseDto);

            var offer = (await _dbContext.ServiceOffers.GetAllAsync())
                           .FirstOrDefault(o => o.ServiceRequestId == requestId && o.Status == "Accepted");
            if (offer != null)
            {
                await _hubContext.Clients.Group($"Provider_{offer.ServiceProviderId}")
                    .SendAsync("ProviderRequestCancelled", responseDto);
            }

            return ResponseHandler.GetSuccessResponse(responseDto, "Service request cancelled with reason.");
        }



        public async Task<APIResponse> GetAllServiceRequestAsync()
        {
            try
            {
                var serviceRequests = await _dbContext.ServiceRequests.GetAllAsync();
                if (!serviceRequests.Any())
                    return ResponseHandler.GetNotFoundResponse("No service requests found.");

                var dtos = new List<ServiceRequestResponseDto>();
                foreach (var request in serviceRequests)
                {
                    dtos.Add(await MapToResponseDto(request));
                }

                return ResponseHandler.GetSuccessResponse(dtos, "Service requests retrieved successfully.");
            }
            catch (Exception ex)
            {
                return ResponseHandler.GetBadRequestResponse("Failed to retrieve service requests: " + ex.Message);
            }
        }

        public async Task<APIResponse> GetServiceRequestByIdAsync(Guid id)
        {
            try
            {
                var serviceRequest = await _dbContext.ServiceRequests.GetByIdAsync(id);
                if (serviceRequest == null)
                    return ResponseHandler.GetNotFoundResponse("Service request not found.");

                var dto = await MapToResponseDto(serviceRequest);
                return ResponseHandler.GetSuccessResponse(dto, "Service request retrieved successfully.");
            }
            catch (Exception ex)
            {
                return ResponseHandler.GetBadRequestResponse("Failed to retrieve service request: " + ex.Message);
            }
        }

        public async Task<APIResponse> GetRequestByCustomerId(Guid customerId)
        {
            try
            {
                var customer = await _dbContext.Users.GetByIdAsync(customerId);
                if (customer == null)
                    return ResponseHandler.GetNotFoundResponse("Customer not found.");

                var requests = (await _dbContext.ServiceRequests.GetAllAsync())
                    .Where(sr => sr.CustomerId == customerId )
                    .ToList();

                if (!requests.Any())
                    return ResponseHandler.GetNotFoundResponse("No service requests found for this customer.");

                var dtos = new List<ServiceRequestResponseDto>();
                foreach (var request in requests)
                {
                    dtos.Add(await MapToResponseDto(request));
                }

                return ResponseHandler.GetSuccessResponse(dtos, "Customer service requests retrieved successfully.");
            }
            catch (Exception ex)
            {
                return ResponseHandler.GetBadRequestResponse("Failed to retrieve customer service requests: " + ex.Message);
            }
        }

        public async Task<APIResponse> GetActiveRequestByCustomerId(Guid customerId)
        {
            try
            {
                var customer = await _dbContext.Users.GetByIdAsync(customerId);
                if (customer == null)
                    return ResponseHandler.GetNotFoundResponse("Customer not found.");

                var activeRequests = (await _dbContext.ServiceRequests.GetAllAsync())
                    .Where(sr => sr.CustomerId == customerId &&
                               (sr.Status == "Pending" || sr.Status == "Accepted"))
                    .ToList();

                if (!activeRequests.Any())
                    return null;

                var dtos = new List<ServiceRequestResponseDto>();
                foreach (var request in activeRequests)
                {
                    dtos.Add(await MapToResponseDto(request));
                }

                return ResponseHandler.GetSuccessResponse(dtos, "Active service requests retrieved successfully.");
            }
            catch (Exception ex)
            {
                return ResponseHandler.GetBadRequestResponse("Failed to retrieve active service requests: " + ex.Message);
            }
        }

        public async Task<APIResponse> GetPendingRequestByCustomerId(Guid customerId)
        {
            try
            {
                var customer = await _dbContext.Users.GetByIdAsync(customerId);
                if (customer == null)
                    return ResponseHandler.GetNotFoundResponse("Customer not found.");

                var pendingRequest = (await _dbContext.ServiceRequests.GetAllAsync())
                    .FirstOrDefault(sr => sr.CustomerId == customerId && sr.Status == "Pending");

                if (pendingRequest == null)
                    return ResponseHandler.GetSuccessResponse(null, "No pending service request.");

                var dto = await MapToResponseDto(pendingRequest);

                return ResponseHandler.GetSuccessResponse(dto, "Pending service request retrieved successfully.");
            }
            catch (Exception ex)
            {
                return ResponseHandler.GetBadRequestResponse("Failed to retrieve pending service request: " + ex.Message);
            }
        }





        public async Task<APIResponse> GetPendingRequestByCategory(Guid categoryId)
        {
            try
            {
                var customer = await _dbContext.ServiceCategories.GetByIdAsync(categoryId);
                if (customer == null)
                    return ResponseHandler.GetNotFoundResponse("Pending request  not found.");

                var activeRequests = (await _dbContext.ServiceRequests.GetAllAsync())
                    .Where(sr => sr.ServiceCategoryId == categoryId &&
                               (sr.Status == "Pending" ))
                    .ToList();

                if (!activeRequests.Any())
                    return ResponseHandler.GetNoContentResponse("Not conent");

                var dtos = new List<ServiceRequestResponseDto>();
                foreach (var request in activeRequests)
                {
                    dtos.Add(await MapToResponseDto(request));
                }

                return ResponseHandler.GetSuccessResponse(dtos, "Pending service requests retrieved successfully.");
            }
            catch (Exception ex)
            {
                return ResponseHandler.GetBadRequestResponse("Failed to retrieve pending service requests: " + ex.Message);
            }
        }

        public async Task<APIResponse> GetRequestByServiceCategory(Guid categoryId)
        {
            try
            {
                var category = await _dbContext.ServiceCategories.GetByIdAsync(categoryId);
                if (category == null)
                    return ResponseHandler.GetNotFoundResponse("Service category not found.");

                var requests = (await _dbContext.ServiceRequests.GetAllAsync())
                    .Where(sr => sr.ServiceCategoryId == categoryId)
                    .ToList();

                if (!requests.Any())
                    return ResponseHandler.GetNotFoundResponse("No service requests found for this category.");

                var dtos = new List<ServiceRequestResponseDto>();
                foreach (var request in requests)
                {
                    dtos.Add(await MapToResponseDto(request));
                }

                return ResponseHandler.GetSuccessResponse(dtos, "Category service requests retrieved successfully.");
            }
            catch (Exception ex)
            {
                return ResponseHandler.GetBadRequestResponse("Failed to retrieve category service requests: " + ex.Message);
            }
        }



        public async Task<APIResponse> DeleteRequestByCustomerId(Guid customerId)
        {
            try
            {
                var customer = await _dbContext.Users.GetByIdAsync(customerId);
                if (customer == null)
                    return ResponseHandler.GetNotFoundResponse("Customer not found.");

                var requests = (await _dbContext.ServiceRequests.GetAllAsync())
                    .Where(sr => sr.CustomerId == customerId)
                    .ToList();

                if (!requests.Any())
                    return ResponseHandler.GetNotFoundResponse("No service requests found for this customer.");

                var mappings = (await _dbContext.ServiceRequestServiceLists.GetAllAsync())
                    .Where(m => requests.Any(r => r.Id == m.RequestId))
                    .ToList();

                foreach (var mapping in mappings)
                {
                    await _dbContext.ServiceRequestServiceLists.DeleteAsync(mapping.Id);
                }

                foreach (var request in requests)
                {
                    await _dbContext.ServiceRequests.DeleteAsync(request.Id);
                }

                await _dbContext.SaveChangesAsync();
                return ResponseHandler.GetSuccessResponse(null, "Service requests deleted successfully.");
            }
            catch (Exception ex)
            {
                return ResponseHandler.GetBadRequestResponse("Failed to delete service requests: " + ex.Message);
            }
        }

        public async Task<APIResponse> UploadServiceRequestImagesAsync(Guid requestId, List<IFormFile> files)
        {
            var serviceRequest = await _dbContext.ServiceRequests.GetByIdAsync(requestId);
            if (serviceRequest == null)
                return ResponseHandler.GetNotFoundResponse("Service request not found.");

            if (files == null || !files.Any())
                return ResponseHandler.GetBadRequestResponse("No images uploaded.");

            if (files.Count > 6)
                return ResponseHandler.GetBadRequestResponse("Maximum 6 images allowed.");

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/serviceRequestImages");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var savedImagePaths = new List<string>();

            foreach (var file in files)
            {
                var fileName = $"{Guid.NewGuid()}_{file.FileName}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var imagePath = $"/serviceRequestImages/{fileName}";
                savedImagePaths.Add(imagePath);

                await _dbContext.ServiceRequestImages.AddAsync(new ServiceRequestImage
                {
                    ServiceRequestId = requestId,
                    ImagePath = imagePath
                });
            }

            await _dbContext.SaveChangesAsync();

            return ResponseHandler.GetSuccessResponse(savedImagePaths, "Images uploaded successfully.");
        }



        private async Task<ServiceRequestResponseDto> MapToResponseDto(ServiceRequest request)
        {
            var customerDetails = await _dbContext.UserDetails.GetByIdAsync(request.CustomerId);
            var category = await _dbContext.ServiceCategories.GetByIdAsync(request.ServiceCategoryId);

            var mappings = (await _dbContext.ServiceRequestServiceLists.GetAllAsync())
                .Where(m => m.RequestId == request.Id)
                .ToList();

            var serviceListIds = new List<Guid>();
            var serviceListNames = new List<string>();
            foreach (var mapping in mappings)
            {
                var serviceList = await _dbContext.ServiceLists.GetByIdAsync(mapping.ServiceListId);
                if (serviceList != null)
                {
                    serviceListIds.Add(serviceList.Id);
                    serviceListNames.Add(serviceList.Name);
                }
            }

            // Fetch images
            var images = (await _dbContext.ServiceRequestImages.GetAllAsync())
                .Where(img => img.ServiceRequestId == request.Id)
                .Select(img => img.ImagePath)
                .ToList();

            return new ServiceRequestResponseDto
            {
                Id = request.Id,
                CustomerId = request.CustomerId,
                CustomerName = $"{customerDetails?.FirstName} {customerDetails?.LastName}",
                LocationId = request.LocationId,
                ServiceCategoryId = request.ServiceCategoryId,
                ServiceCategoryName = category?.Name ?? "Unknown",
                Description = request.Description,
                CreatedAt = request.CreatedAt,
                ExpiresAt = request.ExpiresAt,
                Status = request.Status,
                CancelReason = request.CancelReason,
                ServiceListIds = serviceListIds,
                ServiceListNames = serviceListNames,
                LocationAddress = request.LocationAddress,
                LocationCity = request.LocationCity,
                LocationPostalCode = request.LocationPostalCode,
                LocationLatitude = request.LocationLatitude,
                LocationLongitude = request.LocationLongitude,
                ServiceRequestImagePaths = images
            };
        }



    }
}