#include "AccountService.h"
#include "TransactionManager.h"
#include "KafkaTransactionListener.h"

int main() {
    std::string kafkaUrl = std::getenv("KAFKA_URL");
    std::string kafkaTopic = std::getenv("KAFKA_TOPIC");

    AccountService accountService;
    TransactionManager transactionManager(accountService);
    KafkaTransactionListener listener(kafkaUrl, kafkaTopic, transactionManager);

    listener.startListening();

    return 0;
}
