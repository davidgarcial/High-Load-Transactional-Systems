use crate::transaction_manager::TransactionManager;
use crate::transaction_command::TransactionCommand;
use rdkafka::config::ClientConfig;
use rdkafka::consumer::{StreamConsumer, Consumer, CommitMode};
use rdkafka::message::{BorrowedMessage, Message}; // Import Message trait
use serde_json::from_slice;
use tokio::signal::ctrl_c;
use log::{info, warn, error};
use futures::stream::StreamExt;

pub struct KafkaTransactionListener {
    bootstrap_servers: String,
    topic: String,
    transaction_manager: TransactionManager,
    group_id: String,
    retry_limit: i32,
}

impl KafkaTransactionListener {
    pub fn new(bootstrap_servers: String, topic: String, transaction_manager: TransactionManager, group_id: String, retry_limit: i32) -> Self {
        Self {
            bootstrap_servers,
            topic,
            transaction_manager,
            group_id,
            retry_limit,
        }
    }

    pub async fn start_listening(&self) {
        let consumer: StreamConsumer = ClientConfig::new()
            .set("bootstrap.servers", &self.bootstrap_servers)
            .set("group.id", &self.group_id)
            .set("auto.offset.reset", "earliest")
            .set("enable.auto.commit", "false")
            .create()
            .expect("Consumer creation failed");

        consumer.subscribe(&[&self.topic]).expect("Subscription failed");
        let mut message_stream = consumer.stream();

        while let Some(message) = message_stream.next().await {
            match message {
                Ok(borrowed_message) => {
                    self.handle_message(&borrowed_message, &consumer).await;
                },
                Err(e) => error!("Error in consumer loop: {}", e),
            }
        }
    }

    async fn handle_message(&self, message: &BorrowedMessage<'_>, consumer: &StreamConsumer) {
        if let Some(payload) = message.payload() {
            if let Ok(command) = from_slice::<TransactionCommand>(payload) {
                self.process_command(command).await;
                consumer.commit_message(message, CommitMode::Async).unwrap_or_else(|e| error!("Failed to commit message: {}", e));
            } else {
                warn!("Error deserializing command");
            }
        } else {
            warn!("Received empty or corrupted message");
        }
    }

    async fn process_command(&self, command: TransactionCommand) {
        info!("Processing command: {}", command.action_type);
        match command.action_type.as_str() {
            "CreateAccount" => {
                self.transaction_manager.create_account_with_id(
                    command.command_data.account_id.clone(),
                    command.command_data.account_name.unwrap_or_default(),
                    command.command_data.amount
                ).await.unwrap_or_else(|e| eprintln!("Error processing CreateAccount command: {}", e));
            },
            "Deposit" => {
                self.transaction_manager.deposit_async(
                    command.command_data.account_id.clone(),
                    command.command_data.amount
                ).await.unwrap_or_else(|e| eprintln!("Error processing Deposit command: {}", e));
            },
            "Withdrawal" => {
                self.transaction_manager.withdraw_async(
                    command.command_data.account_id.clone(),
                    command.command_data.amount
                ).await.unwrap_or_else(|e| eprintln!("Error processing Withdrawal command: {}", e));
            },
            "Transfer" => {
                if let Some(to_account_id) = command.command_data.target_account_id {
                    self.transaction_manager.transfer_async(
                        command.command_data.account_id.clone(),
                        to_account_id.clone(),
                        command.command_data.amount
                    ).await.unwrap_or_else(|e| eprintln!("Error processing Transfer command: {}", e));
                }
            },
            _ => {
                warn!("Unsupported command type received: {}", command.action_type);
            },
        }
    }
}
