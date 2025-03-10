using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using GrowthTracking.ShareLibrary.Logs;

namespace ParentManageApi.Application.Messaging
{
    public class EventPublisher : IEventPublisher, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public EventPublisher(IConfiguration configuration)
        {
            var factory = new ConnectionFactory
            {
                HostName = configuration["RabbitMQ:HostName"] ?? "localhost",
                Port = int.Parse(configuration["RabbitMQ:Port"] ?? "5672"),
                UserName = configuration["RabbitMQ:UserName"] ?? "guest",
                Password = configuration["RabbitMQ:Password"] ?? "guest"
            };
            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();
                _channel.QueueDeclare(queue: "parent.events", durable: false, exclusive: false, autoDelete: false, arguments: null);
                LogHandler.LogToConsole("EventPublisher: Successfully connected to RabbitMQ");
            }
            catch (Exception ex)
            {
                LogHandler.LogExceptions(ex);
                throw;
            }
        }

        public void PublishParentCreated(Guid parentId, string fullName)
        {
            LogHandler.LogToFile($"EventPublisher: Publishing ParentCreated event for ParentId: {parentId}");
            var message = new { ParentId = parentId, FullName = fullName, EventType = "ParentCreated" };
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
            _channel.BasicPublish(exchange: "", routingKey: "parent.events", basicProperties: null, body: body);
            LogHandler.LogToConsole($"EventPublisher: Successfully published ParentCreated event for ParentId: {parentId}");
        }

        public void PublishParentUpdated(Guid parentId, string fullName)
        {
            LogHandler.LogToFile($"EventPublisher: Publishing ParentUpdated event for ParentId: {parentId}");
            var message = new { ParentId = parentId, FullName = fullName, EventType = "ParentUpdated" };
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
            _channel.BasicPublish(exchange: "", routingKey: "parent.events", basicProperties: null, body: body);
            LogHandler.LogToConsole($"EventPublisher: Successfully published ParentUpdated event for ParentId: {parentId}");
        }

        public void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
            LogHandler.LogToConsole("EventPublisher: RabbitMQ connection and channel closed");
        }
    }
}