using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;
using Home_Service_Finder.RequestServices.ServiceRequest.Dtos;
namespace Home_Service_Finder.RequestServices
{
    public class ServiceRequestHub : Hub
    {

        public async Task JoinCategoryGroup(string categoryId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Category_{categoryId}");
        }

        public async Task LeaveCategoryGroup(string categoryId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Category_{categoryId}");
        }

        public async Task JoinProviderGroup(string providerId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Provider_{providerId}");
        }

        public async Task JoinCustomerGroup(string customerId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Customer_{customerId}");
        }

        public async Task NotifyNewRequest(ServiceRequestResponseDto request)
        {
            await Clients.Group($"Category_{request.ServiceCategoryId}").SendAsync("NewRequestCreated", request);
        }

        public async Task NotifyStatusUpdate(ServiceRequestResponseDto request)
        {
            await Clients.Group($"Category_{request.ServiceCategoryId}")
                .SendAsync("RequestStatusUpdated", request);

            await Clients.Group($"Customer_{request.CustomerId}")
                .SendAsync("YourRequestStatusUpdated", request);
        }

        public async Task NotifyCancellation(Guid requestId, string categoryId, Guid customerId, Guid? providerId = null)
        {
            await Clients.Group($"Category_{categoryId}")
                .SendAsync("RequestCancelled", new
                {
                    RequestId = requestId,
                    CategoryId = categoryId,
                    Status = "CANCELLED",
                    CancelledAt = DateTime.UtcNow
                });

            await Clients.Group($"Customer_{customerId}")
                .SendAsync("RequestCancelled", new
                {
                    RequestId = requestId,
                    Status = "CANCELLED",
                    CancelledAt = DateTime.UtcNow,
                    Message = "Your service request has been cancelled"
                });

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