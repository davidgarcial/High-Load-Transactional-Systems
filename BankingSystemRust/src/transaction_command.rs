use serde::{Deserialize, Serialize};

#[derive(Debug, Serialize, Deserialize)]
pub struct TransactionCommand {
    #[serde(rename = "ActionType")]
    pub action_type: String,
    #[serde(rename = "commandData")]
    pub command_data: CommandData,
}

#[derive(Debug, Serialize, Deserialize)]
pub struct CommandData {
    #[serde(rename = "AccountId")]
    pub account_id: String,
    #[serde(rename = "AccountName")]
    pub account_name: Option<String>,
    #[serde(rename = "Amount")]
    pub amount: f64,
    #[serde(rename = "TargetAccountId")]
    pub target_account_id: Option<String>,
}
