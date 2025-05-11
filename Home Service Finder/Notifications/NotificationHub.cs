using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace Home_Service_Finder.Notifications
{
    public class NotificationHub : Hub
    {
        
        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst("UserId")?.Value;

            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, userId);
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userId = Context.User?.FindFirst("UserId")?.Value;

            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId);
            }

            await base.OnDisconnectedAsync(exception);
        }

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