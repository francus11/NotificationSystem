using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Domain.Enums;
using Domain.Interfaces;
using Infrastructure.DbContexts;
using Infrastructure.Redis;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace NotificationSchedulerService.Services
{
    public class SchedulerWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<SchedulerWorker> _logger;
        private readonly IConnectionMultiplexer _redis;

        public SchedulerWorker(IServiceScopeFactory scopeFactory, ILogger<SchedulerWorker> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            _redis = RedisConnectionFactory.Connection;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();
                var repository = scope.ServiceProvider.GetRequiredService<INotificationRepository>();

                var now = DateTime.UtcNow;
                var dueNotifications = await repository.GetDueNotificationsAsync(now);

                if (dueNotifications.Any())
                {
                    var db = _redis.GetDatabase();
                    foreach (var notification in dueNotifications)
                    {
                        var json = JsonSerializer.Serialize(notification);
                        if(notification.Priority == SendPriority.High)
                        {
                            await db.PublishAsync(NotificationsChannel.PendingHighPriority, json);
                        }
                        else
                        {
                            await db.PublishAsync(NotificationsChannel.PendingLowPriority, json);
                        }
                    }
                    await repository.MarkAsSentAsync(dueNotifications);
                    _logger.LogInformation("Published {count} notifications", dueNotifications.Count());
                }

                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }
    }
}
