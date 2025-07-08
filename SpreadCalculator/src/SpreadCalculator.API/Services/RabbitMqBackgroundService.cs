using System;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SpreadCalculator.Domain.Entities;
using SpreadCalculator.Domain.Interfaces;
using SpreadCalculator.Infrastructure;
using SpreadCalculator.Infrastructure.Configurations;

namespace SpreadCalculator.API.Services
{
    public class RabbitMqBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<RabbitMqBackgroundService> _logger;
        private IConnection? _connection;
        private IModel? _channel;
        
        private readonly ConcurrentDictionary<string, FuturePrice[]> _priceCache = new();

        private const string QueueName = "future_prices_queue";

        public RabbitMqBackgroundService(IServiceProvider serviceProvider, ILogger<RabbitMqBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            var factory = new ConnectionFactory
            {
                HostName = "rabbitmq",
                UserName = "guest",
                Password = "guest",
                DispatchConsumersAsync = true
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

            _logger.LogInformation("RabbitMQ connection established.");

            return base.StartAsync(cancellationToken);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel!);
            consumer.Received += async (_, ea) =>
            {
                var body = ea.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);
                _logger.LogInformation(">>> Received raw message: {Json}", json);

                try
                {
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var price = JsonSerializer.Deserialize<FuturePrice>(json, options);
                    if (price != null)
                        await ProcessMessageAsync(price);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error deserializing or processing message");
                }

                _channel!.BasicAck(ea.DeliveryTag, multiple: false);
            };

            _channel!.BasicConsume(queue: QueueName, autoAck: false, consumer: consumer);

            return Task.CompletedTask;
        }

        private async Task ProcessMessageAsync(FuturePrice price)
        {
            _logger.LogInformation(">>> Caching price: {ContractCode} = {Price}", price.ContractCode, price.Price);

            var pair = _priceCache.GetOrAdd(price.Symbol, _ => new FuturePrice[2]);

            if (pair[0] == null)
            {
                pair[0] = price; return;
            }
            if (pair[1] == null)
            {
                pair[1] = price;
            }
            else
            {
                pair[0] = price;
                pair[1] = null;
                return;
            }

            if (pair[0] != null && pair[1] != null)
            {
                var near = pair[0]!;
                var far  = pair[1]!;

                var spreadValue = far.Price - near.Price;
                _logger.LogInformation(">>> Calculated spread: Near={Near}, Far={Far}, Spread={Spread}",
                    near.Price, far.Price, spreadValue);

                var result = new SpreadResult
                {
                    Timestamp = DateTime.UtcNow,
                    NearPrice = near.Price,
                    FarPrice  = far.Price
                };

                using var scope = _serviceProvider.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                _logger.LogInformation(">>> Saving SpreadResult to DB...");
                await db.SpreadResults.AddAsync(result);
                await db.SaveChangesAsync();
                _logger.LogInformation(">>> Saved SpreadResult [Id={Id}]", result.Id);

                _priceCache.TryRemove(price.Symbol, out _);
            }
        }

        public override void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
            base.Dispose();
        }
    }
}
