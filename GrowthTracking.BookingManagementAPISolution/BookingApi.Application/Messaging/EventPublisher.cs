using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System;
using System.Text;
using System.Text.Json;
using GrowthTracking.ShareLibrary.Logs;
using BookingApi.Application.Interfaces;

namespace BookingApi.Application.Messaging
{
    public class EventPublisher : IEventPublisher, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly int _retryInterval = 5000; // 5 seconds

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
                _channel.QueueDeclare(queue: "booking.events", durable: false, exclusive: false, autoDelete: false, arguments: null);
                LogHandler.LogToConsole("EventPublisher: Successfully connected to RabbitMQ");
            }
            catch (Exception ex)
            {
                LogHandler.LogExceptions(ex);
                throw;
            }
        }

        public void PublishBookingCreated(Guid bookingId, Guid parentId, Guid childId, Guid doctorId)
        {
            PublishEvent(new { BookingId = bookingId, ParentId = parentId, ChildId = childId, DoctorId = doctorId, EventType = "BookingCreated" });
        }

        public void PublishConsultationScheduled(Guid consultationId, Guid bookingId, Guid doctorId)
        {
            PublishEvent(new { ConsultationId = consultationId, BookingId = bookingId, DoctorId = doctorId, EventType = "ConsultationScheduled" });
        }

        public void PublishBookingCancelled(Guid bookingId, Guid parentId, Guid childId, Guid doctorId)
        {
            PublishEvent(new { BookingId = bookingId, ParentId = parentId, ChildId = childId, DoctorId = doctorId, EventType = "BookingCancelled" });
        }

        public void PublishConsultationCancelled(Guid consultationId, Guid bookingId, Guid doctorId)
        {
            PublishEvent(new { ConsultationId = consultationId, BookingId = bookingId, DoctorId = doctorId, EventType = "ConsultationCancelled" });
        }

        private void PublishEvent(object message)
        {
            int retryCount = 0;
            const int maxRetries = 3;

            while (retryCount < maxRetries)
            {
                try
                {
                    LogHandler.LogToFile($"EventPublisher: Publishing event: {JsonSerializer.Serialize(message)}");
                    var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
                    _channel.BasicPublish(exchange: "", routingKey: "booking.events", basicProperties: null, body: body);
                    LogHandler.LogToConsole($"EventPublisher: Successfully published event: {JsonSerializer.Serialize(message)}");
                    break;
                }
                catch (Exception ex)
                {
                    retryCount++;
                    LogHandler.LogExceptions(ex);
                    LogHandler.LogToFile($"Retry {retryCount}/{maxRetries} for event publishing");
                    if (retryCount == maxRetries)
                    {
                        LogHandler.LogToDebugger($"Failed to publish event after {maxRetries} retries");
                        throw;
                    }
                    System.Threading.Thread.Sleep(_retryInterval);
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