using System;
using Bank.Contract.Data;
using Bank.Contract.Domain;
using Bank.Contract.Services;
using Bank.Services;
using Moq;
using NUnit.Framework;

namespace Bank.UnitTests.CustomerUnitTests
{
    [TestFixture]
    public class DeleteCustomerTests
    {
        [TestCase()]
        public void DeleteExistingCustomerById()
        {
            Mock<IRepositoryReader<Customer>> mockRepoReader = new Mock<IRepositoryReader<Customer>>();
            Mock<IRepositoryWriter<Customer>> mockRepoWriter = new Mock<IRepositoryWriter<Customer>>();

            mockRepoReader.Setup(m => m.GetById(1)).Returns(new Customer { Id = 1, FamilyName = "Customer1", GivenName="given1", DateOfBirth=DateTime.Now, SocialSecurityNumber="111224444", IsActive = true });

            ICustomerService sut = new CustomerService(mockRepoReader.Object, mockRepoWriter.Object);

            sut.DeleteCustomer(1);

            mockRepoWriter.Verify(m => m.Delete(It.IsAny<Customer>()), Times.Once);
        }

        [TestCase()]
        public void DeleteExistingCustomerByEntity()
        {
            Mock<IRepositoryReader<Customer>> mockRepoReader = new Mock<IRepositoryReader<Customer>>();
            Mock<IRepositoryWriter<Customer>> mockRepoWriter = new Mock<IRepositoryWriter<Customer>>();

            Customer custToDelete = new Customer { Id = 1, FamilyName = "Customer2", GivenName = "given2", DateOfBirth = DateTime.Now, SocialSecurityNumber = "111225555", IsActive = true };

            ICustomerService sut = new CustomerService(mockRepoReader.Object, mockRepoWriter.Object);

            sut.DeleteCustomer(custToDelete);

            mockRepoWriter.Verify(m => m.Delete(It.IsAny<Customer>()), Times.Once);
        }

        [TestCase()]
        public void DeleteExistingCustomerDoesNotExist()
        {
            Mock<IRepositoryReader<Customer>> mockRepoReader = new Mock<IRepositoryReader<Customer>>();
            Mock<IRepositoryWriter<Customer>> mockRepoWriter = new Mock<IRepositoryWriter<Customer>>();

            Customer custToDelete = null;
            mockRepoReader.Setup(m => m.GetById(2)).Returns(custToDelete);

            ICustomerService sut = new CustomerService(mockRepoReader.Object, mockRepoWriter.Object);

            sut.DeleteCustomer(2);

            mockRepoWriter.Verify(m => m.Delete(It.IsAny<Customer>()), Times.Never);
        }
    }
}
