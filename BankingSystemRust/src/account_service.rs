use crate::account::Account;
use mongodb::{Client, Collection};
use mongodb::bson::doc;
use std::env;
use std::error::Error;

pub struct AccountService {
    accounts: Collection<Account>,
}

impl AccountService {
    pub async fn new() -> Result<Self, Box<dyn Error>> {
        let connection_string = env::var("MONGODB_CONNECTION_STRING").unwrap_or_else(|_| "mongodb://localhost:27017".to_string());
        let db_name = env::var("MONGODB_DATABASE").unwrap_or_else(|_| "defaultDatabase".to_string());
        let collection_name = env::var("MONGODB_COLLECTION").unwrap_or_else(|_| "defaultCollection".to_string());

        let client = Client::with_uri_str(&connection_string).await?;
        let database = client.database(&db_name);
        let accounts = database.collection::<Account>(&collection_name);

        Ok(Self { accounts })
    }

    pub async fn create_account_async(&self, account: Account) -> Result<(), Box<dyn Error>> {
        println!("Creating new account with ID: {:?}", account.id);
        self.accounts.insert_one(account, None).await?;
        println!("Account created successfully.");
        Ok(())
    }

    pub async fn get_account_async(&self, id: String) -> Result<Option<Account>, Box<dyn Error>> {
        println!("Fetching account with ID: {}", id);
        let account = self.accounts.find_one(doc! { "account_id": id.clone() }, None).await?;
        if let Some(acc) = &account {
            println!("Account with ID: {:?} fetched successfully.", acc.id);
        } else {
            println!("No account found with ID: {}.", id);
        }
        Ok(account)
    }

    pub async fn update_account_async(&self, account: &Account) -> Result<(), Box<dyn Error>> {
        println!("Account with ID: {:?} fetched successfully.", account.id);
        self.accounts.replace_one(doc! { "account_id": &account.id }, account, None).await?;
        println!("Account with ID: {:?} updated successfully.", account.id);
        Ok(())
    }
}

