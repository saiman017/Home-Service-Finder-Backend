using Home_Service_Finder.Data.Contracts;
using Home_Service_Finder.RequestServices.ServiceOffers.Dtos;
using Home_Service_Finder.RequestServices.ServiceOffers.Contracts;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Home_Service_Finder.Data;

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

            var providerDetails = await _dbContext.UserDetails.GetByIdAsync(serviceOffer.ServiceProviderId);

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

            bool hasExpiredOffers = false;
            foreach (var offer in offers.Where(o => o.Status == "Pending" && o.ExpiresAt <= now))
            {
                offer.Status = "Expired";
                hasExpiredOffers = true;

                var providerDetails = await _dbContext.UserDetails.GetByIdAsync(offer.ServiceProviderId);

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
            var provider = await _dbContext.Users.GetByIdAsync(providerId);
            if (provider == null)
                return ResponseHandler.GetNotFoundResponse("Service provider not found.");

            var offers = (await _dbContext.ServiceOffers.GetAllAsync())
                .Where(so => so.ServiceProviderId == providerId)
                .OrderByDescending(so => so.SentAt)
                .ToList();

            if (!offers.Any())
                return ResponseHandler.GetNotFoundResponse("No offers found for this service provider.");

            var providerDetails = await _dbContext.UserDetails.GetByIdAsync(providerId);

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

            offer.Status = "Accepted";

            var otherOffers = (await _dbContext.ServiceOffers.GetAllAsync())
                .Where(o => o.ServiceRequestId == serviceRequest.Id && o.Id != offerId)
                .ToList();

            foreach (var otherOffer in otherOffers)
            {
                otherOffer.Status = "Rejected";

                var rejectedProviderDetails = await _dbContext.UserDetails.GetByIdAsync(otherOffer.ServiceProviderId);

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

                await _hubContext.Clients.Group($"ProviderOffers_{otherOffer.ServiceProviderId}")
                    .SendAsync("YourOfferRejected", rejectedOfferDto);
            }

            serviceRequest.Status = "Accepted";
            serviceRequest.ExpiresAt = now; 

            await _dbContext.SaveChangesAsync();

            var acceptedProviderDetails = await _dbContext.UserDetails.GetByIdAsync(offer.ServiceProviderId);

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

            await _hubContext.Clients.Group($"ProviderOffers_{offer.ServiceProviderId}")
                .SendAsync("YourOfferAccepted", acceptedOfferDto);

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

            var providerDetails = await _dbContext.UserDetails.GetByIdAsync(offer.ServiceProviderId);

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
            var offer = await _dbContext.ServiceOffers.GetByIdAsync(offerId);
            if (offer == null)
                return ResponseHandler.GetNotFoundResponse("Service offer not found.");

            var providerDetails = await _dbContext.UserDetails.GetByIdAsync(offer.ServiceProviderId);
            if (providerDetails == null)
                return ResponseHandler.GetNotFoundResponse("Provider details not found.");

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

            
            return ResponseHandler.GetSuccessResponse(offerDto, "Service offer retrieved successfully.");
        }

        public async Task<APIResponse> UpdateOfferStatusAsync(Guid offerId, string newStatus, Guid requestId, Guid customerId)
        {
            
            if (newStatus == "In_Progress")
            {
                await _hubContext.Clients.Group($"Customer_{customerId}")
                    .SendAsync("ProviderReached", new
                    {
                        RequestId = requestId,
                        Status = "In_Progress",
                        Message = "The service provider has reached your location."
                    });
            }
            else if (newStatus == "Completed")
            {
                await _hubContext.Clients.Group($"Customer_{customerId}")
                    .SendAsync("ProviderCompleted", new
                    {
                        RequestId = requestId,
                        Status = "Completed",
                        Message = "The service provider has completed the service."
                    });
            }

            // 2. Update DB (Repository)
            var offer = await _dbContext.ServiceOffers.GetOfferWithRequestAsync(offerId);
            if (offer == null)
                return ResponseHandler.GetNotFoundResponse("Service offer not found.");

            offer.Status = newStatus;
             _dbContext.ServiceOffers.UpdateAsync(offer);
            await _dbContext.SaveChangesAsync();

            // 3. Prepare Offer DTO for Redux sync
            var providerDetails = await _dbContext.UserDetails.GetByIdAsync(offer.ServiceProviderId);
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

            // 4. Fire OfferStatusUpdated (Redux sync for customer)
            await _hubContext.Clients.Group($"RequestOffers_{offer.ServiceRequestId}")
                .SendAsync("OfferStatusUpdated", offerDto);

            // 5. Fire YourOfferStatusUpdated (Redux sync for provider)
            await _hubContext.Clients.Group($"ProviderOffers_{offer.ServiceProviderId}")
                .SendAsync("YourOfferStatusUpdated", offerDto);

            return ResponseHandler.GetSuccessResponse(null, $"Service offer status updated to {newStatus} successfully.");
        }



        public async Task<APIResponse> UpdatePaymentStatusAsync(Guid offerId, bool paymentStatus)
        {
            // 1) Load the offer
            var offer = await _dbContext.ServiceOffers.GetByIdAsync(offerId);
            if (offer == null)
                return ResponseHandler.GetNotFoundResponse("Service offer not found.");

            // 2) Only allow payment once work is completed
            if (offer.Status != "Completed")
                return ResponseHandler.GetBadRequestResponse("Cannot update payment until the work status is 'Completed'.");

            // 3) Update the flag
            offer.PaymentStatus = paymentStatus;
            _dbContext.ServiceOffers.UpdateAsync(offer);
            await _dbContext.SaveChangesAsync();

            // 4) Build DTO
            var providerDetails = await _dbContext.UserDetails.GetByIdAsync(offer.ServiceProviderId);
            var dto = new ServiceOfferResponseDto
            {
                Id = offer.Id,
                ServiceRequestId = offer.ServiceRequestId,
                ServiceProviderId = offer.ServiceProviderId,
                ProviderName = $"{providerDetails?.FirstName} {providerDetails?.LastName}",
                OfferedPrice = offer.OfferedPrice,
                SentAt = offer.SentAt,
                ExpiresAt = offer.ExpiresAt,
                Status = offer.Status,
                PaymentStatus = offer.PaymentStatus
            };

            // 5) Notify via SignalR
            await _hubContext.Clients.Group($"ProviderOffers_{offer.ServiceProviderId}")
                                     .SendAsync("YourOfferPaymentUpdated", dto);
            await _hubContext.Clients.Group($"RequestOffers_{offer.ServiceRequestId}")
                                     .SendAsync("OfferPaymentUpdated", dto);

            return ResponseHandler.GetSuccessResponse(dto, "Payment status updated successfully.");
        }

        public async Task<APIResponse> UpdatePaymentReasonAsync(Guid offerId, string paymentReason)
        {
            var offer = await _dbContext.ServiceOffers.GetByIdAsync(offerId);
            if (offer == null)
                return ResponseHandler.GetNotFoundResponse("Service offer not found.");

            if (offer.Status != "Completed")
                return ResponseHandler.GetBadRequestResponse("Can only add a reason once work is completed.");

            offer.PaymentReason = paymentReason;
            _dbContext.ServiceOffers.UpdateAsync(offer);
            await _dbContext.SaveChangesAsync();

            // build DTO and notify
            var providerDetails = await _dbContext.UserDetails.GetByIdAsync(offer.ServiceProviderId);
            var dto = new ServiceOfferResponseDto
            {
                Id = offer.Id,
                ServiceRequestId = offer.ServiceRequestId,
                ServiceProviderId = offer.ServiceProviderId,
                ProviderName = $"{providerDetails?.FirstName} {providerDetails?.LastName}",
                OfferedPrice = offer.OfferedPrice,
                SentAt = offer.SentAt,
                ExpiresAt = offer.ExpiresAt,
                Status = offer.Status,
                PaymentStatus = offer.PaymentStatus,
                PaymentReason = offer.PaymentReason
            };

            await _hubContext.Clients.Group($"ProviderOffers_{offer.ServiceProviderId}")
                 .SendAsync("YourOfferPaymentUpdated", dto);
            await _hubContext.Clients.Group($"RequestOffers_{offer.ServiceRequestId}")
                 .SendAsync("OfferPaymentUpdated", dto);

            return ResponseHandler.GetSuccessResponse(dto, "Reason recorded.");
        }








    }
}