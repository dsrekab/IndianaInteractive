using System.Collections.Generic;
using Bank.Contract.Data;
using Bank.Contract.Domain;
using Bank.Contract.Services;
using Bank.Services;
using Moq;
using NUnit.Framework;

namespace Bank.UnitTests.AccountUnitTests
{
    [TestFixture]
    public class ReadAccountTests
    {
        [TestCase()]
        public void GetAllAccountsTest()
        {
            Mock<IRepositoryReader<Account>> mockRepoReader = new Mock<IRepositoryReader<Account>>();
            Mock<IRepositoryWriter<Account>> mockRepoWriter = new Mock<IRepositoryWriter<Account>>();
            Mock<ITransactionService> mockTrxService = new Mock<ITransactionService>();

            IAccountService sut = new AccountService(mockRepoReader.Object, mockRepoWriter.Object, mockTrxService.Object);

            var accts = sut.GetAllAccounts();

            mockRepoReader.Verify(m => m.GetAll(), Times.Once);
        }

        [TestCase(true, TestName = "GetAccountByNumberTestNullReturn")]
        [TestCase(false, TestName = "GetAccountByNumberTestValidReturn")]
        public void GetAccountByNumberTest(bool returnNull)
        {
            Mock<IRepositoryReader<Account>> mockRepoReader = new Mock<IRepositoryReader<Account>>();
            Mock<IRepositoryWriter<Account>> mockRepoWriter = new Mock<IRepositoryWriter<Account>>();
            Mock<ITransactionService> mockTrxService = new Mock<ITransactionService>();

            if (returnNull)
            {
                List<Account> acctList = null;
                mockRepoReader.Setup(m => m.GetAll()).Returns(acctList);
            }
            else
            {
                mockRepoReader.Setup(m => m.GetAll()).Returns(new List<Account> { new Account { Id = 1, AccountNumber = "111" } });
            }
            

            IAccountService sut = new AccountService(mockRepoReader.Object, mockRepoWriter.Object, mockTrxService.Object);

            var accts = sut.GetAccountByAccountNumber("111");

            mockRepoReader.Verify(m => m.GetAll(), Times.Once);
        }

        [TestCase()]
        public void GetAccountBalanceTest()
        {
            Mock<IRepositoryReader<Account>> mockRepoReader = new Mock<IRepositoryReader<Account>>();
            Mock<IRepositoryWriter<Account>> mockRepoWriter = new Mock<IRepositoryWriter<Account>>();
            Mock<ITransactionService> mockTrxService = new Mock<ITransactionService>();

            mockTrxService.Setup(m => m.GetAllTransactionsByAccountId(1)).Returns(
                    new List<Transaction>
                    {
                        new Transaction{AccountId=1, Amount=16 },
                        new Transaction{AccountId=1, Amount=26 }
                    }
                );

            mockRepoReader.Setup(m => m.GetAll()).Returns(new List<Account> { new Account { Id = 1 } });

            IAccountService sut = new AccountService(mockRepoReader.Object, mockRepoWriter.Object, mockTrxService.Object);

            var balance = sut.GetAccountBalance(1);

            mockTrxService.Verify(m => m.GetAllTransactionsByAccountId(1), Times.Once);
            Assert.AreEqual(42, balance);
        }
    }
}
