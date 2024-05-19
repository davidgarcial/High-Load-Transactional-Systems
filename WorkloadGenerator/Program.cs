using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Newtonsoft.Json;
using System.Text.Json;

namespace KafkaWorkloadGenerator
{
    class Program
    {
        static Dictionary<string, Account> accounts = [];
        static IProducer<Null, string>? producer;
        static readonly Random random = new();
        static KafkaConfig config = new KafkaConfig();

        static async Task Main()
        {
            Console.WriteLine("Initializing workload generator...");
            await Task.Delay(10000);

            if (WaitForKafka(config.Url, TimeSpan.FromSeconds(60)))
            {
                Console.WriteLine("Kafka is up and running. Proceed with the workload generator...");
                InitializeProducer(config.Url);
                CreateTopicIfNotExists(config.Url, config.Topic);
                Console.WriteLine($"Kafka connection established at {config.Url}");

                if (int.TryParse(Environment.GetEnvironmentVariable("ACCOUNTS"), out int accountsCount))
                {
                    await GenerateAccounts(accountsCount);
                }
                else
                {
                    await GenerateAccounts(100); 
                }

                if (int.TryParse(Environment.GetEnvironmentVariable("TRANSACTIONS"), out int transactionsCount))
                {
                    SimulateWorkload(config.Topic, transactionsCount);
                }
                else
                {
                    SimulateWorkload(config.Topic, 5000);
                }
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

        static void InitializeProducer(string bootstrapServers)
        {
            var config = new ProducerConfig { BootstrapServers = bootstrapServers };
            producer = new ProducerBuilder<Null, string>(config).Build();
        }

        static void CreateTopicIfNotExists(string bootstrapServers, string topic)
        {
            using (var adminClient = new AdminClientBuilder(new AdminClientConfig { BootstrapServers = bootstrapServers }).Build())
            {
                try
                {
                    var metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(20));
                    if (!metadata.Topics.Exists(t => t.Topic == topic))
                    {
                        adminClient.CreateTopicsAsync(new TopicSpecification[] {
                            new TopicSpecification { Name = topic, NumPartitions = 1, ReplicationFactor = 1 }
                        }).Wait();
                        Console.WriteLine($"Topic '{topic}' created.");
                    }
                    else
                    {
                        Console.WriteLine($"Topic '{topic}' already exists.");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Exception occurred: {e.Message}");
                }
            }
        }

        static void SimulateWorkload(string topic, int transactionsCount)
        {
            for (int i = 0; i < transactionsCount; i++)
            {
                var transactionType = DecideTransactionType();
                var accountIds = PickRandomAccountIds();
                var amount = GenerateRandomAmount();

                switch (transactionType)
                {
                    case TransactionType.Deposit:
                        SendTransactionCommand(topic, "Deposit", new { AccountId = accountIds.Item1, Amount = amount });
                        break;
                    case TransactionType.Withdrawal:
                        SendTransactionCommand(topic, "Withdrawal", new { AccountId = accountIds.Item1, Amount = amount });
                        break;
                    case TransactionType.Transfer:
                        SendTransactionCommand(topic, "Transfer", new { AccountId = accountIds.Item1, TargetAccountId = accountIds.Item2, Amount = amount });
                        break;
                    default:
                        break;
                }

                Task.Delay(TimeSpan.FromMilliseconds(GenerateRandomDelay())).Wait();
            }
        }

        static TransactionType DecideTransactionType()
        {
            TransactionType[] transactionType = new[] {
                TransactionType.Deposit, TransactionType.Transfer,  TransactionType.Withdrawal
            };

            int index = random.Next(transactionType.Length);
            return transactionType[index];
        }

        static Tuple<string, string> PickRandomAccountIds()
        {
            var accountIds = new List<string>(accounts.Keys);
            var accountId1 = accountIds[random.Next(accountIds.Count)];
            var accountId2 = accountIds[random.Next(accountIds.Count)];
            return new Tuple<string, string>(accountId1, accountId2);
        }

        public static void SendTransactionCommand(string topic, string actionType, object commandData)
        {
            var command = new
            {
                ActionType = actionType,
                commandData
            };

            string message = JsonConvert.SerializeObject(command);

            var messageToSend = new Message<Null, string> { Value = message };

            try
            {
                producer.Produce(topic, messageToSend, report =>
                {
                    DeliveryHandler(report, message);
                });
            }
            catch (ProduceException<Null, string> e)
            {
                Console.WriteLine($"Failed to deliver message: {e.Message} [{e.Error.Code}]");
            }
        }

        private static void DeliveryHandler(DeliveryReport<Null, string> report, string originalMessage)
        {
            if (report.Error.Code != ErrorCode.NoError)
            {
                Console.WriteLine($"Failed to deliver message: {report.Error.Reason}");
            }
            else
            {
                Console.WriteLine($"Delivered message to {report.TopicPartitionOffset}");
                Console.WriteLine($"Emitted event: {originalMessage}");
            }
        }

        static double GenerateRandomAmount()
        {
            return random.Next(10, 1000);
        }

        static int GenerateRandomDelay()
        {
            return random.Next(100, 1000);
        }

        static async Task GenerateAccounts(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var account = new Account
                {
                    Name = $"Account_{i}",
                    Balance = random.Next(1000, 100000)
                };
                accounts.Add(account.Id, account);

                var command = new
                {
                    AccountId = account.Id,
                    AccountName = account.Name,
                    Amount = account.Balance
                };

                SendTransactionCommand(config.Topic, "CreateAccount", command);
            }

            producer.Flush(TimeSpan.FromSeconds(10));
        }
    }

    public class KafkaConfig
    {
        public string Url { get; set; }
        public string Topic { get; set; }

        public KafkaConfig()
        {
            Url = Environment.GetEnvironmentVariable("KAFKA_URL") ?? "localhost:9092";
            Topic = Environment.GetEnvironmentVariable("KAFKA_TOPIC") ?? "transaction-topic";
        }
    }

    public class Account
    {
        public string Id { get; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public double Balance { get; set; }
    }

    enum TransactionType
    {
        CreateAccount,
        Deposit,
        Withdrawal,
        Transfer
    }
}
