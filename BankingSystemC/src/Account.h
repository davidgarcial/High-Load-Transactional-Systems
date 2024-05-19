#ifndef ACCOUNT_H
#define ACCOUNT_H

#include <string>

class Account {
public:
    std::string id;
    std::string name;
    double balance;  // Decimal in C# is usually translated to double in C++
};

class CommandData {
public:
    std::string accountId = "";
    double amount = 0.0;
    std::string accountName;
    std::string targetAccountId = "";
};

class TransactionCommand {
public:
    std::string actionType;
    CommandData commandData;
};

#endif // ACCOUNT_H
