using Bank.Contract.Services;
using Bank.Data;
using Bank.Services;
using Moq;
using System;
using System.Linq;

namespace Bank.IntegrationTests
{
    public class TransactionCrudIntegrationTests : IIntegrationTest
    {
        public void Run()
        {
            TransactionMemoryRepository trxRepository = new TransactionMemoryRepository();
            Mock<IDateTimeService> mockDateTime = new Mock<IDateTimeService>();
            mockDateTime.Setup(m => m.Now()).Returns(new DateTime(2018, 03, 18, 11, 29, 42));

            ITransactionService trxService = new TransactionService(trxRepository, trxRepository, mockDateTime.Object);

            //Add a new Transaction
            try
            {
                trxService.AddTransaction(1, 10);
            }
            catch (Exception ex)
            {

                throw new Exception($"Exception thrown attempting to ADD a transaction.  Exception: {ex.ToString()}");
            }

            //Read the transaction table to ensure it was added correctly
            try
            {
                var trxList = trxService.GetAllTransactions();
                if (trxList.Count() != 1) { throw new Exception($"Expected to find 1 transaction, instead found {trxList.Count()}"); }

                var trxListByAcct = trxService.GetAllTransactionsByAccountId(1);
                if (trxListByAcct.Count() != 1) { throw new Exception($"Expected to find 1 transaction by Account (1), instead found {trxListByAcct.Count()}"); }

                var trxListByAcctBeforeDate = trxService.GetAllTransactionsByAccountIdAndDateRange(1,null, new DateTime(2018, 04, 01));
                if (trxListByAcctBeforeDate.Count() != 1) { throw new Exception($"Expected to find 1 transaction by Account (1) before date 04/01/2018, instead found {trxListByAcctBeforeDate.Count()}"); }

                var trxListByAcctAfterDate = trxService.GetAllTransactionsByAccountIdAndDateRange(1, new DateTime(2018, 03, 01), null);
                if (trxListByAcctAfterDate.Count() != 1) { throw new Exception($"Expected to find 1 transaction by Account (1) after date 03/01/2018, instead found {trxListByAcctAfterDate.Count()}"); }

                var trxListByAcctBetweenDate = trxService.GetAllTransactionsByAccountIdAndDateRange(1, new DateTime(2018, 03, 01), new DateTime(2018, 04, 01));
                if (trxListByAcctBetweenDate.Count() != 1) { throw new Exception($"Expected to find 1 transaction by Account (1) between 03/01/2018 and 04/01/2018, instead found {trxListByAcctBetweenDate.Count()}"); }
            }
            catch (Exception ex)
            {

                throw new Exception($"Exception thrown attempting to read transactions after CREATE.  Exception: {ex.ToString()}");
            }
        }
    }
}
