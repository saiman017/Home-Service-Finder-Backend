//using Home_Service_Finder.Data.Contracts;
//using Home_Service_Finder.RequestServices.ServiceOffers.Dtos;
//using Home_Service_Finder.RequestServices.ServiceOffers;
//using Home_Service_Finder;
//using Home_Service_Finder.RequestServices.ServiceOffers.Dtos;
//using Home_Service_Finder.RequestServices.ServiceOffers.Contracts;
//using Home_Service_Finder.RequestServices.ServiceOffers;

//public class ServiceOfferService : IServiceOfferService
//{
//    private readonly IUnitOfWork _dbContext;
//    private readonly TimeSpan _offerExpiration = TimeSpan.FromMinutes(5);

//    public ServiceOfferService(IUnitOfWork unitOfWork)
//    {
//        _dbContext = unitOfWork;
//    }

//    public async Task<APIResponse> CreateServiceOfferAsync(ServiceOfferRequestDto serviceOfferRequestDto)
//    {
//        var serviceRequest = await _dbContext.ServiceRequests.GetByIdAsync(serviceOfferRequestDto.ServiceRequestId);
//        if (serviceRequest == null)
//            return ResponseHandler.GetNotFoundResponse("Service request not found.");

//        if (serviceRequest.Status != "Pending")
//            return ResponseHandler.GetBadRequestResponse($"Cannot send offer to a service request with status '{serviceRequest.Status}'.");

//        var provider = await _dbContext.Users.GetByIdAsync(serviceOfferRequestDto.ServiceProviderId);
//        if (provider == null)
//            return ResponseHandler.GetBadRequestResponse("Service provider not found.");

//        var existingOffers = (await _dbContext.ServiceOffers.GetAllAsync())
//            .Where(so => so.ServiceRequestId == serviceOfferRequestDto.ServiceRequestId &&
//                       so.ServiceProviderId == serviceOfferRequestDto.ServiceProviderId)
//            .ToList();

//        if (existingOffers.Any())
//            return ResponseHandler.GetBadRequestResponse("You have already sent an offer for this service request.");

//        var now = DateTime.UtcNow;
//        var serviceOffer = new ServiceOffer
//        {
//            Id = Guid.NewGuid(),
//            ServiceRequestId = serviceOfferRequestDto.ServiceRequestId,
//            ServiceProviderId = serviceOfferRequestDto.ServiceProviderId,
//            OfferedPrice = serviceOfferRequestDto.OfferedPrice,
//            SentAt = now,
//            ExpiresAt = now.Add(_offerExpiration),
//            Status = "Pending"
//        };

//        await _dbContext.ServiceOffers.AddAsync(serviceOffer);
//        await _dbContext.SaveChangesAsync();

//        return ResponseHandler.GetSuccessResponse(serviceOffer.Id, "Service offer sent successfully.");
//    }

//    public async Task<APIResponse> GetOffersByRequestIdAsync(Guid requestId)
//    {
//        var serviceRequest = await _dbContext.ServiceRequests.GetByIdAsync(requestId);
//        if (serviceRequest == null)
//            return ResponseHandler.GetNotFoundResponse("Service request not found.");

//        var now = DateTime.UtcNow;
//        var offers = (await _dbContext.ServiceOffers.GetAllAsync())
//            .Where(so => so.ServiceRequestId == requestId)
//            .ToList();

//        // Update expired offers
//        foreach (var offer in offers.Where(o => o.Status == "Pending" && o.ExpiresAt <= now))
//        {
//            offer.Status = "Expired";
//        }
//        await _dbContext.SaveChangesAsync();

//        // Filter after potential updates
//        offers = offers.Where(o => o.Status != "Rejected").ToList();

//        if (!offers.Any())
//            return ResponseHandler.GetNotFoundResponse("No offers found for this service request.");

//        var offerDtos = new List<ServiceOfferResponseDto>();
//        foreach (var offer in offers)
//        {
//            var providerDetails = await _dbContext.UserDetails.GetByIdAsync(offer.ServiceProviderId);

//            offerDtos.Add(new ServiceOfferResponseDto
//            {
//                Id = offer.Id,
//                ServiceRequestId = offer.ServiceRequestId,
//                ServiceProviderId = offer.ServiceProviderId,
//                ProviderName = $"{providerDetails?.FirstName} {providerDetails?.LastName}",
//                OfferedPrice = offer.OfferedPrice,
//                SentAt = offer.SentAt,
//                ExpiresAt = offer.ExpiresAt,
//                Status = offer.Status
//            });
//        }

//        return ResponseHandler.GetSuccessResponse(offerDtos, "Service offers retrieved successfully.");
//    }

//    public async Task<APIResponse> GetOffersByProviderIdAsync(Guid providerId)
//    {
//        // Verify provider exists
//        var provider = await _dbContext.Users.GetByIdAsync(providerId);
//        if (provider == null)
//            return ResponseHandler.GetNotFoundResponse("Service provider not found.");

//        // Get all offers for this provider
//        var offers = (await _dbContext.ServiceOffers.GetAllAsync())
//            .Where(so => so.ServiceProviderId == providerId)
//            .OrderByDescending(so => so.SentAt)
//            .ToList();

//        if (!offers.Any())
//            return ResponseHandler.GetNotFoundResponse("No offers found for this service provider.");

//        // Get provider details (assuming you have a UserDetails table)
//        var providerDetails = await _dbContext.UserDetails.GetByIdAsync(providerId);

//        // Map to DTO
//        var offerDtos = offers.Select(offer => new ServiceOfferResponseDto
//        {
//            Id = offer.Id,
//            ServiceRequestId = offer.ServiceRequestId,
//            ServiceProviderId = offer.ServiceProviderId,
//            ProviderName = $"{providerDetails?.FirstName} {providerDetails?.LastName}",
//            OfferedPrice = offer.OfferedPrice,
//            SentAt = offer.SentAt,
//            ExpiresAt = offer.ExpiresAt,
//            Status = offer.Status
//        }).ToList();

//        return ResponseHandler.GetSuccessResponse(offerDtos, "Service offers retrieved successfully.");
//    }

//    public async Task<APIResponse> AcceptOfferAsync(Guid offerId, Guid customerId)
//    {
//        var offer = await _dbContext.ServiceOffers.GetByIdAsync(offerId);
//        if (offer == null)
//            return ResponseHandler.GetNotFoundResponse("Service offer not found.");

//        var now = DateTime.UtcNow;

//        if (offer.Status != "Pending")
//            return ResponseHandler.GetBadRequestResponse($"This offer has already been {offer.Status.ToLower()}.");

//        if (offer.ExpiresAt <= now)
//        {
//            offer.Status = "Expired";
//            await _dbContext.SaveChangesAsync();
//            return ResponseHandler.GetBadRequestResponse("This offer has expired and can no longer be accepted.");
//        }

//        var serviceRequest = await _dbContext.ServiceRequests.GetByIdAsync(offer.ServiceRequestId);
//        if (serviceRequest == null)
//            return ResponseHandler.GetNotFoundResponse("Service request not found.");

//        if (serviceRequest.CustomerId != customerId)
//            return ResponseHandler.GetBadRequestResponse("You can only accept offers for your own service requests.");

//        if (serviceRequest.Status != "Pending")
//            return ResponseHandler.GetBadRequestResponse($"Cannot accept offers for a service request with status '{serviceRequest.Status}'.");

//        // Update offer status
//        offer.Status = "Accepted";

//        // Update all other offers for this request to Rejected
//        var otherOffers = (await _dbContext.ServiceOffers.GetAllAsync())
//            .Where(o => o.ServiceRequestId == serviceRequest.Id && o.Id != offerId)
//            .ToList();

//        foreach (var otherOffer in otherOffers)
//        {
//            otherOffer.Status = "Rejected";
//        }

//        // Update request status
//        serviceRequest.Status = "Accepted";
//        serviceRequest.ExpiresAt = now; // Immediately expire the request

//        await _dbContext.SaveChangesAsync();
//        return ResponseHandler.GetSuccessResponse(null, "Service offer accepted successfully.");
//    }

//    public async Task<APIResponse> RejectOfferAsync(Guid offerId, Guid customerId)
//    {
//        var offer = await _dbContext.ServiceOffers.GetByIdAsync(offerId);
//        if (offer == null)
//            return ResponseHandler.GetNotFoundResponse("Service offer not found.");

//        if (offer.Status != "Pending")
//            return ResponseHandler.GetBadRequestResponse($"This offer has already been {offer.Status.ToLower()}.");

//        var serviceRequest = await _dbContext.ServiceRequests.GetByIdAsync(offer.ServiceRequestId);
//        if (serviceRequest == null)
//            return ResponseHandler.GetNotFoundResponse("Service request not found.");

//        if (serviceRequest.CustomerId != customerId)
//            return ResponseHandler.GetBadRequestResponse("You can only reject offers for your own service requests.");

//        offer.Status = "Rejected";
//        await _dbContext.SaveChangesAsync();

//        return ResponseHandler.GetSuccessResponse(null, "Service offer rejected successfully.");
//    }

//    public async Task<APIResponse> GetOfferByIdAsync(Guid offerId)
//    {
//        // Retrieve the service offer by ID
//        var offer = await _dbContext.ServiceOffers.GetByIdAsync(offerId);
//        if (offer == null)
//            return ResponseHandler.GetNotFoundResponse("Service offer not found.");

//        // Retrieve provider details
//        var providerDetails = await _dbContext.UserDetails.GetByIdAsync(offer.ServiceProviderId);
//        if (providerDetails == null)
//            return ResponseHandler.GetNotFoundResponse("Provider details not found.");

//        // Map the offer to a response DTO
//        var offerDto = new ServiceOfferResponseDto
//        {
//            Id = offer.Id,
//            ServiceRequestId = offer.ServiceRequestId,
//            ServiceProviderId = offer.ServiceProviderId,
//            ProviderName = $"{providerDetails.FirstName} {providerDetails.LastName}",
//            OfferedPrice = offer.OfferedPrice,
//            SentAt = offer.SentAt,
//            ExpiresAt = offer.ExpiresAt,
//            Status = offer.Status
//        };

//        // Return the success response with the mapped DTO
//        return ResponseHandler.GetSuccessResponse(offerDto, "Service offer retrieved successfully.");
//    }

//    public async Task<APIResponse> UpdateOfferStatusAsync(Guid offerId, string status)
//    {
//        var offer = await _dbContext.ServiceOffers.GetByIdAsync(offerId);
//        if (offer == null)
//            return ResponseHandler.GetNotFoundResponse("Service offer not found.");

//        // Validate status
//        if (!new[] { "Pending", "In_Progress", "Completed", "Cancelled" }.Contains(status))
//            return ResponseHandler.GetBadRequestResponse("Invalid status value.");

//        offer.Status = status;
//        await _dbContext.SaveChangesAsync();

//        return ResponseHandler.GetSuccessResponse(null, $"Service offer status updated to {status} successfully.");
//    }

//}


using Home_Service_Finder.Data.Contracts;
using Home_Service_Finder.RequestServices.ServiceOffers.Dtos;
using Home_Service_Finder.RequestServices.ServiceOffers.Contracts;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Home_Service_Finder.RequestServices.ServiceOffers
{
    public class ServiceOfferService : IServiceOfferService
    {
        private readonly IUnitOfWork _dbContext;
        private readonly IHubContext<ServiceOfferHub> _hubContext;
        private readonly TimeSpan _offerExpiration = TimeSpan.FromMinutes(5);

        public ServiceOfferService(IUnitOfWork unitOfWork, IHubContext<ServiceOfferHub> hubContext)
        {
            _dbContext = unitOfWork;
            _hubContext = hubContext;
        }

        public async Task<APIResponse> CreateServiceOfferAsync(ServiceOfferRequestDto serviceOfferRequestDto)
        {
            var serviceRequest = await _dbContext.ServiceRequests.GetByIdAsync(serviceOfferRequestDto.ServiceRequestId);
            if (serviceRequest == null)
                return ResponseHandler.GetNotFoundResponse("Service request not found.");

            if (serviceRequest.Status != "Pending")
                return ResponseHandler.GetBadRequestResponse($"Cannot send offer to a service request with status '{serviceRequest.Status}'.");

            var provider = await _dbContext.Users.GetByIdAsync(serviceOfferRequestDto.ServiceProviderId);
            if (provider == null)
                return ResponseHandler.GetBadRequestResponse("Service provider not found.");

            var existingOffers = (await _dbContext.ServiceOffers.GetAllAsync())
                .Where(so => so.ServiceRequestId == serviceOfferRequestDto.ServiceRequestId &&
                           so.ServiceProviderId == serviceOfferRequestDto.ServiceProviderId)
                .ToList();

            if (existingOffers.Any())
                return ResponseHandler.GetBadRequestResponse("You have already sent an offer for this service request.");

            var now = DateTime.UtcNow;
            var serviceOffer = new ServiceOffer
            {
                Id = Guid.NewGuid(),
                ServiceRequestId = serviceOfferRequestDto.ServiceRequestId,
                ServiceProviderId = serviceOfferRequestDto.ServiceProviderId,
                OfferedPrice = serviceOfferRequestDto.OfferedPrice,
                SentAt = now,
                ExpiresAt = now.Add(_offerExpiration),
                Status = "Pending"
            };

            await _dbContext.ServiceOffers.AddAsync(serviceOffer);
            await _dbContext.SaveChangesAsync();

            // Get provider details for notification
            var providerDetails = await _dbContext.UserDetails.GetByIdAsync(serviceOffer.ServiceProviderId);

            // Create response DTO for real-time notification
            var offerDto = new ServiceOfferResponseDto
            {
                Id = serviceOffer.Id,
                ServiceRequestId = serviceOffer.ServiceRequestId,
                ServiceProviderId = serviceOffer.ServiceProviderId,
                ProviderName = $"{providerDetails?.FirstName} {providerDetails?.LastName}",
                OfferedPrice = serviceOffer.OfferedPrice,
                SentAt = serviceOffer.SentAt,
                ExpiresAt = serviceOffer.ExpiresAt,
                Status = serviceOffer.Status
            };

            // Send real-time notification to customer
            await _hubContext.Clients.Group($"RequestOffers_{serviceOffer.ServiceRequestId}")
                .SendAsync("NewOfferReceived", offerDto);

            return ResponseHandler.GetSuccessResponse(serviceOffer.Id, "Service offer sent successfully.");
        }

        public async Task<APIResponse> GetOffersByRequestIdAsync(Guid requestId)
        {
            var serviceRequest = await _dbContext.ServiceRequests.GetByIdAsync(requestId);
            if (serviceRequest == null)
                return ResponseHandler.GetNotFoundResponse("Service request not found.");

            var now = DateTime.UtcNow;
            var offers = (await _dbContext.ServiceOffers.GetAllAsync())
                .Where(so => so.ServiceRequestId == requestId)
                .ToList();

            // Update expired offers
            bool hasExpiredOffers = false;
            foreach (var offer in offers.Where(o => o.Status == "Pending" && o.ExpiresAt <= now))
            {
                offer.Status = "Expired";
                hasExpiredOffers = true;

                // Get provider details for notification
                var providerDetails = await _dbContext.UserDetails.GetByIdAsync(offer.ServiceProviderId);

                // Create response DTO for real-time notification
                var offerDto = new ServiceOfferResponseDto
                {
                    Id = offer.Id,
                    ServiceRequestId = offer.ServiceRequestId,
                    ServiceProviderId = offer.ServiceProviderId,
                    ProviderName = $"{providerDetails?.FirstName} {providerDetails?.LastName}",
                    OfferedPrice = offer.OfferedPrice,
                    SentAt = offer.SentAt,
                    ExpiresAt = offer.ExpiresAt,
                    Status = offer.Status
                };

                // Notify about expired offer
                await _hubContext.Clients.Group($"ProviderOffers_{offer.ServiceProviderId}")
                    .SendAsync("YourOfferExpired", offerDto);

                await _hubContext.Clients.Group($"RequestOffers_{offer.ServiceRequestId}")
                    .SendAsync("OfferExpired", offerDto);
            }

            if (hasExpiredOffers)
            {
                await _dbContext.SaveChangesAsync();
            }

            // Filter after potential updates
            offers = offers.Where(o => o.Status != "Rejected").ToList();

            if (!offers.Any())
                return ResponseHandler.GetNotFoundResponse("No offers found for this service request.");

            var offerDtos = new List<ServiceOfferResponseDto>();
            foreach (var offer in offers)
            {
                var providerDetails = await _dbContext.UserDetails.GetByIdAsync(offer.ServiceProviderId);

                offerDtos.Add(new ServiceOfferResponseDto
                {
                    Id = offer.Id,
                    ServiceRequestId = offer.ServiceRequestId,
                    ServiceProviderId = offer.ServiceProviderId,
                    ProviderName = $"{providerDetails?.FirstName} {providerDetails?.LastName}",
                    OfferedPrice = offer.OfferedPrice,
                    SentAt = offer.SentAt,
                    ExpiresAt = offer.ExpiresAt,
                    Status = offer.Status
                });
            }

            return ResponseHandler.GetSuccessResponse(offerDtos, "Service offers retrieved successfully.");
        }

        public async Task<APIResponse> GetOffersByProviderIdAsync(Guid providerId)
        {
            // Verify provider exists
            var provider = await _dbContext.Users.GetByIdAsync(providerId);
            if (provider == null)
                return ResponseHandler.GetNotFoundResponse("Service provider not found.");

            // Get all offers for this provider
            var offers = (await _dbContext.ServiceOffers.GetAllAsync())
                .Where(so => so.ServiceProviderId == providerId)
                .OrderByDescending(so => so.SentAt)
                .ToList();

            if (!offers.Any())
                return ResponseHandler.GetNotFoundResponse("No offers found for this service provider.");

            // Get provider details
            var providerDetails = await _dbContext.UserDetails.GetByIdAsync(providerId);

            // Map to DTO
            var offerDtos = offers.Select(offer => new ServiceOfferResponseDto
            {
                Id = offer.Id,
                ServiceRequestId = offer.ServiceRequestId,
                ServiceProviderId = offer.ServiceProviderId,
                ProviderName = $"{providerDetails?.FirstName} {providerDetails?.LastName}",
                OfferedPrice = offer.OfferedPrice,
                SentAt = offer.SentAt,
                ExpiresAt = offer.ExpiresAt,
                Status = offer.Status
            }).ToList();

            return ResponseHandler.GetSuccessResponse(offerDtos, "Service offers retrieved successfully.");
        }

        public async Task<APIResponse> AcceptOfferAsync(Guid offerId, Guid customerId)
        {
            var offer = await _dbContext.ServiceOffers.GetByIdAsync(offerId);
            if (offer == null)
                return ResponseHandler.GetNotFoundResponse("Service offer not found.");

            var now = DateTime.UtcNow;

            if (offer.Status != "Pending")
                return ResponseHandler.GetBadRequestResponse($"This offer has already been {offer.Status.ToLower()}.");

            if (offer.ExpiresAt <= now)
            {
                offer.Status = "Expired";
                await _dbContext.SaveChangesAsync();
                return ResponseHandler.GetBadRequestResponse("This offer has expired and can no longer be accepted.");
            }

            var serviceRequest = await _dbContext.ServiceRequests.GetByIdAsync(offer.ServiceRequestId);
            if (serviceRequest == null)
                return ResponseHandler.GetNotFoundResponse("Service request not found.");

            if (serviceRequest.CustomerId != customerId)
                return ResponseHandler.GetBadRequestResponse("You can only accept offers for your own service requests.");

            if (serviceRequest.Status != "Pending")
                return ResponseHandler.GetBadRequestResponse($"Cannot accept offers for a service request with status '{serviceRequest.Status}'.");

            // Update offer status
            offer.Status = "Accepted";

            // Update all other offers for this request to Rejected
            var otherOffers = (await _dbContext.ServiceOffers.GetAllAsync())
                .Where(o => o.ServiceRequestId == serviceRequest.Id && o.Id != offerId)
                .ToList();

            foreach (var otherOffer in otherOffers)
            {
                otherOffer.Status = "Rejected";

                // Get provider details for rejected offer
                var rejectedProviderDetails = await _dbContext.UserDetails.GetByIdAsync(otherOffer.ServiceProviderId);

                // Create response DTO for rejected offer
                var rejectedOfferDto = new ServiceOfferResponseDto
                {
                    Id = otherOffer.Id,
                    ServiceRequestId = otherOffer.ServiceRequestId,
                    ServiceProviderId = otherOffer.ServiceProviderId,
                    ProviderName = $"{rejectedProviderDetails?.FirstName} {rejectedProviderDetails?.LastName}",
                    OfferedPrice = otherOffer.OfferedPrice,
                    SentAt = otherOffer.SentAt,
                    ExpiresAt = otherOffer.ExpiresAt,
                    Status = otherOffer.Status
                };

                // Notify provider about rejected offer
                await _hubContext.Clients.Group($"ProviderOffers_{otherOffer.ServiceProviderId}")
                    .SendAsync("YourOfferRejected", rejectedOfferDto);
            }

            // Update request status
            serviceRequest.Status = "Accepted";
            serviceRequest.ExpiresAt = now; // Immediately expire the request

            await _dbContext.SaveChangesAsync();

            // Get provider details for accepted offer
            var acceptedProviderDetails = await _dbContext.UserDetails.GetByIdAsync(offer.ServiceProviderId);

            // Create response DTO for accepted offer
            var acceptedOfferDto = new ServiceOfferResponseDto
            {
                Id = offer.Id,
                ServiceRequestId = offer.ServiceRequestId,
                ServiceProviderId = offer.ServiceProviderId,
                ProviderName = $"{acceptedProviderDetails?.FirstName} {acceptedProviderDetails?.LastName}",
                OfferedPrice = offer.OfferedPrice,
                SentAt = offer.SentAt,
                ExpiresAt = offer.ExpiresAt,
                Status = offer.Status
            };

            // Notify provider about accepted offer
            await _hubContext.Clients.Group($"ProviderOffers_{offer.ServiceProviderId}")
                .SendAsync("YourOfferAccepted", acceptedOfferDto);

            // Notify all users monitoring this request that an offer was accepted
            await _hubContext.Clients.Group($"RequestOffers_{offer.ServiceRequestId}")
                .SendAsync("RequestOfferAccepted", acceptedOfferDto);

            return ResponseHandler.GetSuccessResponse(null, "Service offer accepted successfully.");
        }

        public async Task<APIResponse> RejectOfferAsync(Guid offerId, Guid customerId)
        {
            var offer = await _dbContext.ServiceOffers.GetByIdAsync(offerId);
            if (offer == null)
                return ResponseHandler.GetNotFoundResponse("Service offer not found.");

            if (offer.Status != "Pending")
                return ResponseHandler.GetBadRequestResponse($"This offer has already been {offer.Status.ToLower()}.");

            var serviceRequest = await _dbContext.ServiceRequests.GetByIdAsync(offer.ServiceRequestId);
            if (serviceRequest == null)
                return ResponseHandler.GetNotFoundResponse("Service request not found.");

            if (serviceRequest.CustomerId != customerId)
                return ResponseHandler.GetBadRequestResponse("You can only reject offers for your own service requests.");

            offer.Status = "Rejected";
            await _dbContext.SaveChangesAsync();

            // Get provider details for notification
            var providerDetails = await _dbContext.UserDetails.GetByIdAsync(offer.ServiceProviderId);

            // Create response DTO for real-time notification
            var offerDto = new ServiceOfferResponseDto
            {
                Id = offer.Id,
                ServiceRequestId = offer.ServiceRequestId,
                ServiceProviderId = offer.ServiceProviderId,
                ProviderName = $"{providerDetails?.FirstName} {providerDetails?.LastName}",
                OfferedPrice = offer.OfferedPrice,
                SentAt = offer.SentAt,
                ExpiresAt = offer.ExpiresAt,
                Status = offer.Status
            };

            // Notify provider about rejected offer
            await _hubContext.Clients.Group($"ProviderOffers_{offer.ServiceProviderId}")
                .SendAsync("YourOfferRejected", offerDto);

            return ResponseHandler.GetSuccessResponse(null, "Service offer rejected successfully.");
        }

        public async Task<APIResponse> GetOfferByIdAsync(Guid offerId)
        {
            // Retrieve the service offer by ID
            var offer = await _dbContext.ServiceOffers.GetByIdAsync(offerId);
            if (offer == null)
                return ResponseHandler.GetNotFoundResponse("Service offer not found.");

            // Retrieve provider details
            var providerDetails = await _dbContext.UserDetails.GetByIdAsync(offer.ServiceProviderId);
            if (providerDetails == null)
                return ResponseHandler.GetNotFoundResponse("Provider details not found.");

            // Map the offer to a response DTO
            var offerDto = new ServiceOfferResponseDto
            {
                Id = offer.Id,
                ServiceRequestId = offer.ServiceRequestId,
                ServiceProviderId = offer.ServiceProviderId,
                ProviderName = $"{providerDetails.FirstName} {providerDetails.LastName}",
                OfferedPrice = offer.OfferedPrice,
                SentAt = offer.SentAt,
                ExpiresAt = offer.ExpiresAt,
                Status = offer.Status
            };

            // Return the success response with the mapped DTO
            return ResponseHandler.GetSuccessResponse(offerDto, "Service offer retrieved successfully.");
        }

        public async Task<APIResponse> UpdateOfferStatusAsync(Guid offerId, string status)
        {
            var offer = await _dbContext.ServiceOffers.GetByIdAsync(offerId);
            if (offer == null)
                return ResponseHandler.GetNotFoundResponse("Service offer not found.");

            // Validate status
            if (!new[] { "Pending", "In_Progress", "Completed", "Cancelled" }.Contains(status))
                return ResponseHandler.GetBadRequestResponse("Invalid status value.");

            offer.Status = status;
            await _dbContext.SaveChangesAsync();

            // Get provider details for notification
            var providerDetails = await _dbContext.UserDetails.GetByIdAsync(offer.ServiceProviderId);

            // Create response DTO for real-time notification
            var offerDto = new ServiceOfferResponseDto
            {
                Id = offer.Id,
                ServiceRequestId = offer.ServiceRequestId,
                ServiceProviderId = offer.ServiceProviderId,
                ProviderName = $"{providerDetails?.FirstName} {providerDetails?.LastName}",
                OfferedPrice = offer.OfferedPrice,
                SentAt = offer.SentAt,
                ExpiresAt = offer.ExpiresAt,
                Status = offer.Status
            };

            // Notify about status update
            await _hubContext.Clients.Group($"ProviderOffers_{offer.ServiceProviderId}")
                .SendAsync("YourOfferStatusUpdated", offerDto);

            await _hubContext.Clients.Group($"RequestOffers_{offer.ServiceRequestId}")
                .SendAsync("OfferStatusUpdated", offerDto);

            return ResponseHandler.GetSuccessResponse(null, $"Service offer status updated to {status} successfully.");
        }
    }
}