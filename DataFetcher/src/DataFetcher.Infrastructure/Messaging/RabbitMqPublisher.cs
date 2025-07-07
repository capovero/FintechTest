using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DataFetcher.Infrastructure.Messaging
{
    public class RabbitMqPublisher : IMessagePublisher
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public RabbitMqPublisher()
        {
            var factory = new ConnectionFactory
            {
                HostName = "rabbitmq",
                UserName = "guest",
                Password = "guest"
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "future_prices_queue", durable: true, exclusive: false, autoDelete: false, arguments: null);
        }

        public async Task PublishAsync<T>(string queueName, T message)
        {
            var body = JsonSerializer.SerializeToUtf8Bytes(message);
            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;

            _channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: properties, body: body);
            await Task.CompletedTask;
        }

        public void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
        }
    }
}