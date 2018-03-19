using Bank.Contract.Data;
using Bank.Contract.Services;
using Bank.Services;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Bank.UnitTests.BankUnitTests
{
    [TestFixture]
    public class ReadBankTests
    {
        [TestCase("Bank1","Bank1", TestName ="Matching Name")]
        [TestCase("Bank2", "baNK2", TestName = "Matching Name - Different Case")]
        public void GetBankByNameTests(string name, string testName)
        {
            Mock<IRepositoryReader<Contract.Domain.Bank>> mockRepoReader = new Mock<IRepositoryReader<Contract.Domain.Bank>>();
            Mock<IRepositoryWriter<Contract.Domain.Bank>> mockRepoWriter = new Mock<IRepositoryWriter<Contract.Domain.Bank>>();

            mockRepoReader.Setup(m => m.GetAll()).Returns(new List<Contract.Domain.Bank> { new Contract.Domain.Bank { Name = name } });

            IBankService sut = new BankService(mockRepoReader.Object, mockRepoWriter.Object);

            Assert.IsNotNull(sut.GetBankByName(testName));
        }

        [TestCase()]
        public void GetAllBanks()
        {
            Mock<IRepositoryReader<Contract.Domain.Bank>> mockRepoReader = new Mock<IRepositoryReader<Contract.Domain.Bank>>();
            Mock<IRepositoryWriter<Contract.Domain.Bank>> mockRepoWriter = new Mock<IRepositoryWriter<Contract.Domain.Bank>>();

            mockRepoReader.Setup(m => m.GetAll()).Returns(new List<Contract.Domain.Bank> { new Contract.Domain.Bank { Name = "Bank1" }, new Contract.Domain.Bank { Name = "Bank2" } });

            IBankService sut = new BankService(mockRepoReader.Object, mockRepoWriter.Object);

            Assert.AreEqual(2, sut.GetAllBanks().Count());
        }
    }
}
