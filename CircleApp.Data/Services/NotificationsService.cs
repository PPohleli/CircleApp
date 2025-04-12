using CircleApp.Data.Helpers.Constants;
using CircleApp.Data.Hubs;
using CircleApp.Data.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircleApp.Data.Services
{ 
    public class NotificationsService : INotificationsService
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly AppDbContext _context;
        public NotificationsService(AppDbContext context, IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }
        public async Task AddNewNotificationAsync(int userId, string notificationType, string userFullName, int? postId)
        {
            var newNotification = new Notification()
            {
                UserId = userId,
                Message = GetPostMessage(notificationType, userFullName),
                Type = notificationType,
                IsRead = false,
                PostId = postId,
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow,
            };

            await _context.Notifications.AddAsync(newNotification);
            await _context.SaveChangesAsync();

            var notificationNumber = await GetUnreadNotificationsCountAsync(userId);
            await _hubContext.Clients.User(userId.ToString()).SendAsync("RecieveNotification", notificationNumber);
        }
        public async Task<int> GetUnreadNotificationsCountAsync(int userId)
        {
            var count = await _context.Notifications.Where(n => n.UserId == userId && !n.IsRead).CountAsync();
            return count;
        }
        public async Task<List<Notification>> GetNotificationsAsync(int userId)
        {
            var allNotifications = await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderBy(n => n.IsRead)
                .ThenByDescending(n => n.DateCreated)
                .ToListAsync();
            
            return allNotifications;
        }

        public async Task SetNotificationAsReaAsync(int notificationId)
        {
            var notificationDb = await _context.Notifications.FirstOrDefaultAsync(n => n.Id == notificationId);

            if (notificationDb != null)
            {
                notificationDb.DateUpdated = DateTime.UtcNow;
                notificationDb.IsRead = true;

                _context.Notifications.Update(notificationDb);
                await _context.SaveChangesAsync();
            }
        }

        private string GetPostMessage(string notificationType, string userFullName)
        {
            var message = "";

            switch(notificationType)
            {
                case NotificationType.Like: message = $"{userFullName} liked your post"; break;
                case NotificationType.Comment: message = $"{userFullName} commented on your post"; break;
                case NotificationType.Favorite:message = $"{userFullName} marked your post as favorite"; break;
                case NotificationType.FriendRequest: message = $"{userFullName} has sent you a friend request"; break;
                case NotificationType.FriendRequestApproved: message = $"{userFullName} has accepeted your friend request"; break;
                default: message = ""; break;
            }
            return message;
        }
    }
}
