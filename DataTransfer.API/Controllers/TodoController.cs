using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Confluent.Kafka;
using Kafka.NET.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NCT.Kafka;
using Newtonsoft.Json;

namespace DataTransfer.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TodoController : ControllerBase
    {
        private readonly ILogger<TodoController> _logger;
        private readonly KafkaProducer<string, string> _producer;
        private string _topic;

        public TodoController(KafkaProducer<string, string> producer, IConfiguration config, ILogger<TodoController> logger)
        {
            _topic = config.GetValue<string>("Kafka:Topic");
            _producer = producer;
            _logger = logger;
        }

        [HttpGet("async")]
        public async Task<string> GetAsync()
        {
            System.Console.WriteLine($"Write to topic {_topic}");
            try
            {
                var todoItem = new TodoItem();
                var mesage = JsonConvert.SerializeObject(todoItem);
                var res = await _producer.ProduceAsync(
                    _topic,
                    new Message<string, string>
                    {
                        Value = mesage
                    }
                );
                System.Console.WriteLine(JsonConvert.SerializeObject(res));
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine(e);
            }
            finally
            {
                var queueSize = _producer.Flush(TimeSpan.FromSeconds(5));
                if (queueSize > 0)
                {
                    Console.WriteLine("WARNING: Producer event queue has " + queueSize + " pending events on exit.");
                }
                _producer.Dispose();
            }
            return "async";
        }

        [HttpGet("sync")]
        public string GetSync()
        {
            System.Console.WriteLine($"Write to topic {_topic}");
            try
            {
                var todoItem = new TodoItem();
                var mesage = JsonConvert.SerializeObject(todoItem);
                // For higher throughput, use the non-blocking Produce call
                // and handle delivery reports out-of-band, instead of awaiting
                // the result of a ProduceAsync call.
                _producer.Produce(
                    _topic,
                    new Message<string, string> {
                        Value = mesage
                    },
                    (deliveryReport) => {
                        if (deliveryReport.Error.Code != ErrorCode.NoError)
                        {
                            Console.WriteLine($"Failed to deliver message: {deliveryReport.Error.Reason}");
                        }
                        else
                        {
                            System.Console.WriteLine(JsonConvert.SerializeObject(deliveryReport));
                            // Console.WriteLine($"Produced message to: {deliveryReport.TopicPartitionOffset}");
                        }
                    }
                );
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine(e);
            }
            finally
            {
                var queueSize = _producer.Flush(TimeSpan.FromSeconds(5));
                if (queueSize > 0)
                {
                    Console.WriteLine("WARNING: Producer event queue has " + queueSize + " pending events on exit.");
                }
                _producer.Dispose();
            }
            return "sync";
        }

        [HttpPost]
        public string Post([FromBody] List<TodoItem> Items)
        {
            System.Console.WriteLine($"Write to topic {_topic}");
            try
            {
                var mesage = JsonConvert.SerializeObject(Items);
                // For higher throughput, use the non-blocking Produce call
                // and handle delivery reports out-of-band, instead of awaiting
                // the result of a ProduceAsync call.
                _producer.Produce(
                    _topic,
                    new Message<string, string> {
                        Value = mesage
                    },
                    (deliveryReport) => {
                        if (deliveryReport.Error.Code != ErrorCode.NoError)
                        {
                            Console.WriteLine($"Failed to deliver message: {deliveryReport.Error.Reason}");
                        }
                        else
                        {
                            Console.WriteLine($"Produced message to: {deliveryReport.TopicPartitionOffset}");
                        }
                    }
                );
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine(e);
            }
            finally
            {
                var queueSize = _producer.Flush(TimeSpan.FromSeconds(5));
                if (queueSize > 0)
                {
                    Console.WriteLine("WARNING: Producer event queue has " + queueSize + " pending events on exit.");
                }
                _producer.Dispose();
            }
            return "ok";
        }
    }
}
