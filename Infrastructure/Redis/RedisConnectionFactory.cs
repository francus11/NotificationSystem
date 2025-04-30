using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Infrastructure.Redis
{
    public static class RedisConnectionFactory
    {
        private static Lazy<ConnectionMultiplexer> lazyConnection = new(() =>
        {
            return ConnectionMultiplexer.Connect("localhost,abortConnect=false");
        });

        public static IConnectionMultiplexer Connection => lazyConnection.Value;
    }
}
