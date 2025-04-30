using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Infrastructure.Redis
{
    public static class NotificationsChannel
    {
        public static RedisChannel PendingHighPriority => new RedisChannel("notifications:pending_high_priority", RedisChannel.PatternMode.Literal);
        public static RedisChannel PendingLowPriority => new RedisChannel("notifications:pending_low_priority", RedisChannel.PatternMode.Literal);
        public static RedisChannel Status => new RedisChannel("notifications:status", RedisChannel.PatternMode.Literal);
    }
}
