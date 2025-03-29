using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using GrowthTracking.ShareLibrary.Logs;

namespace ChildApi.Application.Messaging
{
    // Consumer chạy nền để nhận các event từ RabbitMQ và cập nhật ParentId vào cache.
    public class ParentEventConsumer : BackgroundService
    {
        private IConnection _connection;
        private IModel _channel;
        private readonly ParentIdCache _parentIdCache;

        public ParentEventConsumer(IConfiguration configuration, ParentIdCache parentIdCache)
        {
            _parentIdCache = parentIdCache;
            var factory = new ConnectionFactory
            {
                HostName = configuration["RabbitMQ:HostName"] ?? "localhost",
                Port = int.Parse(configuration["RabbitMQ:Port"] ?? "5672"),
                UserName = configuration["RabbitMQ:UserName"] ?? "guest",
                Password = configuration["RabbitMQ:Password"] ?? "guest"
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "parent.events", durable: false, exclusive: false, autoDelete: false, arguments: null);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if (!_connection.IsOpen)
                    {
                        _connection.Dispose();
                        var factory = new ConnectionFactory
                        {
                            HostName = "localhost",
                            Port = 5672,
                            UserName = "guest",
                            Password = "guest"
                        };
                        _connection = factory.CreateConnection();
                        _channel = _connection.CreateModel();
                        _channel.QueueDeclare(queue: "parent.events", durable: false, exclusive: false, autoDelete: false, arguments: null);
                    }

                    var consumer = new EventingBasicConsumer(_channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        var eventData = JsonSerializer.Deserialize<ParentEvent>(message);
                        if (eventData != null && (eventData.EventType == "ParentCreated" || eventData.EventType == "ParentUpdated"))
                        {
                            _parentIdCache.ParentId = eventData.ParentId;
                            LogHandler.LogToConsole($"Updated ParentId to {eventData.ParentId}");
                        }
                    };
                    _channel.BasicConsume(queue: "parent.events", autoAck: true, consumer: consumer);
                    break;
                }
                catch (Exception ex)
                {
                    LogHandler.LogExceptions(ex);
                    Thread.Sleep(5000); // Retry sau 5 giây
                }
            }
            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
            base.Dispose();
        }
    }

    // Lớp đại diện cho cấu trúc của event nhận được từ Parent API.
    public class ParentEvent
    {
        public Guid ParentId { get; set; }
        public string FullName { get; set; }
        public string EventType { get; set; }
    }
}