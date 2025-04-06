using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RedisWorker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<RedisSubscriberService>();
                });
    }

    public class RedisSubscriberService : BackgroundService
    {
        private readonly ConnectionMultiplexer _redis;

        public RedisSubscriberService()
        {
            _redis = ConnectionMultiplexer.Connect("red-cvm0paq4d50c73ftt2pg:6379");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var sub = _redis.GetSubscriber();
            await sub.SubscribeAsync("my_channel", (channel, message) =>
            {
                Console.WriteLine($"[Redis] Message received: {message}");
            });

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
