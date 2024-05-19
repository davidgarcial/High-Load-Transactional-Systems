use crate::account_service::AccountService;
use crate::account::Account;
use std::error::Error;

pub struct TransactionManager {
    account_service: AccountService,
}

impl TransactionManager {
    pub fn new(account_service: AccountService) -> Self {
        Self { account_service }
    }

    pub async fn create_account_with_id(&self, id: String, name: String, initial_balance: f64) -> Result<(), Box<dyn Error>> {
        let account = Account::new(id, name, initial_balance);
        self.account_service.create_account_async(account).await
    }

    pub async fn get_account_async(&self, id: String) -> Result<Option<Account>, Box<dyn Error>> {
        self.account_service.get_account_async(id).await
    }

    pub async fn deposit_async(&self, account_id: String, amount: f64) -> Result<(), Box<dyn Error>> {
        if let Ok(Some(mut account)) = self.get_account_async(account_id.clone()).await {
            let new_balance = account.balance.unwrap_or(0.0) + amount;
            account.balance = Some(new_balance);
            self.account_service.update_account_async(&account).await
        } else {
            Err(format!("Account not found for ID: {}", account_id).into())
        }
    }
    
    pub async fn withdraw_async(&self, account_id: String, amount: f64) -> Result<(), Box<dyn Error>> {
        if let Ok(Some(mut account)) = self.get_account_async(account_id.clone()).await {
            if account.balance.unwrap_or(0.0) >= amount {
                let new_balance = account.balance.unwrap_or(0.0) - amount;
                account.balance = Some(new_balance);
                self.account_service.update_account_async(&account).await
            } else {
                Err(format!("Insufficient funds for account ID: {}", account_id).into())
            }
        } else {
            Err(format!("Account not found for ID: {}", account_id).into())
        }
    }    
    
    pub async fn transfer_async(&self, from_account_id: String, to_account_id: String, amount: f64) -> Result<(), Box<dyn Error>> {
        let from_account_result = self.get_account_async(from_account_id.clone()).await?;
        let to_account_result = self.get_account_async(to_account_id.clone()).await?;
    
        if let (Some(mut from_account), Some(mut to_account)) = (from_account_result, to_account_result) {
            if from_account.balance.unwrap_or(0.0) >= amount {
                from_account.balance = Some(from_account.balance.unwrap_or(0.0) - amount);
                to_account.balance = Some(to_account.balance.unwrap_or(0.0) + amount);
    
                self.account_service.update_account_async(&from_account).await?;
                self.account_service.update_account_async(&to_account).await
            } else {
                Err(format!("Insufficient funds for transfer from account ID: {}", from_account_id).into())
            }
        } else {
            Err(format!("One or both accounts not found (from ID: {}, to ID: {})", from_account_id, to_account_id).into())
        }
    }
}
