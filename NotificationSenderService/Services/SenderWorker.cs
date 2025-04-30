using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Application.DTOs;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Redis;
using Prometheus;
using StackExchange.Redis;

namespace NotificationSenderService.Services
{
    public class SenderWorker :BackgroundService
    {
        private readonly ILogger<SenderWorker> _logger;
        private readonly IConnectionMultiplexer _redis;
        private readonly RedisChannel[] _channels;
        private readonly string _instanceName;

        private readonly Counter _notificationsDeliveredCounter;
        private readonly Counter _notificationsFailedCounter;

        public SenderWorker(ILogger<SenderWorker> logger, string instanceName)
        {
            _logger = logger;
            _redis = RedisConnectionFactory.Connection;
            _channels =
            [
            NotificationsChannel.PendingHighPriority,
            NotificationsChannel.PendingLowPriority
            ];
            _instanceName = instanceName;

            _notificationsDeliveredCounter = Metrics.CreateCounter(
                $"notifications_delivered_total_{_instanceName}",
                "Total number of notifications delivered",
                new CounterConfiguration
                {
                    LabelNames = new[] { "instance" }
                });
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var subscriber = _redis.GetSubscriber();
            var random = new Random();

            foreach (var channel in _channels)
            {
                await subscriber.SubscribeAsync(channel, async (ch, value) =>
                {
                    var notification = JsonSerializer.Deserialize<Notification>(value!);
                    if (notification is null) return;

                    await Task.Delay(500);

                    if (random.NextDouble() > 0.5)
                    {
                        _logger.LogInformation("[Priority: {priority}] Sending to {recipient}: {message}", ch, notification.Recipient, notification.Message);

                        var status = new NotificationStatusDto
                        {
                            NotificationId = notification.Id,
                            Status = SendStatus.Delivered
                        };

                        var db = _redis.GetDatabase();
                        var statusJson = JsonSerializer.Serialize(status);
                        await db.PublishAsync(NotificationsChannel.Status, statusJson);
                    }
                    else
                    {
                        _logger.LogWarning("[Priority: {priority}] Notification to {recipient} was not sent", ch, notification.Recipient);
                        var status = new NotificationStatusDto
                        {
                            NotificationId = notification.Id,
                            Status = SendStatus.Failed
                        };

                        var db = _redis.GetDatabase();
                        var statusJson = JsonSerializer.Serialize(status);
                        await db.PublishAsync(NotificationsChannel.Status, statusJson);
                    }
                });
            }

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
    }
}
