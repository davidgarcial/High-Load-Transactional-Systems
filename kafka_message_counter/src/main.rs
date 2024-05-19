use rdkafka::config::ClientConfig;
use rdkafka::consumer::{StreamConsumer, Consumer};
use log::info;
use tokio::signal::ctrl_c;
use futures_util::stream::StreamExt;
use std::env;

#[tokio::main]
async fn main() {
    env_logger::init();
    println!("Application started, waiting for messages...");

    let bootstrap_servers = env::var("KAFKA_URL").expect("KAFKA_URL must be set");
    let topic = env::var("KAFKA_TOPIC").expect("KAFKA_TOPIC must be set");
    let group_id = "transaction-group";

    let consumer: StreamConsumer = ClientConfig::new()
        .set("bootstrap.servers", &bootstrap_servers)
        .set("group.id", group_id.to_string())
        .set("auto.offset.reset", "earliest")
        .set("enable.auto.commit", "false")  // Manual commit configuration
        .create()
        .expect("Consumer creation failed");

    consumer.subscribe(&[&topic]).expect("Subscription failed");

    let mut message_stream = consumer.stream();
    let mut message_count = 0u64;

    println!("Kafka Transaction Listener started. Listening for messages...");

    loop {
        tokio::select! {
            _ = ctrl_c() => {
                info!("Termination signal received. Shutting down...");
                break;
            },
            Some(message) = message_stream.next() => {
                match message {
                    Ok(_) => {
                        message_count += 1;
                        println!("Message received: Count = {}", message_count);
                    },
                    Err(e) => println!("Error in consumer loop: {}", e),
                }
            }
        }
    }

    println!("Total messages received!: {}", message_count);
}
