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
        private readonly ProducerSettings _producerSettings;

        public Worker(ILogger<Worker> logger, IProducer<long, string> producer, IOptions<ProducerSettings> producerSettings)
        {
            _logger = logger;
            _producer = producer;
            _producerSettings = producerSettings.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var topic = _producerSettings.Topic;
            var value = _producerSettings.Value;

            _logger.LogInformation("Producing value {1} on topic {0}", topic, value);

            await _producer.ProduceAsync(topic, value);
        }
    }
}
