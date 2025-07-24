using Confluent.Kafka;
using Confluent.Kafka.Admin;

namespace Click_Go.Kafka
{
    public class KafkaTopicCreator
    {
        public async Task CreateTopicAsync()
        {
            var config = new AdminClientConfig
            {
                BootstrapServers = "localhost:9092" // sửa theo config của bạn
            };

            using (var adminClient = new AdminClientBuilder(config).Build())
            {
                try
                {
                    string topicName = "search-queries";
                    int numPartitions = 3; // số partition tùy bạn
                    short replicationFactor = 1; // nếu chỉ có 1 broker thì để 1

                    var topicSpec = new TopicSpecification
                    {
                        Name = topicName,
                        NumPartitions = numPartitions,
                        ReplicationFactor = replicationFactor
                    };

                    await adminClient.CreateTopicsAsync(new List<TopicSpecification> { topicSpec });
                    Console.WriteLine($"✅ Topic '{topicName}' created successfully.");
                }
                catch (CreateTopicsException e)
                {
                    foreach (var result in e.Results)
                    {
                        if (result.Error.Code != ErrorCode.TopicAlreadyExists)
                            Console.WriteLine($"❌ An error occurred: {result.Error.Reason}");
                        else
                            Console.WriteLine($"⚠️ Topic '{result.Topic}' already exists.");
                    }
                }
            }
        }
    }
}
