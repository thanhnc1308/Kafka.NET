using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Kafka.NET.Models;

namespace KafkaConsumer1
{
    public class KafkaConsumer : BackgroundService
    {
        private string _topic = "";
        private readonly ILogger<KafkaConsumer> _logger;
        private readonly IConsumer<string, string> kafkaConsumer;

        public KafkaConsumer(ILogger<KafkaConsumer> logger, IConfiguration config)
        {
            _logger = logger;
            var consumerConfig = new ConsumerConfig();
            config.GetSection("Kafka:ConsumerConfigs").Bind(consumerConfig);
            _topic = config.GetValue<string>("Kafka:Topic");
            System.Console.WriteLine($"Listen to topic {_topic}");
            kafkaConsumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
        }

        protected override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            new Thread(() => StartConsumerLoop(cancellationToken)).Start();
            return Task.CompletedTask;
        }

        private void StartConsumerLoop(CancellationToken cancellationToken)
        {
            kafkaConsumer.Subscribe(_topic);
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var cr = kafkaConsumer.Consume(cancellationToken);
                    var todoItem = JsonConvert.DeserializeObject<TodoItem>(cr.Message.Value);
                    for (int j = 0; j < 900000000; j++)
                    {
                        var a = 0;
                        a++;
                        a++;
                    }
                    // handle business logic
                    System.Console.WriteLine($"{todoItem.id} - {todoItem.name} - {todoItem.is_complete} - {todoItem.created_at}");
                }
                catch (ConsumeException e)
                {
                    // Consumer errors should generally be ignored (or logged) unless fatal.
                    System.Console.WriteLine($"Consume error: {e.Error.Reason}");
                    if (e.Error.IsFatal)
                    {
                        // https://github.com/edenhill/librdkafka/blob/master/INTRODUCTION.md#fatal-consumer-errors
                        break;
                    }
                }
                catch (Exception e)
                {
                    System.Console.WriteLine($"Unexpected error: {e}");
                    break;
                }
            }
        }

        public override void Dispose()
        {
            kafkaConsumer.Close(); // Commit offsets and leave the group cleanly.
            kafkaConsumer.Dispose();
            base.Dispose();
        }
    }
}
