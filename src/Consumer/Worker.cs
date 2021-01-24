using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Consumer
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConsumer<long, string> _consumer;
        private readonly ConsumerSettings _consumerSettings;

        public Worker(ILogger<Worker> logger, IConsumer<long, string> consumer, IOptions<ConsumerSettings> options)
        {
            this._logger = logger;
            this._consumer = consumer;
            this._consumerSettings = options.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var topic = _consumerSettings.Topic;
            _logger.LogInformation($"Subscribing on topic {topic}");

            var subscription = _consumer.Subscribe(topic);

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Waiting for message at: {0}", DateTime.Now);

                var message = await subscription.ConsumeAsync();
                _logger.LogInformation("Retrieved message: {0}", message.value);
            }

            _consumer.Unsubscribe(subscription);
        }
    }
}
