using System.Collections.Generic;
using System.IO;
using Bank.Contract.Data;
using Bank.Contract.Services;
using Bank.Services;
using Moq;
using NUnit.Framework;

namespace Bank.UnitTests.BankUnitTests
{
    [TestFixture]
    public class AddBankTests
    {
        [TestCase()]
        public void AddBankTest()
        {
            Mock<IRepositoryReader<Contract.Domain.Bank>> mockRepoReader = new Mock<IRepositoryReader<Contract.Domain.Bank>>();
            Mock<IRepositoryWriter<Contract.Domain.Bank>> mockRepoWriter = new Mock<IRepositoryWriter<Contract.Domain.Bank>>();

            IBankService sut = new BankService(mockRepoReader.Object, mockRepoWriter.Object);

            sut.AddBank("Bank1");

            mockRepoWriter.Verify(m => m.Add(It.IsAny<Contract.Domain.Bank>()), Times.Once);
        }

        [TestCase()]
        public void AddDuplicateBankTest()
        {
            Mock<IRepositoryReader<Contract.Domain.Bank>> mockRepoReader = new Mock<IRepositoryReader<Contract.Domain.Bank>>();
            Mock<IRepositoryWriter<Contract.Domain.Bank>> mockRepoWriter = new Mock<IRepositoryWriter<Contract.Domain.Bank>>();

            mockRepoReader.Setup(m => m.GetAll()).Returns(new List<Contract.Domain.Bank> { new Contract.Domain.Bank { Id = 1, Name = "Bank2", IsActive = true } });

            IBankService sut = new BankService(mockRepoReader.Object, mockRepoWriter.Object);

            //ensure the second add throws an exception
            var ex = Assert.Throws<InvalidDataException>(() => sut.AddBank("Bank2"));
            Assert.That(ex.Message, Is.EqualTo("There is already banked name [Bank2].  All Bank names must be unique."));
        }

        [TestCase("", TestName = "AddBankNameIsEmpty")]
        [TestCase(null, TestName = "AddBankNameIsNull")]
        public void AddEmptyBankTest(string bankName)
        {
            Mock<IRepositoryReader<Contract.Domain.Bank>> mockRepoReader = new Mock<IRepositoryReader<Contract.Domain.Bank>>();
            Mock<IRepositoryWriter<Contract.Domain.Bank>> mockRepoWriter = new Mock<IRepositoryWriter<Contract.Domain.Bank>>();

            IBankService sut = new BankService(mockRepoReader.Object, mockRepoWriter.Object);

            var ex = Assert.Throws<InvalidDataException>(() => sut.AddBank(bankName));
            Assert.That(ex.Message, Is.EqualTo("All banks must have a unique Name."));
        }

    }
}
