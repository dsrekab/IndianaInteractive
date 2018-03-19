using Bank.Contract.Data;
using Bank.Contract.Domain;
using Bank.Contract.Domain.AccountType;
using Bank.Contract.Services;
using Bank.Services;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Bank.UnitTests.AccountUnitTests
{
    [TestFixture]
    public class AddAccountTests
    {
        [TestCase("Checking", null, TestName ="AddAccountCheckingNullBalance")]
        [TestCase("CorpInv", null, TestName = "AddAccountCorpInvestmentNullBalance")]
        [TestCase("IndInv", null, TestName = "AddAccountIndividualInvestmentNullBalance")]
        [TestCase("Checking", 10, TestName = "AddAccountCheckingStartingBalance")]
        [TestCase("CorpInv", 20, TestName = "AddAccountCorpInvestmentStartingBalance")]
        [TestCase("IndInv", 30, TestName = "AddAccountIndividualInvestmentStartingBalance")]
        public void AddAccountTest(string accountTypeString, decimal? startBalance)
        {
            Mock<IRepositoryReader<Account>> mockRepoReader = new Mock<IRepositoryReader<Account>>();
            Mock<IRepositoryWriter<Account>> mockRepoWriter = new Mock<IRepositoryWriter<Account>>();
            Mock<ITransactionService> mockTrxService = new Mock<ITransactionService>();

            IAccountType accountType=null;
            switch(accountTypeString)
            {
                case "Checking":
                    accountType = new Checking();
                    break;
                case "CorpInv":
                    accountType = new CorporateInvestment();
                    break;
                case "IndInv":
                    accountType = new IndividualInvestment();
                    break;
            }

            mockRepoReader.Setup(m => m.GetAll()).Returns(
                new List<Account> {
                    new Account { Id = 1, AccountNumber = "12345", AccountType = accountType, BankId=1, OwningCustomerId=1, IsActive=true  }
                });

            IAccountService sut = new AccountService(mockRepoReader.Object, mockRepoWriter.Object, mockTrxService.Object);

            if (startBalance.HasValue)
            {
                sut.AddAccount("12345", accountType, 1, 1, startBalance.Value);
                mockTrxService.Verify(m => m.AddTransaction(It.IsAny<int>(), It.IsAny<decimal>(), "Opening Account"), Times.Once);
            }
            else
            {
                sut.AddAccount("12345", accountType, 1, 1);
                mockTrxService.Verify(m => m.AddTransaction(It.IsAny<int>(), 0, "Opening Account"), Times.Once);
            }
            

            mockRepoWriter.Verify(m => m.Add(It.IsAny<Account>()), Times.Once);
            
        }

        [TestCase()]
        public void AddAccountFailedTest()
        {
            Mock<IRepositoryReader<Account>> mockRepoReader = new Mock<IRepositoryReader<Account>>();
            Mock<IRepositoryWriter<Account>> mockRepoWriter = new Mock<IRepositoryWriter<Account>>();
            Mock<ITransactionService> mockTrxService = new Mock<ITransactionService>();

            IEnumerable<Account> acctList = null;
            mockRepoReader.Setup(m => m.GetAll()).Returns(acctList);

            IAccountService sut = new AccountService(mockRepoReader.Object, mockRepoWriter.Object, mockTrxService.Object);

            var ex = Assert.Throws<Exception>(() => sut.AddAccount("222",new Checking(),1,1,0));
            Assert.That(ex.Message, Is.EqualTo("Account Number 222 was not successfully added to the database for unknown reasons."));

        }
    }
}
