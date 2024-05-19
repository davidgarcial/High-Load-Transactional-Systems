#include "KafkaTransactionListener.h"
#include <iostream>
#include <csignal>
#include <simdjson.h> // Include simdjson header

KafkaTransactionListener::KafkaTransactionListener(const std::string& servers, const std::string& topicName, TransactionManager& manager)
    : bootstrapServers(servers), topic(topicName), transactionManager(manager) {}

void KafkaTransactionListener::event_cb(RdKafka::Event &event) {
    std::cerr << "Kafka Event: " << RdKafka::err2str(event.err()) << " " << event.str() << std::endl;
}

void KafkaTransactionListener::startListening() {
    std::string errstr;
    RdKafka::Conf *conf = RdKafka::Conf::create(RdKafka::Conf::CONF_GLOBAL);
    conf->set("metadata.broker.list", bootstrapServers, errstr);
    conf->set("group.id", "transaction-group", errstr);
    conf->set("event_cb", this, errstr);

    RdKafka::Consumer *consumer = RdKafka::Consumer::create(conf, errstr);
    RdKafka::Topic *topic = RdKafka::Topic::create(consumer, this->topic, nullptr, errstr);
    consumer->start(topic, 0, RdKafka::Topic::OFFSET_BEGINNING);

    std::cout << "Kafka Transaction Listener started. Listening for transaction commands..." << std::endl;

    while (true) {
        // Reduce timeout to 100ms
        RdKafka::Message *msg = consumer->consume(topic, 0, 100);

        if (msg->err() == RdKafka::ERR_NO_ERROR) {
            auto payload_str = std::string(static_cast<char*>(msg->payload()), msg->len());
            std::cout << "Message Received: " << payload_str << std::endl;
            try {
                simdjson::dom::parser parser; // Create a parser
                simdjson::dom::element root = parser.parse(payload_str); // Parse JSON
                ProcessCommand(root);
            } catch (simdjson::simdjson_error& e) {
                std::cerr << "JSON parsing error: " << e.what() << std::endl;
            }
        }
    }

    // Cleanup after the loop
    consumer->stop(topic, 0);
    delete topic;
    delete consumer;
    RdKafka::wait_destroyed(5000);
}

void KafkaTransactionListener::ProcessCommand(const simdjson::dom::element& cmd) {
    try {
        std::string_view actionType = cmd["ActionType"].get_string().value();
        std::cout << "Processing command: " << actionType << std::endl;

        if (actionType == "CreateAccount") {
            simdjson::dom::element commandData = cmd["commandData"];
            std::string_view accountId = commandData["AccountId"].get_string().value();
            std::string_view accountName = commandData["AccountName"].get_string().value();
            double amount = commandData["Amount"].get_double().value();

            std::cout << "Creating account with ID: " << accountId << " and Name: " << accountName << std::endl;

            std::string accountIdStr(accountId);
            std::string accountNameStr(accountName);

            transactionManager.createAccount(accountIdStr, accountNameStr, amount);

            std::cout << "Account created successfully with ID: " << accountId << std::endl;
        } else if (actionType == "Deposit") {
            simdjson::dom::element commandData = cmd["commandData"];
            std::string_view accountId = commandData["AccountId"].get_string().value();
            double amount = commandData["Amount"].get_double().value();
            
            std::string accountIdStr(accountId);
            transactionManager.deposit(accountIdStr, amount);
            std::cout << "Deposit completed for Account ID: " << accountId << std::endl;
        } else if (actionType == "Withdrawal") {
            simdjson::dom::element commandData = cmd["commandData"];
            std::string_view accountId = commandData["AccountId"].get_string().value();
            double amount = commandData["Amount"].get_double().value();

            std::string accountIdStr(accountId);
            transactionManager.withdraw(accountIdStr, amount);
            std::cout << "Withdrawal completed for Account ID: " << accountId << std::endl;
        } else if (actionType == "Transfer") {
            simdjson::dom::element commandData = cmd["commandData"];
            std::string_view fromAccountId = commandData["AccountId"].get_string().value();
            std::string_view toAccountId = commandData["TargetAccountId"].get_string().value();
            double amount = commandData["Amount"].get_double().value();

            std::string fromAccountIdStr(fromAccountId);
            std::string toAccountIdStr(toAccountId);

            transactionManager.transfer(fromAccountIdStr, toAccountIdStr, amount);
            std::cout << "Transfer completed from Account ID: " << fromAccountId << " to Target Account ID: " << toAccountId << std::endl;
        } else {
            std::cerr << "Unknown action type received: " << actionType << std::endl;
        }
    } catch (const simdjson::simdjson_error& e) {
        std::cerr << "simdjson error: " << e.what() << std::endl;
    }
}
