using Confluent.Kafka;
using Confluent.Kafka.Admin;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Starting Transactional System...");
        await Task.Delay(10000);

        KafkaConfig config = new KafkaConfig();
        if (WaitForKafka(config.KafkaUrl, TimeSpan.FromSeconds(60)))
        {
            Console.WriteLine($"Connecting to Kafka at {config.KafkaUrl}");
            Console.WriteLine($"Using Kafka Topic: {config.KafkaTopic}");

            var accountService = new AccountService();
            var transactionManager = new TransactionManager(accountService);
            
            // Use `config.KafkaTopic` to specify the Kafka topic
            var kafkaListener = new KafkaTransactionListener(config.KafkaUrl, config.KafkaTopic, transactionManager);
            
            Console.WriteLine("Listening for transaction commands on Kafka topic...");
            
            kafkaListener.StartListening();
            
            Console.WriteLine("Transactional System Running. Press any key to exit...");
            Console.ReadKey();
        }
        else
        {
            Console.WriteLine("Failed to connect to Kafka after the specified timeout.");
        }
    }

    static bool WaitForKafka(string bootstrapServers, TimeSpan timeout)
        {
            using var adminClient = new AdminClientBuilder(new AdminClientConfig { BootstrapServers = bootstrapServers }).Build();
            var startTime = DateTime.UtcNow;
            while (DateTime.UtcNow - startTime < timeout)
            {
                try
                {
                    var metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(5));
                    if (metadata.Brokers.Count > 0)
                    {
                        return true;
                    }
                }
                catch (KafkaException ex)
                {
                    Console.WriteLine($"Waiting for Kafka to become available... Exception: {ex.Message}");
                }

                System.Threading.Thread.Sleep(5000);
            }

            return false;
        }
}

public class KafkaConfig
{
    public string KafkaUrl { get; set; }
    public string KafkaTopic { get; set; }

    public KafkaConfig()
    {
        KafkaUrl = Environment.GetEnvironmentVariable("KAFKA_URL") ?? "localhost:9092";
        KafkaTopic = Environment.GetEnvironmentVariable("KAFKA_TOPIC") ?? "transaction-topic";
    }
}
