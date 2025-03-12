using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Connections;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ChildApi.Application.Messaging
{
    // Consumer chạy nền để nhận các event từ RabbitMQ và cập nhật ParentId vào cache.
    public class ParentEventConsumer : BackgroundService
    {
        private readonly IConnection _connection;
        private readonly RabbitMQ.Client.IModel _channel;
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
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                try
                {
                    // Giả sử message có cấu trúc: { "ParentId": "guid", "FullName": "xxx", "EventType": "ParentCreated" }
                    var eventData = JsonSerializer.Deserialize<ParentEvent>(message);
                    if (eventData != null && (eventData.EventType == "ParentCreated" || eventData.EventType == "ParentUpdated"))
                    {
                        _parentIdCache.ParentId = eventData.ParentId;
                    }
                }
                catch (Exception)
                {
                    // Log hoặc xử lý lỗi nếu cần
                }
            };
            _channel.BasicConsume(queue: "parent.events", autoAck: true, consumer: consumer);
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
