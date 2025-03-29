using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using GrowthTracking.ShareLibrary.Logs;

namespace ParentManagementAPI.Application.Messaging
{
    public class ConsultationEvent : BackgroundService
    {
        private IConnection _connection;
        private IModel _channel;

        public ConsultationEvent(IConfiguration configuration)
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
            _channel.QueueDeclare(queue: "consultation.events", durable: false, exclusive: false, autoDelete: false, arguments: null);
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
                        _channel.QueueDeclare(queue: "consultation.events", durable: false, exclusive: false, autoDelete: false, arguments: null);
                    }

                    var consumer = new EventingBasicConsumer(_channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        var eventData = JsonSerializer.Deserialize<ConsultationEventData>(message);
                        if (eventData != null)
                        {
                            if (eventData.EventType == "ConsultationScheduled")
                            {
                                LogHandler.LogToConsole($"Received ConsultationScheduled event: ConsultationId={eventData.ConsultationId}, BookingId={eventData.BookingId}, DoctorId={eventData.DoctorId}");
                            }
                            else if (eventData.EventType == "ConsultationCancelled")
                            {
                                LogHandler.LogToConsole($"Received ConsultationCancelled event: ConsultationId={eventData.ConsultationId}, BookingId={eventData.BookingId}, DoctorId={eventData.DoctorId}");
                            }
                        }
                    };
                    _channel.BasicConsume(queue: "consultation.events", autoAck: true, consumer: consumer);
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

    public class ConsultationEventData
    {
        public Guid ConsultationId { get; set; }
        public Guid BookingId { get; set; }
        public Guid DoctorId { get; set; }
        public string EventType { get; set; }
    }
}