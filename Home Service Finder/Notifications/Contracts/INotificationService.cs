using Home_Service_Finder.Data.Contracts;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Home_Service_Finder.Notifications.Contracts
{
    public interface INotificationService
    {
        Task CreateAndSendNotificationAsync(Guid userId, string title, string message, NotificationType type, string relatedEntityId = null);
        Task<List<NotificationDto>> GetUserNotificationsAsync(Guid userId, bool includeRead = false);
        Task MarkNotificationAsReadAsync(Guid notificationId, Guid userId);
    }

}