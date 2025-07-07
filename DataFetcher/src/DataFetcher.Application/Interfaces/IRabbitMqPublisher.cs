using DataFetcher.Domain.Entities;

namespace DataFetcher.Infrastructure.Messaging
{
    public interface IRabbitMqPublisher
    {
        Task PublishAsync(FuturePrice price);
    }
}