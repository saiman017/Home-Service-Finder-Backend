//using Home_Service_Finder.Data.Contracts;
//using Home_Service_Finder.Notifications.Contracts;
//using Microsoft.AspNetCore.SignalR;

//namespace Home_Service_Finder.Notifications
//{
//    public class NotificationService : INotificationService
//    {
//        private readonly IUnitOfWork _dbContext;
//        private readonly IHubContext<NotificationHub> _notificationHub;

//        public NotificationService(
//            IUnitOfWork dbContext,
//            IHubContext<NotificationHub> notificationHub)
//        {
//            _dbContext = dbContext;
//            _notificationHub = notificationHub;
   
//            _emailService = emailService;
//        }

//        public async Task CreateAndSendNotificationAsync(Guid userId, string title, string message, NotificationType type, string relatedEntityId = null)
//        {
//            // 1. Create the database notification
//            var notification = new Notification
//            {
//                Id = Guid.NewGuid(),
//                UserId = userId,
//                Title = title,
//                Message = message,
//                CreatedAt = DateTime.UtcNow,
//                IsRead = false,
//                Type = type.ToString(),
//                RelatedEntityId = relatedEntityId
//            };

//            await _dbContext.Notifications.AddAsync(notification);
//            await _dbContext.SaveChangesAsync();

//            // 2. Create the DTO to send via SignalR
//            var notificationDto = new NotificationDto
//            {
//                Id = notification.Id,
//                Title = notification.Title,
//                Message = notification.Message,
//                CreatedAt = notification.CreatedAt,
//                Type = type,
//                RelatedEntityId = relatedEntityId
//            };

//            // 3. Send real-time notification via SignalR
//            await _notificationHub.Clients.Group(userId.ToString())
//                .SendAsync("ReceiveNotification", notificationDto);

//            // 4. Send push notification to mobile devices
//            await _pushNotificationService.SendPushNotificationAsync(
//                userId,
//                title,
//                message,
//                new Dictionary<string, string>
//                {
//                    { "notificationType", type.ToString() },
//                    { "relatedEntityId", relatedEntityId ?? string.Empty }
//                }
//            );

          
//        }

//        public async Task<List<NotificationDto>> GetUserNotificationsAsync(Guid userId, bool includeRead = false)
//        {
//            var notifications = (await _dbContext.Notifications.GetAllAsync())
//                .Where(n => n.UserId == userId)
//                .Where(n => includeRead || !n.IsRead)
//                .OrderByDescending(n => n.CreatedAt)
//                .Take(50) // Limit to most recent 50
//                .ToList();

//            return notifications.Select(n => new NotificationDto
//            {
//                Id = n.Id,
//                Title = n.Title,
//                Message = n.Message,
//                CreatedAt = n.CreatedAt,
//                Type = Enum.Parse<NotificationType>(n.Type),
//                RelatedEntityId = n.RelatedEntityId
//            }).ToList();
//        }

//        public async Task MarkNotificationAsReadAsync(Guid notificationId, Guid userId)
//        {
//            var notification = await _dbContext.Notifications.GetByIdAsync(notificationId);

//            if (notification != null && notification.UserId == userId)
//            {
//                notification.IsRead = true;
//                await _dbContext.SaveChangesAsync();
//            }
//        }
//    }
//}
