using Bank.Contract.Data;
using Bank.Contract.Services;
using Bank.Services;
using Moq;
using NUnit.Framework;

namespace Bank.UnitTests.BankUnitTests
{
    [TestFixture]
    public class DeleteBankTests
    {
        [TestCase()]
        public void DeleteExistingBankById()
        {
            Mock<IRepositoryReader<Contract.Domain.Bank>> mockRepoReader = new Mock<IRepositoryReader<Contract.Domain.Bank>>();
            Mock<IRepositoryWriter<Contract.Domain.Bank>> mockRepoWriter = new Mock<IRepositoryWriter<Contract.Domain.Bank>>();

            mockRepoReader.Setup(m => m.GetById(1)).Returns(new Contract.Domain.Bank { Id = 1, Name = "Bank1", IsActive = true });

            IBankService sut = new BankService(mockRepoReader.Object, mockRepoWriter.Object);

            sut.DeleteBank(1);

            mockRepoWriter.Verify(m => m.Delete(It.IsAny<Contract.Domain.Bank>()), Times.Once);

        }

        [TestCase()]
        public void DeleteExistingBankByEntity()
        {
            Mock<IRepositoryReader<Contract.Domain.Bank>> mockRepoReader = new Mock<IRepositoryReader<Contract.Domain.Bank>>();
            Mock<IRepositoryWriter<Contract.Domain.Bank>> mockRepoWriter = new Mock<IRepositoryWriter<Contract.Domain.Bank>>();

            Contract.Domain.Bank bankToDelete = new Contract.Domain.Bank { Id = 2, Name = "Bank2", IsActive = true };

            IBankService sut = new BankService(mockRepoReader.Object, mockRepoWriter.Object);

            sut.DeleteBank(bankToDelete);

            mockRepoWriter.Verify(m => m.Delete(It.IsAny<Contract.Domain.Bank>()), Times.Once);

        }

        [TestCase()]
        public void DeleteBankIdDoesNotExist()
        {
            Mock<IRepositoryReader<Contract.Domain.Bank>> mockRepoReader = new Mock<IRepositoryReader<Contract.Domain.Bank>>();
            Mock<IRepositoryWriter<Contract.Domain.Bank>> mockRepoWriter = new Mock<IRepositoryWriter<Contract.Domain.Bank>>();

            Contract.Domain.Bank bankToDelete = null;

            mockRepoReader.Setup(m => m.GetById(It.IsAny<int>())).Returns(bankToDelete);

            IBankService sut = new BankService(mockRepoReader.Object, mockRepoWriter.Object);
            
            sut.DeleteBank(2);

            mockRepoWriter.Verify(m => m.Delete(It.IsAny<Contract.Domain.Bank>()), Times.Never);

        }
    }
}
