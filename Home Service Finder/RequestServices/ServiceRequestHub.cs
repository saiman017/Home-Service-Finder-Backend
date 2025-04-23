using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;
using Home_Service_Finder.RequestServices.ServiceRequest.Dtos;
namespace Home_Service_Finder.RequestServices
{
    public class ServiceRequestHub : Hub
    {
        // Method to join a category group to receive updates for that category
        public async Task JoinCategoryGroup(string categoryId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Category_{categoryId}");
        }

        // Method to leave a category group
        public async Task LeaveCategoryGroup(string categoryId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Category_{categoryId}");
        }

        // Method for providers to join their own provider group
        public async Task JoinProviderGroup(string providerId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Provider_{providerId}");
        }

        // Method for customers to join their personal notification group
        public async Task JoinCustomerGroup(string customerId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Customer_{customerId}");
        }

        // Method to notify about new pending requests
        public async Task NotifyNewRequest(ServiceRequestResponseDto request)
        {
            await Clients.Group($"Category_{request.ServiceCategoryId}").SendAsync("NewRequestCreated", request);
        }

        // Method to notify about status updates
        public async Task NotifyStatusUpdate(ServiceRequestResponseDto request)
        {
            // Notify category subscribers about the status change
            await Clients.Group($"Category_{request.ServiceCategoryId}")
                .SendAsync("RequestStatusUpdated", request);

            // Also notify the customer
            await Clients.Group($"Customer_{request.CustomerId}")
                .SendAsync("YourRequestStatusUpdated", request);
        }

        // Method to notify customers about cancellations
        public async Task NotifyCancellation(Guid requestId, string categoryId, Guid customerId, Guid? providerId = null)
        {
            // Notify the category group (all providers in this category)
            await Clients.Group($"Category_{categoryId}")
                .SendAsync("RequestCancelled", new
                {
                    RequestId = requestId,
                    CategoryId = categoryId,
                    Status = "CANCELLED",
                    CancelledAt = DateTime.UtcNow
                });

            // Notify the customer
            await Clients.Group($"Customer_{customerId}")
                .SendAsync("RequestCancelled", new
                {
                    RequestId = requestId,
                    Status = "CANCELLED",
                    CancelledAt = DateTime.UtcNow,
                    Message = "Your service request has been cancelled"
                });

            // Notify provider if the request was assigned
            if (providerId.HasValue)
            {
                await Clients.Group($"Provider_{providerId}")
                    .SendAsync("RequestCancelled", new
                    {
                        RequestId = requestId,
                        Status = "CANCELLED",
                        CancelledAt = DateTime.UtcNow,
                        Message = "An assigned service request has been cancelled"
                    });
            }
        }

        // Method to notify providers about cancellations when request was already accepted
        public async Task NotifyProviderCancellation(Guid requestId, Guid providerId, Guid customerId)
        {
            await Clients.Group($"Provider_{providerId}")
                .SendAsync("ProviderRequestCancelled", new
                {
                    RequestId = requestId,
                    CustomerId = customerId,
                    CancelledAt = DateTime.UtcNow,
                    Message = "A service request assigned to you has been cancelled"
                });
        }

        // Override for connection handling
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            // Cleanup logic if needed
            await base.OnDisconnectedAsync(exception);
        }
    }
}