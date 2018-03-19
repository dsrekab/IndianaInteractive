using Bank.Contract.Domain;
using Bank.Contract.Domain.AccountType;
using System;
using System.Collections.Generic;

namespace Bank.Contract.Services
{
    public interface IAccountService
    {
        void AddAccount(string accountNumber, IAccountType accountType, int owningCustomerId, int bankId, decimal startingBalance=0);
        void DeleteAccount(Account account);
        void DeleteAccount(Account account, DateTime trxDateTime);
        void DeleteAccount(int accountId);
        void DeleteAccount(int accountId, DateTime trxDateTime);
        void UpdateAccount(int accountId, IAccountType accountType, int owningCustomerId, int bankId, bool isActive); //Can't change the account number
        void WithdrawalFromAccount(int accountId, decimal amount, string description = null);
        void WithdrawalFromAccount(int accountId, decimal amount, DateTime trxDateTime, string description = null);
        void DepositToAccount(int accountId, decimal amount, string description = null);
        void DepositToAccount(int accountId, decimal amount, DateTime trxDateTime, string description = null);
        void TransferBetweenAccount(int fromAccountId, int toAccountId, decimal amount, string description = null);
        void TransferBetweenAccount(int fromAccountId, int toAccountId, decimal amount, DateTime trxDateTime, string description = null);
        IEnumerable<Account> GetAllAccounts();
        Account GetAccountByAccountNumber(string accountNumber);
        decimal GetAccountBalance(int accountId);
    }
}
