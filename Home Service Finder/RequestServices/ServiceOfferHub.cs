﻿//using Microsoft.AspNetCore.SignalR;
//using Home_Service_Finder.RequestServices.ServiceOffers.Dtos;
//using System;
//using System.Threading.Tasks;

//namespace Home_Service_Finder.RequestServices
//{
//    public class ServiceOfferHub : Hub
//    {
//        // Method for providers to join their offer notifications group
//        public async Task JoinProviderOffersGroup(string providerId)
//        {
//            await Groups.AddToGroupAsync(Context.ConnectionId, $"ProviderOffers_{providerId}");
//        }

//        // Method for customers to join their request offers group
//        public async Task JoinRequestOffersGroup(string requestId)
//        {
//            await Groups.AddToGroupAsync(Context.ConnectionId, $"RequestOffers_{requestId}");
//        }

//        // Method to notify about new offers
//        public async Task NotifyNewOffer(ServiceOfferResponseDto offer)
//        {
//            // Notify the customer who owns the request
//            await Clients.Group($"RequestOffers_{offer.ServiceRequestId}")
//                .SendAsync("NewOfferReceived", offer);
//        }

//        // Method to notify about offer acceptance
//        public async Task NotifyOfferAccepted(ServiceOfferResponseDto offer)
//        {
//            // Notify the provider whose offer was accepted
//            await Clients.Group($"ProviderOffers_{offer.ServiceProviderId}")
//                .SendAsync("YourOfferAccepted", offer);

//            // Notify all providers who submitted offers about this request
//            await Clients.Group($"RequestOffers_{offer.ServiceRequestId}")
//                .SendAsync("RequestOfferAccepted", offer);
//        }

//        // Method to notify about offer rejection
//        public async Task NotifyOfferRejected(ServiceOfferResponseDto offer)
//        {
//            // Notify the provider whose offer was rejected
//            await Clients.Group($"ProviderOffers_{offer.ServiceProviderId}")
//                .SendAsync("YourOfferRejected", offer);
//        }

//        // Method to notify about offer status updates
//        public async Task NotifyOfferStatusUpdate(ServiceOfferResponseDto offer)
//        {
//            // Notify the customer who owns the request
//            await Clients.Group($"RequestOffers_{offer.ServiceRequestId}")
//                .SendAsync("OfferStatusUpdated", offer);

//            // Notify the provider who made the offer
//            await Clients.Group($"ProviderOffers_{offer.ServiceProviderId}")
//                .SendAsync("YourOfferStatusUpdated", offer);
//        }

//        // Method to notify about offer expiration
//        public async Task NotifyOfferExpired(ServiceOfferResponseDto offer)
//        {
//            // Notify the provider whose offer expired
//            await Clients.Group($"ProviderOffers_{offer.ServiceProviderId}")
//                .SendAsync("YourOfferExpired", offer);

//            // Notify the customer about the expired offer
//            await Clients.Group($"RequestOffers_{offer.ServiceRequestId}")
//                .SendAsync("OfferExpired", offer);
//        }

//        public async Task NotifyProviderReached(Guid requestId, Guid customerId)
//        {
//            await Clients.Group($"Customer_{customerId}")
//                .SendAsync("ProviderReached", new
//                {
//                    RequestId = requestId,
//                    Status = "In_Progress",
//                    Message = "The service provider has reached your location."
//                });
//        }

//        public async Task NotifyProviderCompleted(Guid requestId, Guid customerId)
//        {
//            await Clients.Group($"Customer_{customerId}")
//                .SendAsync("ProviderCompleted", new
//                {
//                    RequestId = requestId,
//                    Status = "Completed",
//                    Message = "The service provider has completed the service."
//                });
//        }


//        public override async Task OnConnectedAsync()
//        {
//            await base.OnConnectedAsync();
//        }

//        public override async Task OnDisconnectedAsync(Exception exception)
//        {
//            await base.OnDisconnectedAsync(exception);
//        }




//    }
//}

using Microsoft.AspNetCore.SignalR;
using Home_Service_Finder.RequestServices.ServiceOffers.Dtos;
using System;
using System.Threading.Tasks;

namespace Home_Service_Finder.RequestServices
{
    public class ServiceOfferHub : Hub
    {
        // Clients call these to join their groups

        public async Task JoinProviderOffersGroup(string providerId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"ProviderOffers_{providerId}");
        }

        public async Task JoinRequestOffersGroup(string requestId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"RequestOffers_{requestId}");
        }

        // Server‐side notifications

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

        //public async Task NotifyProviderReached(Guid requestId, Guid customerId)
        //{
        //    await Clients.Group($"Customer_{customerId}")
        //                 .SendAsync("ProviderReached", new
        //                 {
        //                     RequestId = requestId,
        //                     Status = "In_Progress",
        //                     Message = "The service provider has reached your location."
        //                 });
        //}

        //public async Task NotifyProviderCompleted(Guid requestId, Guid customerId)
        //{
        //    await Clients.Group($"Customer_{customerId}")
        //                 .SendAsync("ProviderCompleted", new
        //                 {
        //                     RequestId = requestId,
        //                     Status = "Completed",
        //                     Message = "The service provider has completed the service."
        //                 });
        //}

        // New: Notify both provider and customer of a payment update
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
