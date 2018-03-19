using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bank.Contract.Data;
using Bank.Contract.Domain;
using Bank.Contract.Services;
using Bank.Services;
using Moq;
using NUnit.Framework;

namespace Bank.UnitTests.TransactionUnitTests
{
    [TestFixture]
    public class ReadTransactionUnitTests
    {
        [TestCase()]
        public void ReadAllTest()
        {
            Mock<IRepositoryReader<Transaction>> mockRepoReader = new Mock<IRepositoryReader<Transaction>>();
            Mock<IRepositoryWriter<Transaction>> mockRepoWriter = new Mock<IRepositoryWriter<Transaction>>();
            Mock<IDateTimeService> dateTimeMock = new Mock<IDateTimeService>();

            mockRepoReader.Setup(m => m.GetAll()).Returns(
                new List<Transaction>()
                {
                    new Transaction{Id=1, AccountId=1,Amount=11,DateTime=new DateTime(2018,03,18),Description="AcctId1 first Trans"},
                    new Transaction{Id=2, AccountId=2,Amount=22,DateTime=new DateTime(2018,03,19),Description="AcctId2 first Trans"},
                    new Transaction{Id=3, AccountId=1,Amount=33,DateTime=new DateTime(2018,03,20),Description="AcctId1 second Trans"},
                    new Transaction{Id=4, AccountId=1,Amount=33,DateTime=new DateTime(2018,03,21),Description="AcctId1 third Trans"}
                });


            ITransactionService sut = new TransactionService(mockRepoReader.Object, mockRepoWriter.Object, dateTimeMock.Object);

            Assert.AreEqual(4, sut.GetAllTransactions().Count());

        }

        [TestCase()]
        public void ReadByAccountIdTest()
        {
            Mock<IRepositoryReader<Transaction>> mockRepoReader = new Mock<IRepositoryReader<Transaction>>();
            Mock<IRepositoryWriter<Transaction>> mockRepoWriter = new Mock<IRepositoryWriter<Transaction>>();
            Mock<IDateTimeService> dateTimeMock = new Mock<IDateTimeService>();

            mockRepoReader.Setup(m => m.GetAll()).Returns(
                new List<Transaction>()
                {
                    new Transaction{Id=1, AccountId=1,Amount=11,DateTime=new DateTime(2018,03,18),Description="AcctId1 first Trans"},
                    new Transaction{Id=2, AccountId=2,Amount=22,DateTime=new DateTime(2018,03,19),Description="AcctId2 first Trans"},
                    new Transaction{Id=3, AccountId=1,Amount=33,DateTime=new DateTime(2018,03,20),Description="AcctId1 second Trans"},
                    new Transaction{Id=4, AccountId=1,Amount=33,DateTime=new DateTime(2018,03,21),Description="AcctId1 third Trans"}
                });


            ITransactionService sut = new TransactionService(mockRepoReader.Object, mockRepoWriter.Object, dateTimeMock.Object);

            Assert.AreEqual(3, sut.GetAllTransactionsByAccountId(1).Count());
            Assert.AreEqual(1, sut.GetAllTransactionsByAccountId(2).Count());

        }

        [TestCase()]
        public void ReadByAccountIdBeforeDateTest()
        {
            Mock<IRepositoryReader<Transaction>> mockRepoReader = new Mock<IRepositoryReader<Transaction>>();
            Mock<IRepositoryWriter<Transaction>> mockRepoWriter = new Mock<IRepositoryWriter<Transaction>>();
            Mock<IDateTimeService> dateTimeMock = new Mock<IDateTimeService>();

            mockRepoReader.Setup(m => m.GetAll()).Returns(
                new List<Transaction>()
                {
                    new Transaction{Id=1, AccountId=1,Amount=11,DateTime=new DateTime(2018,03,18),Description="AcctId1 first Trans"},
                    new Transaction{Id=2, AccountId=2,Amount=22,DateTime=new DateTime(2018,03,19),Description="AcctId2 first Trans"},
                    new Transaction{Id=3, AccountId=1,Amount=33,DateTime=new DateTime(2018,03,20),Description="AcctId1 second Trans"},
                    new Transaction{Id=4, AccountId=1,Amount=33,DateTime=new DateTime(2018,03,21),Description="AcctId1 third Trans"}
                });


            ITransactionService sut = new TransactionService(mockRepoReader.Object, mockRepoWriter.Object, dateTimeMock.Object);

            Assert.AreEqual(2, sut.GetAllTransactionsByAccountIdAndDateRange(1, null, new DateTime(2018, 03, 20)).Count());
            Assert.AreEqual(0, sut.GetAllTransactionsByAccountIdAndDateRange(2, null, new DateTime(2018, 03, 18)).Count());

        }

        [TestCase()]
        public void ReadByAccountIdAfterDateTest()
        {
            Mock<IRepositoryReader<Transaction>> mockRepoReader = new Mock<IRepositoryReader<Transaction>>();
            Mock<IRepositoryWriter<Transaction>> mockRepoWriter = new Mock<IRepositoryWriter<Transaction>>();
            Mock<IDateTimeService> dateTimeMock = new Mock<IDateTimeService>();

            mockRepoReader.Setup(m => m.GetAll()).Returns(
                new List<Transaction>()
                {
                    new Transaction{Id=1, AccountId=1,Amount=11,DateTime=new DateTime(2018,03,18),Description="AcctId1 first Trans"},
                    new Transaction{Id=2, AccountId=2,Amount=22,DateTime=new DateTime(2018,03,19),Description="AcctId2 first Trans"},
                    new Transaction{Id=3, AccountId=1,Amount=33,DateTime=new DateTime(2018,03,20),Description="AcctId1 second Trans"},
                    new Transaction{Id=4, AccountId=1,Amount=33,DateTime=new DateTime(2018,03,21),Description="AcctId1 third Trans"}
                });


            ITransactionService sut = new TransactionService(mockRepoReader.Object, mockRepoWriter.Object, dateTimeMock.Object);

            Assert.AreEqual(2, sut.GetAllTransactionsByAccountIdAndDateRange(1, new DateTime(2018, 03, 20), null).Count());
            Assert.AreEqual(0, sut.GetAllTransactionsByAccountIdAndDateRange(2, new DateTime(2018, 03, 20), null).Count());

        }

        [TestCase()]
        public void ReadByAccountIdBetweenDatesTest()
        {
            Mock<IRepositoryReader<Transaction>> mockRepoReader = new Mock<IRepositoryReader<Transaction>>();
            Mock<IRepositoryWriter<Transaction>> mockRepoWriter = new Mock<IRepositoryWriter<Transaction>>();
            Mock<IDateTimeService> dateTimeMock = new Mock<IDateTimeService>();

            mockRepoReader.Setup(m => m.GetAll()).Returns(
                new List<Transaction>()
                {
                    new Transaction{Id=1, AccountId=1,Amount=11,DateTime=new DateTime(2018,03,18),Description="AcctId1 first Trans"},
                    new Transaction{Id=2, AccountId=2,Amount=22,DateTime=new DateTime(2018,03,19),Description="AcctId2 first Trans"},
                    new Transaction{Id=3, AccountId=1,Amount=33,DateTime=new DateTime(2018,03,20),Description="AcctId1 second Trans"},
                    new Transaction{Id=4, AccountId=1,Amount=33,DateTime=new DateTime(2018,03,21),Description="AcctId1 third Trans"}
                });


            ITransactionService sut = new TransactionService(mockRepoReader.Object, mockRepoWriter.Object, dateTimeMock.Object);

            Assert.AreEqual(1, sut.GetAllTransactionsByAccountIdAndDateRange(1, new DateTime(2018, 03, 19), new DateTime(2018, 03, 20)).Count());
            Assert.AreEqual(1, sut.GetAllTransactionsByAccountIdAndDateRange(2, new DateTime(2018, 03, 19), new DateTime(2018, 03, 20)).Count());

        }

        [TestCase()]
        public void ReadByAccountIdNullDatesTest()
        {
            Mock<IRepositoryReader<Transaction>> mockRepoReader = new Mock<IRepositoryReader<Transaction>>();
            Mock<IRepositoryWriter<Transaction>> mockRepoWriter = new Mock<IRepositoryWriter<Transaction>>();
            Mock<IDateTimeService> dateTimeMock = new Mock<IDateTimeService>();

            mockRepoReader.Setup(m => m.GetAll()).Returns(
                new List<Transaction>()
                {
                    new Transaction{Id=1, AccountId=1,Amount=11,DateTime=new DateTime(2018,03,18),Description="AcctId1 first Trans"},
                    new Transaction{Id=2, AccountId=2,Amount=22,DateTime=new DateTime(2018,03,19),Description="AcctId2 first Trans"},
                    new Transaction{Id=3, AccountId=1,Amount=33,DateTime=new DateTime(2018,03,20),Description="AcctId1 second Trans"},
                    new Transaction{Id=4, AccountId=1,Amount=33,DateTime=new DateTime(2018,03,21),Description="AcctId1 third Trans"}
                });


            ITransactionService sut = new TransactionService(mockRepoReader.Object, mockRepoWriter.Object, dateTimeMock.Object);

            var ex = Assert.Throws<InvalidDataException>(() => sut.GetAllTransactionsByAccountIdAndDateRange(1, null, null));
            Assert.That(ex.Message, Is.EqualTo("When searching by Account and Date Range a begin date or an end date must be supplied."));
        }
    }
}
