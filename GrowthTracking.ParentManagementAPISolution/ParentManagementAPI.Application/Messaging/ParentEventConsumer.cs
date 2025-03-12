using GrowthTracking.ShareLibrary.Logs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace ParentManagementAPI.Application.Messaging
{
    // Consumer chạy nền để nhận các event từ Child API
    public class ParentEventConsumer : BackgroundService
    {
        private IConnection _connection;
        private IModel _channel;

        public ParentEventConsumer(IConfiguration configuration)
        {
            var factory = new ConnectionFactory
            {
                HostName = configuration["RabbitMQ:HostName"] ?? "localhost",
                Port = int.Parse(configuration["RabbitMQ:Port"] ?? "5672"),
                UserName = configuration["RabbitMQ:UserName"] ?? "guest",
                Password = configuration["RabbitMQ:Password"] ?? "guest"
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "child.events", durable: false, exclusive: false, autoDelete: false, arguments: null);
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
                        _channel.QueueDeclare(queue: "child.events", durable: false, exclusive: false, autoDelete: false, arguments: null);
                    }

                    var consumer = new EventingBasicConsumer(_channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        var eventData = JsonSerializer.Deserialize<ChildEvent>(message);
                        if (eventData != null && eventData.EventType == "ChildCreated")
                        {
                            LogHandler.LogToConsole($"Received ChildCreated event: ChildId={eventData.ChildId}, ParentId={eventData.ParentId}");
                        }
                    };
                    _channel.BasicConsume(queue: "child.events", autoAck: true, consumer: consumer);
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

    // Lớp đại diện cho cấu trúc event từ Child API
    public class ChildEvent
    {
        public Guid ChildId { get; set; }
        public Guid ParentId { get; set; }
        public string FullName { get; set; }
        public string EventType { get; set; }
    }
}
