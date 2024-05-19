use mongodb::bson::{doc, oid::ObjectId};
use serde::{Deserialize, Serialize};

#[derive(Debug, Serialize, Deserialize)]
pub struct Account {
    #[serde(rename = "_id", skip_serializing_if = "Option::is_none")]
    pub id: Option<ObjectId>,
    #[serde(rename = "account_id")]
    pub account_id: String,
    pub name: String,
    pub balance: Option<f64>,
}

impl Account {
    pub fn new(account_id: String, name: String, balance: f64) -> Self {
        Self {
            id: None,
            account_id: account_id,
            name,
            balance: Some(balance)
        }
    }
}
