using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace Home_Service_Finder.Notifications
{
    public class NotificationHub : Hub
    {
        // Called when a user connects to the hub
        public override async Task OnConnectedAsync()
        {
            // Get the user ID from the context
            var userId = Context.User?.FindFirst("UserId")?.Value;

            if (!string.IsNullOrEmpty(userId))
            {
                // Add the user to a group with their user ID
                await Groups.AddToGroupAsync(Context.ConnectionId, userId);
            }

            await base.OnConnectedAsync();
        }

        // Called when a user disconnects from the hub
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userId = Context.User?.FindFirst("UserId")?.Value;

            if (!string.IsNullOrEmpty(userId))
            {
                // Remove the user from their group
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId);
            }

            await base.OnDisconnectedAsync(exception);
        }

        // Send notification to specific user
        public async Task SendNotificationToUser(string userId, NotificationDto notification)
        {
            await Clients.Group(userId).SendAsync("ReceiveNotification", notification);
        }
    }

    public class NotificationDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
        public NotificationType Type { get; set; }
        public string RelatedEntityId { get; set; }
    }

    public enum NotificationType
    {
        OfferAccepted,
        OfferRejected,
        OfferReceived,
        ServiceCompleted,
        PaymentReceived,
        SystemNotification
    }
}