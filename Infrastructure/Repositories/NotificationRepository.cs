using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly AppDbContext _context;

        public NotificationRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddNotificationAsync(Notification notification)
        {
            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteNotificationAsync(int id)
        {
            await _context.Notifications
                .Where(n => n.Id == id)
                .ExecuteDeleteAsync();
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Notification>> GetAllNotificationsAsync()
        {
            return await _context.Notifications.ToListAsync();
        }

        public async Task<IEnumerable<Notification>> GetDueNotificationsAsync(DateTime dueDate)
        {
            return await _context.Notifications.Where(n => n.Status == SendStatus.Pending 
            && n.ScheduledAt <= dueDate + n.ScheduledAt.Offset
            && (dueDate + n.ScheduledAt.Offset).TimeOfDay > TimeSpan.FromHours(8)
            && (dueDate + n.ScheduledAt.Offset).TimeOfDay < TimeSpan.FromHours(21)
            ).ToListAsync();
        }

        public async Task<Notification> GetNotificationByIdAsync(int id)
        {
            return await _context.Notifications.FindAsync(id);
        }

        public async Task MarkAsSentAsync(IEnumerable<Notification> notifications)
        {
            foreach (var notification in notifications)
            {
                notification.Status = SendStatus.Sent;
                _context.Notifications.Update(notification);
            }
            await _context.SaveChangesAsync();
        }

        public async Task UpdateNotificationAsync(Notification notification)
        {
            _context.Notifications.Update(notification);
            await _context.SaveChangesAsync();
        }
    }
}
