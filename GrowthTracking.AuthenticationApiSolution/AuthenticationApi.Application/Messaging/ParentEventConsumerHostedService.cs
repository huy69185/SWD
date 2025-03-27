using AuthenticationApi.Application.Messaging;
using Microsoft.Extensions.Hosting;

namespace AuthenticationApi.Application.Services
{
    public class ParentEventConsumerHostedService : IHostedService
    {
        private readonly ParentEventConsumer _consumer;

        public ParentEventConsumerHostedService(ParentEventConsumer consumer)
        {
            _consumer = consumer;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _consumer.StartConsuming();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _consumer.Dispose();
            return Task.CompletedTask;
        }
    }
}