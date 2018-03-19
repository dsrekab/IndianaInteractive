using Bank.Contract.Domain;
using System;
using System.Collections.Generic;

namespace Bank.Contract.Services
{
    public interface ITransactionService
    {
        void AddTransaction(int accountId, decimal amount, string description=null);
        void AddTransaction(int accountId, decimal amount, DateTime trxDateTime, string description = null);
        IEnumerable<Transaction> GetAllTransactions();
        IEnumerable<Transaction> GetAllTransactionsByAccountId(int accountId);
        IEnumerable<Transaction> GetAllTransactionsByAccountIdAndDateRange(int accountId, DateTime? beginDate, DateTime? endDate);
    }
}
