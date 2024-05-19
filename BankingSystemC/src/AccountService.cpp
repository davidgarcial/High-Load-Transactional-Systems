#include "AccountService.h"
#include <iostream>
#include <bsoncxx/json.hpp>
#include <mongocxx/client.hpp>
#include <mongocxx/instance.hpp>
#include <bsoncxx/builder/stream/document.hpp>
#include <bsoncxx/builder/stream/helpers.hpp> 

mongocxx::instance AccountService::instance{};

AccountService::AccountService()
    : client(mongocxx::uri(std::getenv("MONGODB_CONNECTION_STRING") ? std::getenv("MONGODB_CONNECTION_STRING") : "mongodb://localhost:27017")) {

    const char* dbName = std::getenv("MONGODB_DATABASE");
    const char* collectionName = std::getenv("MONGODB_COLLECTION");
    auto db = client[dbName ? dbName : "defaultDB"];
    accounts = db[collectionName ? collectionName : "defaultCollection"];
}


void AccountService::createAccount(const Account& account) {
    std::cout << "Attempting to create account with ID: " << account.id << std::endl;
    try {
        auto builder = bsoncxx::builder::stream::document{};
        bsoncxx::document::value doc_value = builder
            << "id" << account.id
            << "name" << account.name
            << "balance" << account.balance
            << bsoncxx::builder::stream::finalize;

        std::cout << "Connecting to MongoDB to insert document." << std::endl;
        auto result = accounts.insert_one(doc_value.view());
        if (result) {
            std::cout << "Account creation successful for ID: " << account.id << std::endl;
        } else {
            std::cerr << "Account creation failed for ID: " << account.id << std::endl;
        }
    } catch (const std::exception& e) {
        std::cerr << "Exception during account creation: " << e.what() << std::endl;
    }
}

Account AccountService::getAccount(const std::string& id) {
    std::cout << "Fetching account with ID: " << id << std::endl;
    try {
        auto builder = bsoncxx::builder::stream::document{};
        bsoncxx::document::value doc_value = builder << "id" << id << bsoncxx::builder::stream::finalize;

        auto result = accounts.find_one(doc_value.view());
        Account account;
        if (result) {
            auto view = result->view();
            account.id = view["id"].get_utf8().value.to_string();
            account.name = view["name"].get_utf8().value.to_string();
            account.balance = view["balance"].get_double().value;
            std::cout << "Account fetched successfully for ID: " << id << std::endl;
        } else {
            std::cerr << "No account found for ID: " << id << std::endl;
        }
        return account;
    } catch (const std::exception& e) {
        std::cerr << "Exception while fetching account: " << e.what() << std::endl;
        return Account{}; // Return an empty Account object or handle it as needed
    }
}

void AccountService::updateAccount(const Account& account) {
    std::cout << "Updating account with ID: " << account.id << std::endl;
    try {
        auto filter_builder = bsoncxx::builder::stream::document{} << "id" << account.id << bsoncxx::builder::stream::finalize;
        auto update_builder = bsoncxx::builder::stream::document{} << "$set" << bsoncxx::builder::stream::open_document
                             << "balance" << account.balance
                             << bsoncxx::builder::stream::close_document << bsoncxx::builder::stream::finalize;

        auto result = accounts.update_one(filter_builder.view(), update_builder.view());
        if (result->modified_count() > 0) {
            std::cout << "Account updated successfully for ID: " << account.id << std::endl;
        } else {
            std::cerr << "Failed to update account for ID: " << account.id << std::endl;
            if (result->matched_count() == 0) {
                std::cerr << "No account matched the ID: " << account.id << std::endl;
            }
        }
    } catch (const std::exception& e) {
        std::cerr << "Exception while updating account: " << e.what() << std::endl;
    }
}
