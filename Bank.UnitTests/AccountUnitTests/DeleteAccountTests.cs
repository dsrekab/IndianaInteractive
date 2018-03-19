using Bank.Contract.Data;
using Bank.Contract.Domain;
using Bank.Contract.Services;
using Bank.Services;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Bank.UnitTests.AccountUnitTests
{
    [TestFixture]
    public class DeleteAccountTests
    {
        [TestCase(true, TestName ="DeleteAccountSpecifyDate")]
        [TestCase(false, TestName = "DeleteAccountDateTimeNow")]
        public void DeleteAccountTest(bool specifyDate)
        {
            Mock<IRepositoryReader<Account>> mockRepoReader = new Mock<IRepositoryReader<Account>>();
            Mock<IRepositoryWriter<Account>> mockRepoWriter = new Mock<IRepositoryWriter<Account>>();
            Mock<ITransactionService> mockTrxService = new Mock<ITransactionService>();

            mockTrxService.Setup(m => m.GetAllTransactionsByAccountId(1)).Returns(
                    new List<Transaction>
                    {
                        new Transaction{AccountId=1, Amount=15 },
                        new Transaction{AccountId=1, Amount=25 }
                    }
                );

            mockRepoReader.Setup(m => m.GetAll()).Returns(new List<Account> { new Account { Id = 1 } });

            IAccountService sut = new AccountService(mockRepoReader.Object, mockRepoWriter.Object, mockTrxService.Object);

            if (specifyDate)
            {
                DateTime trxDate = new DateTime(2018, 03, 01);

                sut.DeleteAccount(1, trxDate);
                mockTrxService.Verify(m => m.AddTransaction(It.IsAny<int>(), -40, trxDate, "Withdraw existing balance before deleting account."), Times.Once);
            }
            else
            {
                sut.DeleteAccount(1);
                mockTrxService.Verify(m => m.AddTransaction(It.IsAny<int>(), -40, "Withdraw existing balance before deleting account."), Times.Once);
            }
            

            
            mockRepoWriter.Verify(m => m.Delete(It.IsAny<Account>()), Times.Once);
        }
    }
}
