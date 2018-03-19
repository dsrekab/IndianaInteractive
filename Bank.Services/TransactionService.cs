using Bank.Contract.Data;
using Bank.Contract.Domain;
using Bank.Contract.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Bank.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IRepositoryReader<Transaction> _transactionRepoReader;
        private readonly IRepositoryAdder<Transaction> _transactionRepoWriter;
        private readonly IDateTimeService _dateTimeService;

        public TransactionService(IRepositoryReader<Transaction> transactionRepoReader, IRepositoryAdder<Transaction> transactionRepoWriter, IDateTimeService dateTimeService)
        {
            _transactionRepoReader = transactionRepoReader;
            _transactionRepoWriter = transactionRepoWriter;
            _dateTimeService = dateTimeService;
        }

        public void AddTransaction(int accountId, decimal amount, string description = null)
        {
            //transactions are created from the account service, which ensures there is an active account.
            //No need to verify accountId is an active account here.

            Transaction transaction = new Transaction { AccountId = accountId, Amount = amount, DateTime = _dateTimeService.Now(), Description=description };
            _transactionRepoWriter.Add(transaction);
        }

        public void AddTransaction(int accountId, decimal amount, DateTime trxDateTime, string description = null)
        {
            //transactions are created from the account service, which ensures there is an active account.
            //No need to verify accountId is an active account here.

            Transaction transaction = new Transaction { AccountId = accountId, Amount = amount, DateTime = trxDateTime, Description = description };
            _transactionRepoWriter.Add(transaction);
        }

        public IEnumerable<Transaction> GetAllTransactions()
        {
            return _transactionRepoReader.GetAll();
        }

        public IEnumerable<Transaction> GetAllTransactionsByAccountId(int accountId)
        {
            var allTransactions = GetAllTransactions();
            return allTransactions.Where(m => m.AccountId == accountId).OrderBy(d=>d.DateTime);
        }

        public IEnumerable<Transaction> GetAllTransactionsByAccountIdAndDateRange(int accountId, DateTime? beginDate, DateTime? endDate)
        {
            var allTransactions = GetAllTransactions();
            IEnumerable<Transaction> transactionList;

            if (beginDate is null)
            {
                if (endDate is null)
                {
                    throw new InvalidDataException("When searching by Account and Date Range a begin date or an end date must be supplied.");
                }
                else
                {
                    //no begin date, search for all transactions less than or equal to the end date
                    transactionList= allTransactions.Where(m => m.AccountId == accountId && m.DateTime<=endDate.Value).OrderBy(d => d.DateTime);
                }
            }
            else
            {
                if (endDate is null)
                {
                    //no end date, search for all transactions after the begin date
                    transactionList = allTransactions.Where(m => m.AccountId == accountId && m.DateTime >= beginDate.Value).OrderBy(d => d.DateTime);
                }
                else
                {
                    //begin date and end date, search for all transactions between the two
                    transactionList = allTransactions.Where(m => m.AccountId == accountId && m.DateTime >= beginDate.Value && m.DateTime<=endDate.Value).OrderBy(d => d.DateTime);
                }
            }

            return transactionList;
        }
    }
}
