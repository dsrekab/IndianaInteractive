using Bank.Contract.Data;
using Bank.Contract.Domain;
using Bank.Contract.Domain.AccountType;
using Bank.Contract.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bank.Services
{
    public class AccountService : IAccountService
    {
        private readonly IRepositoryReader<Account> _repositoryReader;
        private readonly IRepositoryWriter<Account> _repositoryWriter;
        private readonly ITransactionService _trxService;

        public AccountService(IRepositoryReader<Account> repositoryReader, IRepositoryWriter<Account> repositoryWriter, ITransactionService trxService)
        {
            _repositoryReader = repositoryReader;
            _repositoryWriter = repositoryWriter;
            _trxService = trxService;
        }

        public void AddAccount(string accountNumber, IAccountType accountType, int owningCustomerId, int bankId, decimal startingBalance = 0)
        {
            Account newAccount = new Account { AccountNumber = accountNumber, AccountType = accountType, OwningCustomerId = owningCustomerId, BankId = bankId, IsActive = true };
            _repositoryWriter.Add(newAccount);

            Account addedAccount = GetAccountByAccountNumber(accountNumber);
            if (addedAccount==null)
            {
                throw new Exception($"Account Number {accountNumber} was not successfully added to the database for unknown reasons.");
            }

            //create the initial transactions
            _trxService.AddTransaction(addedAccount.Id, startingBalance, "Opening Account");
        }

        public void DeleteAccount(Account account)
        {
            //withdraw the balance
            _trxService.AddTransaction(account.Id, GetAccountBalance(account.Id) * -1, "Withdraw existing balance before deleting account.");

            _repositoryWriter.Delete(account);
        }

        public void DeleteAccount(Account account, DateTime trxDateTime)
        {
            //withdraw the balance
            _trxService.AddTransaction(account.Id, GetAccountBalance(account.Id) * -1, trxDateTime, "Withdraw existing balance before deleting account.");

            _repositoryWriter.Delete(account);
        }

        public void DeleteAccount(int accountId)
        {
            Account acctToDelete = GetAllAccounts().FirstOrDefault(m => m.Id == accountId);

            if (acctToDelete != null)
            {
                DeleteAccount(acctToDelete);
            }
        }

        public void DeleteAccount(int accountId, DateTime trxDateTime)
        {
            Account acctToDelete = GetAllAccounts().FirstOrDefault(m => m.Id == accountId);

            if (acctToDelete!=null)
            {
                DeleteAccount(acctToDelete, trxDateTime);
            }
        }

        public void DepositToAccount(int accountId, decimal amount, string description=null)
        {
            if (amount<0)
            {
                throw new Exception("Unable to deposit a negative dollar amount");
            }

            Account account = GetAllAccounts().FirstOrDefault(m => m.Id == accountId && m.IsActive==true);

            if (account == null)
            {
                throw new Exception($"No active accounts found for AccountId {accountId}");
            }
            else
            {
                _trxService.AddTransaction(account.Id, amount, description);
            }
        }

        public void DepositToAccount(int accountId, decimal amount, DateTime trxDateTime, string description = null)
        {
            if (amount < 0)
            {
                throw new Exception("Unable to deposit a negative dollar amount");
            }

            Account account = GetAllAccounts().FirstOrDefault(m => m.Id == accountId && m.IsActive == true);

            if (account == null)
            {
                throw new Exception($"No active accounts found for AccountId {accountId}");
            }
            else
            {
                _trxService.AddTransaction(account.Id, amount, trxDateTime, description);
            }
        }

        public decimal GetAccountBalance(int accountId)
        {
            //this would be done in the db with stored procedures normally
            var transactionList = _trxService.GetAllTransactionsByAccountId(accountId);
            return transactionList.Sum(m => m.Amount);
        }

        public Account GetAccountByAccountNumber(string accountNumber)
        {
            var accountList = GetAllAccounts();

            if (accountList!=null)
            {
                return accountList.FirstOrDefault(m => m.AccountNumber == accountNumber);
            }
            else
            {
                return null;
            }
            
        }

        public IEnumerable<Account> GetAllAccounts()
        {
            return _repositoryReader.GetAll();
        }

        public void TransferBetweenAccount(int fromAccountId, int toAccountId, decimal amount, string description = null)
        {
            if (amount < 0)
            {
                throw new Exception("Unable to transfer a negative dollar amount");
            }

            Account fromAccount = GetAllAccounts().FirstOrDefault(m => m.Id == fromAccountId && m.IsActive == true);
            Account toAccount = GetAllAccounts().FirstOrDefault(m => m.Id == toAccountId && m.IsActive == true);

            if (fromAccount == null)
            {
                throw new Exception($"No active accounts found for FromAccount AccountId {fromAccountId}");
            }
            else if (toAccount==null)
            {
                throw new Exception($"No active accounts found for ToAccount AccountId {toAccountId}");
            }
            else
            {
                decimal transferOutAmount = amount * -1;

                //In real life, this would be in a transCommit in case something failed between the transactions
                _trxService.AddTransaction(fromAccount.Id, transferOutAmount, description);
                _trxService.AddTransaction(toAccount.Id, amount, description);

            }
        }

        public void TransferBetweenAccount(int fromAccountId, int toAccountId, decimal amount, DateTime trxDateTime, string description = null)
        {
            if (amount < 0)
            {
                throw new Exception("Unable to transfer a negative dollar amount");
            }

            List<Account> AccountList = GetAllAccounts().ToList();

            Account fromAccount = AccountList.FirstOrDefault(m => m.Id == fromAccountId && m.IsActive == true);
            Account toAccount = AccountList.FirstOrDefault(m => m.Id == toAccountId && m.IsActive == true);

            if (fromAccount == null)
            {
                throw new Exception($"No active accounts found for FromAccount AccountId {fromAccount}");
            }
            else if (toAccount == null)
            {
                throw new Exception($"No active accounts found for ToAccount AccountId {fromAccount}");
            }
            else
            {
                decimal transferOutAmount = amount * -1;

                //In real life, this would be in a transCommit in case something failed between the transactions
                _trxService.AddTransaction(fromAccount.Id, transferOutAmount, trxDateTime, description);
                _trxService.AddTransaction(toAccount.Id, amount, trxDateTime, description);

            }
        }

        public void UpdateAccount(int accountId, IAccountType accountType, int owningCustomerId, int bankId, bool isActive)
        {
            Account account = GetAllAccounts().FirstOrDefault(m => m.Id == accountId);

            if (account == null)
            {
                throw new Exception($"Unable to find account for Account Id {accountId}");
            }
            else
            {
                account.AccountType = accountType;
                account.OwningCustomerId = owningCustomerId;
                account.BankId = bankId;
                account.IsActive = isActive;

                _repositoryWriter.Update(account);
            }
        }

        public void WithdrawalFromAccount(int accountId, decimal amount, string description = null)
        {
            Account account = GetAllAccounts().FirstOrDefault(m => m.Id == accountId && m.IsActive == true);

            if (amount > 0)
            {
                amount = amount * -1; //withdrawals should be a negative dollar amount
            }

            if (account == null)
            {
                throw new Exception($"No active accounts found for AccountId {accountId}");
            }
            else
            {
                //make sure the dollar amount is ok based on the type
                if (account.AccountType.GetMaxWithdrawalAmount().HasValue && account.AccountType.GetMaxWithdrawalAmount().Value < Math.Abs(amount))
                {
                    throw new Exception($"Account Types of {account.AccountType.GetDisplayName()} are limited to a dollar amount of {account.AccountType.GetMaxWithdrawalAmount()} in a single transaction");
                }
                
                //make sure there is enough money in the account
                decimal currentBalance = GetAccountBalance(accountId);

                if (Math.Abs(amount) > currentBalance)
                {
                    throw new Exception("Withdrawal amount exceeds current account balance.  Overdrafting not allowed.");
                }

                _trxService.AddTransaction(account.Id, amount, description);
            }
        }

        public void WithdrawalFromAccount(int accountId, decimal amount, DateTime trxDateTime, string description = null)
        {
            Account account = GetAllAccounts().FirstOrDefault(m => m.Id == accountId && m.IsActive == true);

            if (amount > 0)
            {
                amount = amount * -1; //withdrawals should be a negative dollar amount
            }

            if (account == null)
            {
                throw new Exception($"No active accounts found for AccountId {accountId}");
            }
            else
            {
                //make sure the dollar amount is ok based on the type
                if (account.AccountType.GetMaxWithdrawalAmount().HasValue && account.AccountType.GetMaxWithdrawalAmount().Value < Math.Abs(amount))
                {
                    throw new Exception($"Account Types of {account.AccountType.GetDisplayName()} are limited to a dollar amount of {account.AccountType.GetMaxWithdrawalAmount()} in a single transaction");
                }

                //make sure there is enough money in the account
                decimal currentBalance = GetAccountBalance(accountId);

                if (Math.Abs(amount)>currentBalance)
                {
                    throw new Exception("Withdrawal amount exceeds current account balance.  Overdrafting not allowed.");
                }
                
                _trxService.AddTransaction(account.Id, amount, trxDateTime, description);
            }
        }
    }
}
