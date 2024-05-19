public class CommandData
{
    public string AccountId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? AccountName { get; set; }
    public string TargetAccountId { get; set; } = string.Empty;
}

public class TransactionCommand
{
    public string ActionType { get; set; }
    public CommandData commandData { get; set; }
}
