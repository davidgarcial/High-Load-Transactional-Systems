#ifndef TRANSACTION_MANAGER_H
#define TRANSACTION_MANAGER_H

#include "AccountService.h"

class TransactionManager {
    AccountService& accountService;

public:
    TransactionManager(AccountService& service);
    void createAccount(const std::string& id, const std::string& name, double initialBalance);
    void deposit(const std::string& accountId, double amount);
    void withdraw(const std::string& accountId, double amount);
    void transfer(const std::string& fromAccountId, const std::string& toAccountId, double amount);
};

#endif // TRANSACTION_MANAGER_H
