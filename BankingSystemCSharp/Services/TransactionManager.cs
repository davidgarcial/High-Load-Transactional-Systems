using System;
using System.Threading.Tasks;

public class TransactionManager
{
    private readonly AccountService _accountService;

    public TransactionManager(AccountService accountService)
    {
        _accountService = accountService;
    }

    public async Task CreateAccountAsync(string id, string name, decimal initialBalance)
    {
        Console.WriteLine($"Creating account with ID: {id}, Name: {name}, Initial Balance: {initialBalance}");
        var account = new Account { Id = id, Name = name, Balance = initialBalance };
        await _accountService.CreateAccountAsync(account);
        Console.WriteLine($"Account created successfully: {id}");
    }

    public async Task<Account> GetAccountAsync(string id)
    {
        Console.WriteLine($"Fetching account with ID: {id}");
        return await _accountService.GetAccountAsync(id);
    }

    public async Task DepositAsync(string accountId, decimal amount)
    {
        Console.WriteLine($"Depositing {amount} to account ID: {accountId}");
        var account = await GetAccountAsync(accountId);
        if (account != null)
        {
            account.Balance += amount;
            await _accountService.UpdateAccountAsync(account);
            Console.WriteLine($"Deposit successful: {accountId}");
        }
        else
        {
            Console.WriteLine($"Account not found: {accountId}");
        }
    }

    public async Task WithdrawAsync(string accountId, decimal amount)
    {
        Console.WriteLine($"Withdrawing {amount} from account ID: {accountId}");
        var account = await GetAccountAsync(accountId);
        if (account != null && account.Balance >= amount)
        {
            account.Balance -= amount;
            await _accountService.UpdateAccountAsync(account);
            Console.WriteLine($"Withdrawal successful: {accountId}");
        }
        else
        {
            Console.WriteLine($"Insufficient funds or account not found: {accountId}");
        }
    }

    public async Task TransferAsync(string fromAccountId, string toAccountId, decimal amount)
    {
        Console.WriteLine($"Transferring {amount} from account ID: {fromAccountId} to account ID: {toAccountId}");
        var fromAccount = await GetAccountAsync(fromAccountId);
        var toAccount = await GetAccountAsync(toAccountId);

        if (fromAccount != null && toAccount != null && fromAccount.Balance >= amount)
        {
            fromAccount.Balance -= amount;
            toAccount.Balance += amount;
            
            await _accountService.UpdateAccountAsync(fromAccount);
            await _accountService.UpdateAccountAsync(toAccount);
            Console.WriteLine($"Transfer successful: {fromAccountId} to {toAccountId}");
        }
        else
        {
            Console.WriteLine($"Transfer failed: Insufficient funds or accounts not found.");
        }
    }
}
