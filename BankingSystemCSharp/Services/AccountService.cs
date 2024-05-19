using MongoDB.Driver;
using System;
using System.Threading.Tasks;

public class AccountService
{
    private readonly IMongoCollection<Account> _accounts;

    public AccountService()
    {
        string connectionString = Environment.GetEnvironmentVariable("MONGODB_CONNECTION_STRING") ?? "mongodb://localhost:27017";
        string dbName = Environment.GetEnvironmentVariable("MONGODB_DATABASE") ?? "defaultDatabase";
        string collectionName = Environment.GetEnvironmentVariable("MONGODB_COLLECTION") ?? "defaultCollection";

        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(dbName);
        _accounts = database.GetCollection<Account>(collectionName);
    }


    public async Task CreateAccountAsync(Account account)
    {
        Console.WriteLine($"Creating new account with ID: {account.Id}");
        await _accounts.InsertOneAsync(account);
        Console.WriteLine($"Account with ID: {account.Id} created successfully.");
    }

    public async Task<Account> GetAccountAsync(string id)
    {
        Console.WriteLine($"Fetching account with ID: {id}");
        var account = await _accounts.Find(a => a.Id == id).FirstOrDefaultAsync();
        if (account != null)
        {
            Console.WriteLine($"Account with ID: {id} fetched successfully.");
        }
        else
        {
            Console.WriteLine($"No account found with ID: {id}.");
        }
        return account;
    }

    public async Task UpdateAccountAsync(Account account)
    {
        Console.WriteLine($"Updating account with ID: {account.Id}");
        await _accounts.ReplaceOneAsync(a => a.Id == account.Id, account);
        Console.WriteLine($"Account with ID: {account.Id} updated successfully.");
    }
}
