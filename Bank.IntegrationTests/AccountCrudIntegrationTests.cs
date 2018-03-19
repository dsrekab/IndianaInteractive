using Bank.Contract.Domain.AccountType;
using Bank.Contract.Services;
using Bank.Data;
using Bank.Services;
using System;
using System.Linq;

namespace Bank.IntegrationTests
{
    public class AccountCrudIntegrationTests : IIntegrationTest
    {
        public void Run()
        {
            AccountMemoryRepository accountRepository = new AccountMemoryRepository();
            TransactionMemoryRepository trxRepository = new TransactionMemoryRepository();

            ITransactionService trxService = new TransactionService(trxRepository, trxRepository, new DateTimeService());
            IAccountService accountService = new AccountService(accountRepository, accountRepository, trxService);

            //Create a new account
            try
            {
                accountService.AddAccount("1234", new Checking(), 1, 1, 0);
            }
            catch (Exception ex)
            {
                throw new Exception($"Exception thrown while CREATING new account.  Exception:{ex.ToString()}");
            }

            //Read from the repo to ensure the account is created
            try
            {
                var accountList = accountService.GetAllAccounts();
                if (accountList.Count()!=1){ throw new Exception($"Expected 1 account, instead found {accountList.Count()}"); }

                var accountByNumber = accountService.GetAccountByAccountNumber("1234");
                if (accountByNumber is null) { throw new Exception("Unable to find account number 1234 after creation"); }

                //ensure there is an opening balance
                var trxList = trxService.GetAllTransactionsByAccountId(accountByNumber.Id);
                if (trxList.Count()!=1) { throw new Exception("Account 1234 was created, however unable to locate its opening balance transaction"); }

                var openingTrx = trxList.FirstOrDefault(m => m.Description == "Opening Account");
                if (openingTrx is null) { throw new Exception("Account 1234 was created and a single transaction was found for it, however the description doesn't match [Opening Account]."); }

            }
            catch (Exception ex)
            {
                throw new Exception($"Exception thrown while reading from the repository after CREATING account.  Exception:{ex.ToString()}");
            }

            //Update the Account to a different owner
            try
            {
                var accountByNumber = accountService.GetAccountByAccountNumber("1234");
                accountByNumber.OwningCustomerId = 2;

                accountService.UpdateAccount(accountByNumber.Id, accountByNumber.AccountType, accountByNumber.OwningCustomerId, accountByNumber.BankId, accountByNumber.IsActive);
            }
            catch (Exception ex)
            {
                throw new Exception($"Exception thrown while Updating account.  Exception:{ex.ToString()}");
            }

            //Read from the repo to ensure the account is updated
            try
            {
                //make sure there is still only 1 account
                var accountList = accountService.GetAllAccounts();
                if (accountList.Count() != 1) { throw new Exception($"Expected 1 account, instead found {accountList.Count()}"); }

                var accountByNumber = accountService.GetAccountByAccountNumber("1234");
                if (accountByNumber is null) { throw new Exception("Unable to find account number 1234 after updating"); }

                //ensure the customer is now 2
                if (accountByNumber.OwningCustomerId!=2)
                {
                    throw new Exception($"Update owning customer id to 2, however database retuns the owning customer id as {accountByNumber.OwningCustomerId}");
                }

            }
            catch (Exception ex)
            {
                throw new Exception($"Exception thrown while reading from the repository after UPDATING account.  Exception:{ex.ToString()}");
            }

            //Delete the Account
            try
            {
                var accountByNumber = accountService.GetAccountByAccountNumber("1234");
                accountService.DeleteAccount(accountByNumber);
            }
            catch (Exception ex)
            {
                throw new Exception($"Exception thrown while DELETING an account.  Exception:{ex.ToString()}");
            }

            //Read from the repo to ensure the account is deleted
            try
            {
                //make sure there is still only 1 account
                var accountList = accountService.GetAllAccounts();
                if (accountList.Count() != 1) { throw new Exception($"Expected 1 account, instead found {accountList.Count()}"); }

                var accountByNumber = accountService.GetAccountByAccountNumber("1234");
                if (accountByNumber is null) { throw new Exception("Unable to find account number 1234 after deletion (account should still be there, just with an IsActive indicator of false)"); }

                //ensure the customer is now 2
                if (accountByNumber.IsActive)
                {
                    throw new Exception("Account number (1234) is still active after deletion.");
                }

            }
            catch (Exception ex)
            {
                throw new Exception($"Exception thrown while reading from the repository after DELETING account.  Exception:{ex.ToString()}");
            }
        }
    }
}
