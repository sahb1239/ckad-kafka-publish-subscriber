using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared;
using System.Threading;
using System.Threading.Tasks;

namespace Producer
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IProducer<long, string> _producer;
        private readonly IHostApplicationLifetime hostApplicationLifetime;
        private readonly ProducerSettings _producerSettings;

        public Worker(ILogger<Worker> logger, IProducer<long, string> producer, IOptions<ProducerSettings> producerSettings, IHostApplicationLifetime hostApplicationLifetime)
        {
            _logger = logger;
            _producer = producer;
            this.hostApplicationLifetime = hostApplicationLifetime;
            _producerSettings = producerSettings.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var topic = _producerSettings.Topic;
            var value = _producerSettings.Value;

            for (int i = 0; i < 4000; i++)
            {
                _logger.LogInformation("Producing value {0} on topic {1}", value, topic);

                await _producer.ProduceAsync(topic, value);

                await Task.Delay(10);
            }

            hostApplicationLifetime.StopApplication();
        }
    }
}
