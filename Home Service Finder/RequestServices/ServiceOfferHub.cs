using Microsoft.AspNetCore.SignalR;
using Home_Service_Finder.RequestServices.ServiceOffers.Dtos;
using System;
using System.Threading.Tasks;

namespace Home_Service_Finder.RequestServices
{
    public class ServiceOfferHub : Hub
    {

        public async Task JoinProviderOffersGroup(string providerId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"ProviderOffers_{providerId}");
        }

        public async Task JoinRequestOffersGroup(string requestId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"RequestOffers_{requestId}");
        }


        public async Task NotifyNewOffer(ServiceOfferResponseDto offer)
        {
            await Clients.Group($"RequestOffers_{offer.ServiceRequestId}")
                         .SendAsync("NewOfferReceived", offer);
        }

        public async Task NotifyOfferAccepted(ServiceOfferResponseDto offer)
        {
            await Clients.Group($"ProviderOffers_{offer.ServiceProviderId}")
                         .SendAsync("YourOfferAccepted", offer);

            await Clients.Group($"RequestOffers_{offer.ServiceRequestId}")
                         .SendAsync("RequestOfferAccepted", offer);
        }

        public async Task NotifyOfferRejected(ServiceOfferResponseDto offer)
        {
            await Clients.Group($"ProviderOffers_{offer.ServiceProviderId}")
                         .SendAsync("YourOfferRejected", offer);
        }

        public async Task NotifyOfferStatusUpdate(ServiceOfferResponseDto offer)
        {
            await Clients.Group($"RequestOffers_{offer.ServiceRequestId}")
                         .SendAsync("OfferStatusUpdated", offer);

            await Clients.Group($"ProviderOffers_{offer.ServiceProviderId}")
                         .SendAsync("YourOfferStatusUpdated", offer);
        }

        public async Task NotifyOfferExpired(ServiceOfferResponseDto offer)
        {
            await Clients.Group($"ProviderOffers_{offer.ServiceProviderId}")
                         .SendAsync("YourOfferExpired", offer);

            await Clients.Group($"RequestOffers_{offer.ServiceRequestId}")
                         .SendAsync("OfferExpired", offer);
        }

        public async Task NotifyPaymentUpdated(ServiceOfferResponseDto offer)
        {
            await Clients.Group($"ProviderOffers_{offer.ServiceProviderId}")
                         .SendAsync("YourOfferPaymentUpdated", offer);

            await Clients.Group($"RequestOffers_{offer.ServiceRequestId}")
                         .SendAsync("OfferPaymentUpdated", offer);
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
