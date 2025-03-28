using AuthenticationApi.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace AuthenticationApi.Application.Messaging
{
    public class ParentEventConsumer : IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<ParentEventConsumer> _logger;

        public ParentEventConsumer(IConfiguration configuration, IServiceScopeFactory scopeFactory, ILogger<ParentEventConsumer> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;

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
                _logger.LogInformation("Successfully connected to RabbitMQ and declared queue 'parent.events'.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to connect to RabbitMQ. The consumer will not start.");
                throw;
            }
        }

        public void StartConsuming()
        {
            if (_channel == null || !_channel.IsOpen)
            {
                _logger.LogWarning("RabbitMQ channel is not open. Cannot start consuming.");
                return;
            }

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var eventData = JsonSerializer.Deserialize<ParentEvent>(message);

                if (eventData != null)
                {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
                        switch (eventData.EventType)
                        {
                            case "ParentCreated":
                            case "ParentUpdated":
                                await userRepository.UpdateUser(eventData.ParentId, eventData.FullName);
                                break;
                        }
                    }
                }
                _channel.BasicAck(ea.DeliveryTag, false);
            };
            _channel.BasicConsume(queue: "parent.events", autoAck: false, consumer: consumer);
            _logger.LogInformation("Started consuming messages from queue 'parent.events'.");
        }

        public void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
            _logger.LogInformation("RabbitMQ connection and channel closed.");
        }

        private class ParentEvent
        {
            public Guid ParentId { get; set; }
            public string FullName { get; set; }
            public string EventType { get; set; }
        }
    }
}