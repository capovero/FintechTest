using System.Threading.Tasks;

namespace DataFetcher.Infrastructure.Messaging
{
    public interface IMessagePublisher
    {
        Task PublishAsync<T>(string queueName, T message);
    }
}