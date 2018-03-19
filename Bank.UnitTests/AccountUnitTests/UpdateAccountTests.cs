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
    public class UpdateAccountTests
    {
        [TestCase()]
        public void UpdateAccountTest()
        {
            Mock<IRepositoryReader<Account>> mockRepoReader = new Mock<IRepositoryReader<Account>>();
            Mock<IRepositoryWriter<Account>> mockRepoWriter = new Mock<IRepositoryWriter<Account>>();
            Mock<ITransactionService> mockTrxService = new Mock<ITransactionService>();


            mockRepoReader.Setup(m => m.GetAll()).Returns(new List<Account> { new Account { Id = 1 } });

            IAccountService sut = new AccountService(mockRepoReader.Object, mockRepoWriter.Object, mockTrxService.Object);

            sut.UpdateAccount(1, new Checking(), 1, 2, true);

            mockRepoWriter.Verify(m => m.Update(It.IsAny<Account>()), Times.Once);
        }

        [TestCase()]
        public void UpdateAccountNullTest()
        {
            Mock<IRepositoryReader<Account>> mockRepoReader = new Mock<IRepositoryReader<Account>>();
            Mock<IRepositoryWriter<Account>> mockRepoWriter = new Mock<IRepositoryWriter<Account>>();
            Mock<ITransactionService> mockTrxService = new Mock<ITransactionService>();

            mockRepoReader.Setup(m => m.GetAll()).Returns(new List<Account> { new Account { Id = 4 } });

            IAccountService sut = new AccountService(mockRepoReader.Object, mockRepoWriter.Object, mockTrxService.Object);

            var ex = Assert.Throws<Exception>(() => sut.UpdateAccount(1, new Checking(), 1, 2, true));
            Assert.That(ex.Message, Is.EqualTo("Unable to find account for Account Id 1"));
        }
    }
}
