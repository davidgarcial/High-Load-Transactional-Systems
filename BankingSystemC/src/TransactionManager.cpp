#include "TransactionManager.h"
#include <iostream>

TransactionManager::TransactionManager(AccountService& service) : accountService(service) {}

void TransactionManager::createAccount(const std::string& id, const std::string& name, double initialBalance) {
    Account account{id, name, initialBalance};
    accountService.createAccount(account);
    std::cout << "Account created successfully: " << id << std::endl;
}

void TransactionManager::deposit(const std::string& accountId, double amount) {
    Account account = accountService.getAccount(accountId);
    account.balance += amount;
    accountService.updateAccount(account);
    std::cout << "Deposit successful: " << accountId << std::endl;
}

void TransactionManager::withdraw(const std::string& accountId, double amount) {
    Account account = accountService.getAccount(accountId);
    if (account.balance >= amount) {
        account.balance -= amount;
        accountService.updateAccount(account);
        std::cout << "Withdrawal successful: " << accountId << std::endl;
    } else {
        std::cout << "Insufficient funds: " << accountId << std::endl;
    }
}

void TransactionManager::transfer(const std::string& fromAccountId, const std::string& toAccountId, double amount) {
    Account fromAccount = accountService.getAccount(fromAccountId);
    Account toAccount = accountService.getAccount(toAccountId);

    if (fromAccount.balance >= amount) {
        fromAccount.balance -= amount;
        toAccount.balance += amount;
        accountService.updateAccount(fromAccount);
        accountService.updateAccount(toAccount);
        std::cout << "Transfer successful: " << fromAccountId << " to " << toAccountId << std::endl;
    } else {
        std::cout << "Transfer failed: Insufficient funds in " << fromAccountId << std::endl;
    }
}
