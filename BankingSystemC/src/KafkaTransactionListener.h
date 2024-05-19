#ifndef KAFKA_TRANSACTION_LISTENER_H
#define KAFKA_TRANSACTION_LISTENER_H

#include <string>
#include <librdkafka/rdkafkacpp.h>
#include "TransactionManager.h"
#include <simdjson.h> // Include simdjson header

class KafkaTransactionListener : public RdKafka::EventCb {
    std::string bootstrapServers;
    std::string topic;
    TransactionManager& transactionManager;

public:
    KafkaTransactionListener(const std::string& servers, const std::string& topic, TransactionManager& manager);
    void event_cb(RdKafka::Event &event) override;
    void startListening();
    void ProcessCommand(const simdjson::dom::element& root); // Update method signature
};

#endif // KAFKA_TRANSACTION_LISTENER_H
