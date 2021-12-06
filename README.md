# Demo for Kafka .NET
## Kafka
### Using kafka
- https://kafka.apache.org/quickstart
- Start the ZooKeeper service: bin/zookeeper-server-start.sh config/zookeeper.properties
- Start the Kafka broker service: bin/kafka-server-start.sh config/server.properties
- Create a topic: bin/kafka-topics.sh --create --topic topic_1 --bootstrap-server localhost:9092
- Write some events: bin/kafka-console-producer.sh --topic topic_1 --bootstrap-server localhost:9092
- Read some events: bin/kafka-console-consumer.sh --topic topic_1 --from-beginning --bootstrap-server localhost:9092
### Using docker
- Create topic: docker-compose exec kafka1 kafka-topics --create --topic topic_1 --bootstrap-server localhost:9092 --replication-factor 1 --partitions 1
- Write some events: docker-compose exec kafka1 kafka-console-producer --topic topic_1 --bootstrap-server localhost:9092
- Read the events: docker-compose exec kafka1 kafka-console-consumer --topic topic_1 --from-beginning --bootstrap-server localhost:9092
## Notes
- How do we open the connection to Kafka brokers? Creating a producer class instance with the recommended settings will maintain a connection with all the brokers in the cluster. You should generally avoid creating multiple Consumer or Producer instances in your application.
