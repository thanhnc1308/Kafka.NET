using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NCT.Kafka
{
    public interface IKafkaConsumer
    {
        Task SubscribeAsync(string topic, CancellationToken token, Action<string> message);
    }
}
