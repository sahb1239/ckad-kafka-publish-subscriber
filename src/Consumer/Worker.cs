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
        private readonly IProducer<long, string> _producer;
        private readonly ConsumerSettings _consumerSettings;
        private readonly Random _random = new Random();

        public Worker(ILogger<Worker> logger, IConsumer<long, string> consumer, IProducer<long, string> producer, IOptions<ConsumerSettings> options)
        {
            this._logger = logger;
            this._consumer = consumer;
            _producer = producer;
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

                var produceTopic = _consumerSettings.ProduceTopic;

                var randomNumber = _random.Next(2);
                if (randomNumber == 0)
                {
                    _logger.LogInformation("Producing value {0} on topic {1}", message.value, produceTopic);
                    await _producer.ProduceAsync(produceTopic, message.value);
                }
                else
                {
                    _logger.LogInformation("Skipped producing value");
                }
            }

            _consumer.Unsubscribe(subscription);
        }
    }
}
