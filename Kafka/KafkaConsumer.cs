using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NCT.Kafka
{
    public class KafkaConsumer : IKafkaConsumer
    {
        IConsumer<string, string> _consumer;
        public KafkaConsumer(IConfiguration config)
        {
            var consumerConfig = new ConsumerConfig();
            config.GetSection("Kafka:ConsumerConfigs").Bind(consumerConfig);
            _consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
        }
        public Task SubscribeAsync(string topic, CancellationToken token, Action<string> message)
        {
            _consumer.Subscribe(topic);
            while (!token.IsCancellationRequested)
            {
                try
                {
                    var cr = _consumer.Consume(token);
                    message(cr.Message.Value);
                }
                catch (ConsumeException e)
                {
                    Console.WriteLine(e);
                }
            }
            return Task.CompletedTask;
        }
    }
}