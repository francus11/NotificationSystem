using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Application.DTOs;
using Domain.Interfaces;
using Infrastructure.Redis;
using StackExchange.Redis;

namespace NotificationSchedulerService.Services
{
    public class StatusListenerWorker : BackgroundService
    {
        private readonly ILogger<StatusListenerWorker> _logger;
        private readonly IConnectionMultiplexer _redis;
        private readonly INotificationRepository _repository;

        public StatusListenerWorker(
            ILogger<StatusListenerWorker> logger,
            IConnectionMultiplexer redis,
            INotificationRepository repository)
        {
            _logger = logger;
            _redis = redis;
            _repository = repository;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var subscriber = _redis.GetSubscriber();

            // Nasłuchujemy na kanał Status
            await subscriber.SubscribeAsync(
                NotificationsChannel.Status,
                async (channel, value) =>
                {
                    var statusDto = JsonSerializer.Deserialize<NotificationStatusDto>(value!);
                    if (statusDto == null) return;

                    // Zaktualizuj status powiadomienia w bazie
                    var notification = await _repository.GetNotificationByIdAsync(statusDto.NotificationId);
                    if (notification != null)
                    {
                        notification.Status = statusDto.Status;
                        await _repository.UpdateNotificationAsync(notification);
                        _logger.LogInformation("Updated status for notification {NotificationId}: {Status}", statusDto.NotificationId, statusDto.Status);
                    }
                    else
                    {
                        _logger.LogWarning("Notification {NotificationId} not found in database", statusDto.NotificationId);
                    }
                });

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
    }
}
