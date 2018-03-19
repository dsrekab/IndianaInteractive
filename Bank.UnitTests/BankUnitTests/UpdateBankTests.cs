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
    public class UpdateBankTests
    {
        [TestCase()]
        public void UpdateBankTest()
        {
            Mock<IRepositoryReader<Contract.Domain.Bank>> mockRepoReader = new Mock<IRepositoryReader<Contract.Domain.Bank>>();
            Mock<IRepositoryWriter<Contract.Domain.Bank>> mockRepoWriter = new Mock<IRepositoryWriter<Contract.Domain.Bank>>();

            mockRepoReader.Setup(m => m.GetById(1)).Returns(new Contract.Domain.Bank { Id = 1, Name = "Bank", IsActive = true });

            IBankService sut = new BankService(mockRepoReader.Object, mockRepoWriter.Object);

            sut.UpdateBank(1, "Bank1", true);

            mockRepoWriter.Verify(m => m.Update(It.IsAny<Contract.Domain.Bank>()), Times.Once);
        }

        [TestCase()]
        public void UpdateDuplicateBankTest()
        {
            Mock<IRepositoryReader<Contract.Domain.Bank>> mockRepoReader = new Mock<IRepositoryReader<Contract.Domain.Bank>>();
            Mock<IRepositoryWriter<Contract.Domain.Bank>> mockRepoWriter = new Mock<IRepositoryWriter<Contract.Domain.Bank>>();

            mockRepoReader.Setup(m => m.GetAll()).Returns(new List<Contract.Domain.Bank> { new Contract.Domain.Bank { Id = 1, Name = "Bank2", IsActive = true } });

            IBankService sut = new BankService(mockRepoReader.Object, mockRepoWriter.Object);

            //ensure updating to an existing name throws an exception
            var ex = Assert.Throws<InvalidDataException>(() => sut.UpdateBank(2, "Bank2", true));
            Assert.That(ex.Message, Is.EqualTo("There is already banked name [Bank2].  All Bank names must be unique."));
        }

        [TestCase("", TestName = "UpdateBankNameIsEmpty")]
        [TestCase(null, TestName = "UpdateBankNameIsNull")]
        public void UpdateEmptyBankTest(string bankName)
        {
            Mock<IRepositoryReader<Contract.Domain.Bank>> mockRepoReader = new Mock<IRepositoryReader<Contract.Domain.Bank>>();
            Mock<IRepositoryWriter<Contract.Domain.Bank>> mockRepoWriter = new Mock<IRepositoryWriter<Contract.Domain.Bank>>();

            IBankService sut = new BankService(mockRepoReader.Object, mockRepoWriter.Object);

            var ex = Assert.Throws<InvalidDataException>(() => sut.UpdateBank(3, bankName, true));
            Assert.That(ex.Message, Is.EqualTo("All banks must have a unique Name."));
        }
    }
}
