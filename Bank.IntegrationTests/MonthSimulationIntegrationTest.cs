using Bank.Contract.Domain;
using Bank.Contract.Domain.AccountType;
using Bank.Contract.Services;
using Bank.Data;
using Bank.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.IntegrationTests
{
    public class MonthSimulationIntegrationTest : IIntegrationTest
    {
        public void Run()
        {
            Mock<IDateTimeService> mockDateTimeService = new Mock<IDateTimeService>();

            BankMemoryRepository bankRepository = new BankMemoryRepository();
            CustomerMemoryRepository customerRepository = new CustomerMemoryRepository();
            AccountMemoryRepository accountRepository = new AccountMemoryRepository();
            TransactionMemoryRepository trxRepository = new TransactionMemoryRepository();

            IBankService bankService = new BankService(bankRepository, bankRepository);
            ICustomerService customerService = new CustomerService(customerRepository, customerRepository);
            ITransactionService trxService = new TransactionService(trxRepository, trxRepository, mockDateTimeService.Object);
            IAccountService accountService = new AccountService(accountRepository, accountRepository, trxService);

            //Create a bank
            bankService.AddBank("Simulation Bank");
            Contract.Domain.Bank bank = bankService.GetBankByName("Simulation Bank");

            if (bank is null) { throw new Exception("Unable to create and retrieve new bank (Simulation Bank)"); }

            //Create a customer
            customerService.AddCustomer("Simulation", "Tracy", new DateTime(1980, 10, 7), "987654321");
            Customer customer1 = customerService.GetCustomerBySsn("987654321");

            if (customer1 is null) { throw new Exception("Unable to create and retrieve new customer (SSN = 987654321)"); }

            //Add a checking account
            mockDateTimeService.Setup(m => m.Now()).Returns(new DateTime(2018, 03, 01));
            accountService.AddAccount("234", new Checking(), customer1.Id, bank.Id, 500);
            Account checkingAcct = accountService.GetAccountByAccountNumber("234");

            if (checkingAcct is null) { throw new Exception("Unable to create and retrieve new checking account (accountNumber = 234)"); }

            decimal currentCheckingBalance = accountService.GetAccountBalance(checkingAcct.Id);
            if (currentCheckingBalance != 500) { throw new Exception($"Unexpected account balance.  Expected 500, found {currentCheckingBalance}"); }

            //Deposit a check
            accountService.DepositToAccount(checkingAcct.Id, 1200, new DateTime(2018, 03, 01), "Deposited check number 9231423");
            currentCheckingBalance = accountService.GetAccountBalance(checkingAcct.Id);
            if (currentCheckingBalance != 1700) { throw new Exception($"Unexpected account balance.  Expected 1700, found {currentCheckingBalance}"); }


            //Add a corporate investment account
            mockDateTimeService.Setup(m => m.Now()).Returns(new DateTime(2018, 03, 02));
            accountService.AddAccount("345", new CorporateInvestment(), customer1.Id, bank.Id, 10000);
            Account corpInvestAcct = accountService.GetAccountByAccountNumber("345");

            if (corpInvestAcct is null) { throw new Exception("Unable to create and retrieve new corporate investment account (accountNumber = 345)"); }

            decimal currentCorpInvBalance = accountService.GetAccountBalance(corpInvestAcct.Id);
            if (currentCorpInvBalance != 10000) { throw new Exception($"Unexpected account balance.  Expected 10000, found {currentCorpInvBalance}"); }

            //Add an individual investment account
            mockDateTimeService.Setup(m => m.Now()).Returns(new DateTime(2018, 03, 04));
            accountService.AddAccount("456", new IndividualInvestment(), customer1.Id, bank.Id, 5000);
            Account indInvestAcct = accountService.GetAccountByAccountNumber("456");

            if (indInvestAcct is null) { throw new Exception("Unable to create and retrieve new individual investment account (accountNumber = 456)"); }

            decimal currentIndInvBalance = accountService.GetAccountBalance(indInvestAcct.Id);
            if (currentIndInvBalance != 5000) { throw new Exception($"Unexpected account balance.  Expected 5000, found {currentIndInvBalance}"); }

            
            //withdraw a check
            accountService.WithdrawalFromAccount(checkingAcct.Id, 300, new DateTime(2018, 03, 09), "Wrote check number 2356784");
            currentCheckingBalance = accountService.GetAccountBalance(checkingAcct.Id);
            if (currentCheckingBalance != 1400) { throw new Exception($"Unexpected account balance.  Expected 1400, found {currentCheckingBalance}"); }

            //Deposit to individual Investment Acct
            accountService.DepositToAccount(indInvestAcct.Id, 300, new DateTime(2018, 03, 10), "Deposited to Individual Investment Acct");
            currentIndInvBalance = accountService.GetAccountBalance(indInvestAcct.Id);
            if (currentIndInvBalance != 5300) { throw new Exception($"Unexpected account balance.  Expected 5300, found {currentIndInvBalance}"); }

            //Deposit to Corp inv Acct
            accountService.DepositToAccount(corpInvestAcct.Id, 5500, new DateTime(2018, 03, 15), "Added investment funds");
            currentCorpInvBalance = accountService.GetAccountBalance(corpInvestAcct.Id);
            if (currentCorpInvBalance != 15500) { throw new Exception($"Unexpected account balance.  Expected 15500, found {currentCorpInvBalance}"); }

            //Valid withdraw from individual investment
            accountService.WithdrawalFromAccount(indInvestAcct.Id, 700, new DateTime(2018, 03, 17), "Withdrawal from Individual Investment Acct");
            currentIndInvBalance = accountService.GetAccountBalance(indInvestAcct.Id);
            if (currentIndInvBalance != 4600) { throw new Exception($"Unexpected account balance.  Expected 4600, found {currentIndInvBalance}"); }

            //Withdraw from Corp inv Acct
            accountService.WithdrawalFromAccount(corpInvestAcct.Id, 2500, new DateTime(2018, 03, 20), "Withdraw 1100 from corporate investment");
            currentCorpInvBalance = accountService.GetAccountBalance(corpInvestAcct.Id);
            if (currentCorpInvBalance != 13000) { throw new Exception($"Unexpected account balance.  Expected 13000, found {currentCorpInvBalance}"); }

            //Transfer from checking to corp invest acct
            accountService.TransferBetweenAccount(checkingAcct.Id, corpInvestAcct.Id, 200, new DateTime(2018, 03, 28), "Transfering from checking to corp investment");

            currentCheckingBalance = accountService.GetAccountBalance(checkingAcct.Id);
            if (currentCheckingBalance != 1200) { throw new Exception($"Unexpected account balance.  Expected 1200, found {currentCheckingBalance}"); }

            currentCorpInvBalance = accountService.GetAccountBalance(corpInvestAcct.Id);
            if (currentCorpInvBalance != 13200) { throw new Exception($"Unexpected account balance.  Expected 13200, found {currentCorpInvBalance}"); }

            //Attempt to withdraw 1100 from checking - should be fine
            accountService.WithdrawalFromAccount(checkingAcct.Id, 1100, new DateTime(2018, 03, 29), "Wrote check number 6785491");
            currentCheckingBalance = accountService.GetAccountBalance(checkingAcct.Id);
            if (currentCheckingBalance != 100) { throw new Exception($"Unexpected account balance.  Expected 100, found {currentCheckingBalance}"); }

            //Attempt to withdraw 1100 from corp investment acct - should be fine
            accountService.WithdrawalFromAccount(corpInvestAcct.Id, 1100, new DateTime(2018, 03, 29), "withdraw 1100 from corp investment");
            currentCorpInvBalance = accountService.GetAccountBalance(corpInvestAcct.Id);
            if (currentCorpInvBalance != 12100) { throw new Exception($"Unexpected account balance.  Expected 12100, found {currentCorpInvBalance}"); }

            //Attempt to withdraw 1100 from individual investment acct - should throw exception
            try
            {
                accountService.WithdrawalFromAccount(indInvestAcct.Id, 1100, new DateTime(2018, 03, 29), "Withdraw 11 from Individual Investment Acct");
            }
            catch (Exception ex)
            {
                if (ex.Message!= "Account Types of Individual Investment are limited to a dollar amount of 1000 in a single transaction")
                {
                    throw new Exception($"Unexpected exception message received attempting to withdraw too much from individual investment account.  Unexpected Exception: {ex.Message}");
                }
            }
            //balance should be unchanged and should match balance from before the attempt
            currentIndInvBalance = accountService.GetAccountBalance(indInvestAcct.Id);
            if (currentIndInvBalance != 4600) { throw new Exception($"Unexpected account balance.  Expected 4600, found {currentIndInvBalance}"); }



            //Attempt to overdraft checking account
            try
            {
                accountService.WithdrawalFromAccount(checkingAcct.Id, 1000000, new DateTime(2018, 03, 30), "Wrote check number 89756423");
            }
            catch (Exception ex)
            {
                if (ex.Message != "Withdrawal amount exceeds current account balance.  Overdrafting not allowed.")
                {
                    throw new Exception($"Unexpected exception message received attempting to overdraft account.  Unexpected Exception: {ex.Message}");
                }
            }
            //balance should be unchanged and should match balance from before the attempt
            currentCheckingBalance = accountService.GetAccountBalance(checkingAcct.Id);
            if (currentCheckingBalance != 100) { throw new Exception($"Unexpected account balance.  Expected 100, found {currentCheckingBalance}"); }


            //Attempt to overdraft Corporate Investment account
            try
            {
                accountService.WithdrawalFromAccount(checkingAcct.Id, 1000000, new DateTime(2018, 03, 30), "Withdraw 1000000 from corp investment acct");
            }
            catch (Exception ex)
            {
                if (ex.Message != "Withdrawal amount exceeds current account balance.  Overdrafting not allowed.")
                {
                    throw new Exception($"Unexpected exception message received attempting to overdraft account.  Unexpected Exception: {ex.Message}");
                }
            }
            //balance should be unchanged and should match balance from before the attempt
            currentCorpInvBalance = accountService.GetAccountBalance(corpInvestAcct.Id);
            if (currentCorpInvBalance != 12100) { throw new Exception($"Unexpected account balance.  Expected 12100, found {currentCorpInvBalance}"); }


            //withdraw down individual account in order to test overdraft instead of individual transaction limit
            accountService.WithdrawalFromAccount(indInvestAcct.Id, 900, new DateTime(2018, 03, 30), "Withdraw 900 from Individual Investment Acct");
            currentIndInvBalance = accountService.GetAccountBalance(indInvestAcct.Id);
            if (currentIndInvBalance != 3700) { throw new Exception($"Unexpected account balance.  Expected 3700, found {currentIndInvBalance}"); }

            accountService.WithdrawalFromAccount(indInvestAcct.Id, 900, new DateTime(2018, 03, 30), "Withdraw 900 from Individual Investment Acct");
            currentIndInvBalance = accountService.GetAccountBalance(indInvestAcct.Id);
            if (currentIndInvBalance != 2800) { throw new Exception($"Unexpected account balance.  Expected 2800, found {currentIndInvBalance}"); }

            accountService.WithdrawalFromAccount(indInvestAcct.Id, 900, new DateTime(2018, 03, 30), "Withdraw 900 from Individual Investment Acct");
            currentIndInvBalance = accountService.GetAccountBalance(indInvestAcct.Id);
            if (currentIndInvBalance != 1900) { throw new Exception($"Unexpected account balance.  Expected 1900, found {currentIndInvBalance}"); }

            accountService.WithdrawalFromAccount(indInvestAcct.Id, 900, new DateTime(2018, 03, 30), "Withdraw 900 from Individual Investment Acct");
            currentIndInvBalance = accountService.GetAccountBalance(indInvestAcct.Id);
            if (currentIndInvBalance != 1000) { throw new Exception($"Unexpected account balance.  Expected 1000, found {currentIndInvBalance}"); }

            accountService.WithdrawalFromAccount(indInvestAcct.Id, 900, new DateTime(2018, 03, 30), "Withdraw 900 from Individual Investment Acct");
            currentIndInvBalance = accountService.GetAccountBalance(indInvestAcct.Id);
            if (currentIndInvBalance != 100) { throw new Exception($"Unexpected account balance.  Expected 100, found {currentIndInvBalance}"); }


            //Attempt to overdraft Individual Investment account
            try
            {
                accountService.WithdrawalFromAccount(checkingAcct.Id, 900, new DateTime(2018, 03, 30), "Withdraw 900 from individual investment acct");
            }
            catch (Exception ex)
            {
                if (ex.Message != "Withdrawal amount exceeds current account balance.  Overdrafting not allowed.")
                {
                    throw new Exception($"Unexpected exception message received attempting to overdraft account.  Unexpected Exception: {ex.Message}");
                }
            }
            //balance should be unchanged and should match balance from before the attempt
            currentIndInvBalance = accountService.GetAccountBalance(indInvestAcct.Id);
            if (currentIndInvBalance != 100) { throw new Exception($"Unexpected account balance.  Expected 100, found {currentIndInvBalance}"); }
        }
    }
}
