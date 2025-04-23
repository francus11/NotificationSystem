using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Domain.Interfaces
{
    public interface INotificationRepository
    {
        Task AddNotificationAsync(Notification notification);
        Task AddNotificationAsync(Notification notification);
        Task<Notification> GetNotificationByIdAsync(int id);
        Task<IEnumerable<Notification>> GetAllNotificationsAsync();
        Task UpdateNotificationAsync(Notification notification);
        Task DeleteNotificationAsync(int id);
    }
}
