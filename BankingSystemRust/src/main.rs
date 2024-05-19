mod account_service;
mod transaction_manager;
mod kafka_transaction_listener;
mod account;
mod transaction_command;

use std::env;
use tokio::time::sleep;
use crate::account_service::AccountService;
use crate::transaction_manager::TransactionManager;
use crate::kafka_transaction_listener::KafkaTransactionListener;

#[tokio::main]
async fn main() {
    println!("Starting Transactional System...");
    sleep(tokio::time::Duration::from_secs(10)).await;

    let kafka_url = env::var("KAFKA_URL").expect("KAFKA_URL must be set");
    let kafka_topic = env::var("KAFKA_TOPIC").expect("KAFKA_TOPIC must be set");

    println!("Connecting to Kafka at {}", kafka_url);
    println!("Using Kafka topic: {}", kafka_topic);
    
    let account_service = AccountService::new().await.expect("Failed to connect to MongoDB");

    let transaction_manager = TransactionManager::new(account_service);
    let kafka_listener = KafkaTransactionListener::new(
        kafka_url,
        kafka_topic,
        transaction_manager,
        "transaction-group".to_owned(),
        2097152
    );
    println!("Listening for transaction commands on Kafka topic...");

    kafka_listener.start_listening().await;

    println!("Transactional System Running. Press Ctrl+C to exit.");
}