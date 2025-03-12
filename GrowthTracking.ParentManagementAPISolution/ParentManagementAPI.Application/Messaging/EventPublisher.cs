using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using GrowthTracking.ShareLibrary.Logs;

namespace ParentManageApi.Application.Messaging
{
    public class EventPublisher : IEventPublisher, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly int _retryInterval = 5000; // 5 giây

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
            PublishEvent(parentId, fullName, "ParentCreated");
        }

        public void PublishParentUpdated(Guid parentId, string fullName)
        {
            PublishEvent(parentId, fullName, "ParentUpdated");
        }

        private void PublishEvent(Guid parentId, string fullName, string eventType)
        {
            int retryCount = 0;
            const int maxRetries = 3;

            while (retryCount < maxRetries)
            {
                try
                {
                    LogHandler.LogToFile($"EventPublisher: Publishing {eventType} event for ParentId: {parentId}");
                    var message = new { ParentId = parentId, FullName = fullName, EventType = eventType };
                    var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
                    _channel.BasicPublish(exchange: "", routingKey: "parent.events", basicProperties: null, body: body);
                    LogHandler.LogToConsole($"EventPublisher: Successfully published {eventType} event for ParentId: {parentId}");
                    break;
                }
                catch (Exception ex)
                {
                    retryCount++;
                    LogHandler.LogExceptions(ex); // Gọi với 1 tham số Exception
                    LogHandler.LogToFile($"Retry {retryCount}/{maxRetries} for {eventType} event"); // Thêm message vào file log
                    if (retryCount == maxRetries)
                    {
                        LogHandler.LogToDebugger($"Failed to publish {eventType} event after {maxRetries} retries");
                        throw;
                    }
                    Thread.Sleep(_retryInterval);
                }
            }
        }

        public void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
            LogHandler.LogToConsole("EventPublisher: RabbitMQ connection and channel closed");
        }
    }
}