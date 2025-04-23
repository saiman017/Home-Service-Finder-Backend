using Microsoft.AspNetCore.SignalR;
using Home_Service_Finder.RequestServices.ServiceOffers.Dtos;
using System;
using System.Threading.Tasks;

namespace Home_Service_Finder.RequestServices
{
    public class ServiceOfferHub : Hub
    {
        // Method for providers to join their offer notifications group
        public async Task JoinProviderOffersGroup(string providerId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"ProviderOffers_{providerId}");
        }

        // Method for customers to join their request offers group
        public async Task JoinRequestOffersGroup(string requestId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"RequestOffers_{requestId}");
        }

        // Method to notify about new offers
        public async Task NotifyNewOffer(ServiceOfferResponseDto offer)
        {
            // Notify the customer who owns the request
            await Clients.Group($"RequestOffers_{offer.ServiceRequestId}")
                .SendAsync("NewOfferReceived", offer);
        }

        // Method to notify about offer acceptance
        public async Task NotifyOfferAccepted(ServiceOfferResponseDto offer)
        {
            // Notify the provider whose offer was accepted
            await Clients.Group($"ProviderOffers_{offer.ServiceProviderId}")
                .SendAsync("YourOfferAccepted", offer);

            // Notify all providers who submitted offers about this request
            await Clients.Group($"RequestOffers_{offer.ServiceRequestId}")
                .SendAsync("RequestOfferAccepted", offer);
        }

        // Method to notify about offer rejection
        public async Task NotifyOfferRejected(ServiceOfferResponseDto offer)
        {
            // Notify the provider whose offer was rejected
            await Clients.Group($"ProviderOffers_{offer.ServiceProviderId}")
                .SendAsync("YourOfferRejected", offer);
        }

        // Method to notify about offer status updates
        public async Task NotifyOfferStatusUpdate(ServiceOfferResponseDto offer)
        {
            // Notify the customer who owns the request
            await Clients.Group($"RequestOffers_{offer.ServiceRequestId}")
                .SendAsync("OfferStatusUpdated", offer);

            // Notify the provider who made the offer
            await Clients.Group($"ProviderOffers_{offer.ServiceProviderId}")
                .SendAsync("YourOfferStatusUpdated", offer);
        }

        // Method to notify about offer expiration
        public async Task NotifyOfferExpired(ServiceOfferResponseDto offer)
        {
            // Notify the provider whose offer expired
            await Clients.Group($"ProviderOffers_{offer.ServiceProviderId}")
                .SendAsync("YourOfferExpired", offer);

            // Notify the customer about the expired offer
            await Clients.Group($"RequestOffers_{offer.ServiceRequestId}")
                .SendAsync("OfferExpired", offer);
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await base.OnDisconnectedAsync(exception);
        }


    }
}