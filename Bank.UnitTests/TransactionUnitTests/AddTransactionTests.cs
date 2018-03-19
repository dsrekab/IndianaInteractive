using Bank.Contract.Data;
using Bank.Contract.Domain;
using Bank.Contract.Services;
using Bank.Services;
using Moq;
using NUnit.Framework;
using System;

namespace Bank.UnitTests.TransactionUnitTests
{
    [TestFixture]
    public class AddTransactionTests
    {
        [TestCase(null, TestName ="AddTransactionWithNullDescription")]
        [TestCase("", TestName = "AddTransactionWithEmptyDescription")]
        [TestCase("test", TestName = "AddTransactionWithValidDescription")]
        public void AddTransactionTest(string description)
        {
            Mock<IRepositoryReader<Transaction>> mockRepoReader = new Mock<IRepositoryReader<Transaction>>();
            Mock<IRepositoryWriter<Transaction>> mockRepoWriter = new Mock<IRepositoryWriter<Transaction>>();
            Mock<IDateTimeService> dateTimeMock = new Mock<IDateTimeService>();

            ITransactionService sut = new TransactionService(mockRepoReader.Object, mockRepoWriter.Object, dateTimeMock.Object);

            sut.AddTransaction(1, 1, description);

            mockRepoWriter.Verify(m => m.Add(It.IsAny<Transaction>()), Times.Once);
        }

        [TestCase(null, TestName = "AddTransactionWithDateAndNullDescription")]
        [TestCase("", TestName = "AddTransactionWithDateAndEmptyDescription")]
        [TestCase("test", TestName = "AddTransactionWithDateAndValidDescription")]
        public void AddTransactionWithDateTest(string description)
        {
            Mock<IRepositoryReader<Transaction>> mockRepoReader = new Mock<IRepositoryReader<Transaction>>();
            Mock<IRepositoryWriter<Transaction>> mockRepoWriter = new Mock<IRepositoryWriter<Transaction>>();
            Mock<IDateTimeService> dateTimeMock = new Mock<IDateTimeService>();

            ITransactionService sut = new TransactionService(mockRepoReader.Object, mockRepoWriter.Object, dateTimeMock.Object);

            sut.AddTransaction(1, 1, DateTime.Now, description);

            mockRepoWriter.Verify(m => m.Add(It.IsAny<Transaction>()), Times.Once);
        }
    }
}
