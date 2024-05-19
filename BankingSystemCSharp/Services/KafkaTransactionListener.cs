using Confluent.Kafka;
using System.Text.Json;

public class KafkaTransactionListener
{
    private readonly string _bootstrapServers;
    private readonly string _topic;
    private readonly TransactionManager _transactionManager;

    public KafkaTransactionListener(string bootstrapServers, string topic, TransactionManager transactionManager)
    {
        _bootstrapServers = bootstrapServers;
        _topic = topic;
        _transactionManager = transactionManager;
    }

    public void StartListening()
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = _bootstrapServers,
            GroupId = "transaction-group",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false  // Manual commit of offsets
        };

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true, // Improves performance by skipping case sensitivity checks
            IgnoreNullValues = true,           // Avoids serializing null values to reduce payload size
            WriteIndented = false              // Produces minimized JSON to reduce payload size
        };

        using (var consumer = new ConsumerBuilder<Ignore, string>(config).Build())
        {
            consumer.Subscribe(_topic);
            var cancellationToken = new CancellationTokenSource();

            Console.WriteLine("Kafka Transaction Listener started. Listening for transaction commands...");

            try
            {
                while (true)
                {
                    var consumeResult = consumer.Consume(cancellationToken.Token);
                    var transactionCommand = JsonSerializer.Deserialize<TransactionCommand>(consumeResult.Message.Value, options);

                    if (transactionCommand != null)
                    {
                        ProcessCommand(transactionCommand).Wait();
                    }
                }
            }
            catch (OperationCanceledException)
            {
                consumer.Close();
            }
        }
    }

    private async Task ProcessCommand(TransactionCommand command)
    {
        Console.WriteLine($"Processing command: {command.ActionType}");

        switch (command.ActionType)
        {
            case "CreateAccount":
                Console.WriteLine($"Creating account with ID: {command.commandData.AccountId} and Name: {command.commandData.AccountName}");
                await _transactionManager.CreateAccountAsync(command.commandData.AccountId, command.commandData.AccountName, command.commandData.Amount);
                Console.WriteLine($"Account created with ID: {command.commandData.AccountId}");
                break;
            case "Deposit":
                Console.WriteLine($"Depositing amount: {command.commandData.Amount} to Account ID: {command.commandData.AccountId}");
                await _transactionManager.DepositAsync(command.commandData.AccountId, command.commandData.Amount);
                Console.WriteLine($"Deposit completed for Account ID: {command.commandData.AccountId}");
                break;
            case "Withdrawal":
                Console.WriteLine($"Withdrawing amount: {command.commandData.Amount} from Account ID: {command.commandData.AccountId}");
                await _transactionManager.WithdrawAsync(command.commandData.AccountId, command.commandData.Amount);
                Console.WriteLine($"Withdrawal completed for Account ID: {command.commandData.AccountId}");
                break;
            case "Transfer":
                Console.WriteLine($"Transferring amount: {command.commandData.Amount} from Account ID: {command.commandData.AccountId} to Target Account ID: {command.commandData.TargetAccountId}");
                await _transactionManager.TransferAsync(command.commandData.AccountId, command.commandData.TargetAccountId, command.commandData.Amount);
                Console.WriteLine($"Transfer completed from Account ID: {command.commandData.AccountId} to Target Account ID: {command.commandData.TargetAccountId}");
                break;
            default:
                Console.WriteLine($"Unknown action type received: {command.ActionType}");
                break;
        }
    }
}
