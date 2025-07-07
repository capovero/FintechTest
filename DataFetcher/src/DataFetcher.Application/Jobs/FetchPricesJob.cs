using DataFetcher.Application.Interfaces;
using DataFetcher.Infrastructure.Messaging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using DataFetcher.Domain.Entities;

namespace DataFetcher.Application.Jobs
{
    public class FetchPricesJob
    {
        private readonly IFuturesFetcherService _futuresFetcher;
        private readonly IMessagePublisher _messagePublisher;
        private readonly IConfiguration _configuration;
        private readonly ILogger<FetchPricesJob> _logger;

        public FetchPricesJob(
            IFuturesFetcherService futuresFetcher,
            IMessagePublisher messagePublisher,
            IConfiguration configuration,
            ILogger<FetchPricesJob> logger)
        {
            _futuresFetcher = futuresFetcher;
            _messagePublisher = messagePublisher;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task ExecuteAsync()
        {
            var nearContract = _configuration["ContractCodes:Near"];
            var farContract = _configuration["ContractCodes:Far"];
            var nearPrice = await _futuresFetcher.GetPriceAsync(nearContract);
            var farPrice = await _futuresFetcher.GetPriceAsync(farContract);

            if (nearPrice.HasValue)
            {
                var nearMessage = new FuturePrice
                {
                    Symbol = "BTCUSDT",
                    ContractCode = nearContract,
                    Price = nearPrice.Value,
                    Timestamp = DateTime.UtcNow
                };
                await _messagePublisher.PublishAsync("future_prices_queue", nearMessage);
                _logger.LogInformation("Published price for {ContractCode}: {Price}", nearContract, nearPrice);
            }
            else
            {
                _logger.LogWarning("No price fetched for {ContractCode}", nearContract);
            }

            if (farPrice.HasValue)
            {
                var farMessage = new FuturePrice
                {
                    Symbol = "BTCUSDT",
                    ContractCode = farContract,
                    Price = farPrice.Value,
                    Timestamp = DateTime.UtcNow
                };
                await _messagePublisher.PublishAsync("future_prices_queue", farMessage);
                _logger.LogInformation("Published price for {ContractCode}: {Price}", farContract, farPrice);
            }
            else
            {
                _logger.LogWarning("No price fetched for {ContractCode}", farContract);
            }
        }
    }
}