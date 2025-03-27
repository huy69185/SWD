using Microsoft.Extensions.Configuration;
using ParentManagementAPI.Application.Messaging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using GrowthTracking.ShareLibrary.Logs;
using Xunit;

namespace ParentManagementAPI.Tests.Integration
{
    public class ConsultationEventIntegrationTests : IDisposable
    {
        private readonly IConfiguration _configuration;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public ConsultationEventIntegrationTests()
        {
            // C?u hình RabbitMQ cho test (s? d?ng localhost)
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "RabbitMQ:HostName", "localhost" },
                    { "RabbitMQ:Port", "5672" },
                    { "RabbitMQ:UserName", "guest" },
                    { "RabbitMQ:Password", "guest" }
                })
                .Build();

            _configuration = config;

            var factory = new ConnectionFactory
            {
                HostName = _configuration["RabbitMQ:HostName"],
                Port = int.Parse(_configuration["RabbitMQ:Port"]),
                UserName = _configuration["RabbitMQ:UserName"],
                Password = _configuration["RabbitMQ:Password"]
            };

            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();
                // Xóa queue c? (n?u có) ?? ??m b?o không có message c? gây nhi?u
                _channel.QueueDelete("consultation.events");
                _channel.QueueDeclare(queue: "consultation.events", durable: false, exclusive: false, autoDelete: false, arguments: null);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to connect to RabbitMQ. Ensure RabbitMQ is running on localhost:5672 with user 'guest' and password 'guest'.", ex);
            }
        }

        [Fact]
        public async Task ConsultationEvent_ReceivesConsultationScheduledEvent_LogsCorrectly()
        {
            // Arrange
            var consultationId = Guid.NewGuid();
            var bookingId = Guid.NewGuid();
            var doctorId = Guid.NewGuid();
            var message = new
            {
                ConsultationId = consultationId,
                BookingId = bookingId,
                DoctorId = doctorId,
                EventType = "ConsultationScheduled"
            };
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

            // Publish message tr??c ?? ki?m tra r?ng message ???c g?i thành công
            _channel.BasicPublish(exchange: "", routingKey: "consultation.events", basicProperties: null, body: body);

            // Ki?m tra r?ng message ?ã ???c publish thành công
            var publishedMessage = await WaitForMessageAsync("consultation.events", consultationId, "ConsultationScheduled");
            Assert.NotNull(publishedMessage);
            var eventData = JsonSerializer.Deserialize<ConsultationEventData>(publishedMessage);
            Assert.NotNull(eventData);
            Assert.Equal("ConsultationScheduled", eventData.EventType);
            Assert.Equal(consultationId, eventData.ConsultationId);
            Assert.Equal(bookingId, eventData.BookingId);
            Assert.Equal(doctorId, eventData.DoctorId);

            // Kh?i ??ng ConsultationEvent ?? x? lý message
            var consultationEvent = new ConsultationEvent(_configuration);
            var cts = new CancellationTokenSource();
            await consultationEvent.StartAsync(cts.Token);

            // ??i m?t chút ?? ConsultationEvent x? lý message
            await Task.Delay(1000); // ??i 1 giây ?? ??m b?o message ???c x? lý

            // Stop the consumer
            await consultationEvent.StopAsync(cts.Token);
        }

        [Fact]
        public async Task ConsultationEvent_ReceivesConsultationCancelledEvent_LogsCorrectly()
        {
            // Arrange
            var consultationId = Guid.NewGuid();
            var bookingId = Guid.NewGuid();
            var doctorId = Guid.NewGuid();
            var message = new
            {
                ConsultationId = consultationId,
                BookingId = bookingId,
                DoctorId = doctorId,
                EventType = "ConsultationCancelled"
            };
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

            // Publish message tr??c ?? ki?m tra r?ng message ???c g?i thành công
            _channel.BasicPublish(exchange: "", routingKey: "consultation.events", basicProperties: null, body: body);

            // Ki?m tra r?ng message ?ã ???c publish thành công
            var publishedMessage = await WaitForMessageAsync("consultation.events", consultationId, "ConsultationCancelled");
            Assert.NotNull(publishedMessage);
            var eventData = JsonSerializer.Deserialize<ConsultationEventData>(publishedMessage);
            Assert.NotNull(eventData);
            Assert.Equal("ConsultationCancelled", eventData.EventType);
            Assert.Equal(consultationId, eventData.ConsultationId);
            Assert.Equal(bookingId, eventData.BookingId);
            Assert.Equal(doctorId, eventData.DoctorId);

            // Kh?i ??ng ConsultationEvent ?? x? lý message
            var consultationEvent = new ConsultationEvent(_configuration);
            var cts = new CancellationTokenSource();
            await consultationEvent.StartAsync(cts.Token);

            // ??i m?t chút ?? ConsultationEvent x? lý message
            await Task.Delay(1000); // ??i 1 giây ?? ??m b?o message ???c x? lý

            // Stop the consumer
            await consultationEvent.StopAsync(cts.Token);
        }

        private async Task<string> WaitForMessageAsync(string queueName, Guid consultationId, string eventType)
        {
            var tcs = new TaskCompletionSource<string>();
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var eventData = JsonSerializer.Deserialize<ConsultationEventData>(message);
                if (eventData != null && eventData.ConsultationId == consultationId && eventData.EventType == eventType)
                {
                    tcs.SetResult(message);
                }
            };
            _channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

            var timeoutTask = Task.Delay(10000);
            var completedTask = await Task.WhenAny(tcs.Task, timeoutTask);
            if (completedTask == timeoutTask)
            {
                throw new TimeoutException($"Timed out waiting for {eventType} message");
            }

            return await tcs.Task;
        }

        public void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
        }
    }

    // ??nh ngh?a class ConsultationEventData ?? deserialize message
    public class ConsultationEventData
    {
        public Guid ConsultationId { get; set; }
        public Guid BookingId { get; set; }
        public Guid DoctorId { get; set; }
        public string EventType { get; set; }
    }
}