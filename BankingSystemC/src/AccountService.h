#ifndef ACCOUNT_SERVICE_H
#define ACCOUNT_SERVICE_H

#include "Account.h"
#include <mongocxx/client.hpp>
#include <mongocxx/instance.hpp>
#include <mongocxx/uri.hpp>
#include <mongocxx/collection.hpp>

class AccountService {
    static mongocxx::instance instance; // Static instance of mongocxx::instance
    mongocxx::client client;            // MongoDB client
    mongocxx::collection accounts;      // Collection handle

public:
    AccountService();
    void createAccount(const Account& account);
    Account getAccount(const std::string& id);
    void updateAccount(const Account& account);
};

#endif // ACCOUNT_SERVICE_H
