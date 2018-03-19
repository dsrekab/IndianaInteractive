using System;
using System.Collections.Generic;
using System.IO;
using Bank.Contract.Data;
using Bank.Contract.Domain;
using Bank.Contract.Services;
using Bank.Services;
using Moq;
using NUnit.Framework;

namespace Bank.UnitTests.CustomerUnitTests
{
    [TestFixture]
    public class UpdateCustomerTests
    {
        [TestCase()]
        public void UpdateCustomerTest()
        {
            Mock<IRepositoryReader<Customer>> mockRepoReader = new Mock<IRepositoryReader<Customer>>();
            Mock<IRepositoryWriter<Customer>> mockRepoWriter = new Mock<IRepositoryWriter<Customer>>();

            Customer mockReturnCustomer = new Customer { Id = 1, FamilyName = "Customer1", GivenName = "Update1", DateOfBirth = new DateTime(2018, 07, 20), SocialSecurityNumber = "111226666", IsActive = true };

            mockRepoReader.Setup(m => m.GetAll()).Returns(new List<Customer> { mockReturnCustomer });
            mockRepoReader.Setup(m => m.GetById(1)).Returns(mockReturnCustomer);

            ICustomerService sut = new CustomerService(mockRepoReader.Object, mockRepoWriter.Object);

            sut.UpdateCustomer(1, "Customer1","postUpdate1", new DateTime(2018,07,20),"111226666", true);

            mockRepoWriter.Verify(m => m.Update(It.IsAny<Customer>()), Times.Once);
        }

        [TestCase()]
        public void UpdateCustomerDuplicateTest()
        {
            Mock<IRepositoryReader<Customer>> mockRepoReader = new Mock<IRepositoryReader<Customer>>();
            Mock<IRepositoryWriter<Customer>> mockRepoWriter = new Mock<IRepositoryWriter<Customer>>();

            Customer mockReturnCustomer = new Customer { Id = 1, FamilyName = "Customer2", GivenName = "Update2", DateOfBirth = new DateTime(2018, 07, 20), SocialSecurityNumber = "111227777", IsActive = true };

            mockRepoReader.Setup(m => m.GetAll()).Returns(new List<Customer>{mockReturnCustomer});

            ICustomerService sut = new CustomerService(mockRepoReader.Object, mockRepoWriter.Object);

            var ex = Assert.Throws<InvalidDataException>(() => sut.UpdateCustomer(2, "Customer2", "Update2", new DateTime(2018, 07, 20), "111227777", true));
            Assert.That(ex.Message, Is.EqualTo("You cannot update a customer to match the demographics of another customer."));
        }

        [TestCase("", TestName = "UpdateCustomerFamilyNameIsEmpty")]
        [TestCase(null, TestName = "UpdateCustomerFamilyNameIsNull")]
        public void UpdateCustomerBlankFamilyNameTest(string familyName)
        {
            Mock<IRepositoryReader<Customer>> mockRepoReader = new Mock<IRepositoryReader<Customer>>();
            Mock<IRepositoryWriter<Customer>> mockRepoWriter = new Mock<IRepositoryWriter<Customer>>();

            ICustomerService sut = new CustomerService(mockRepoReader.Object, mockRepoWriter.Object);

            var ex = Assert.Throws<InvalidDataException>(() => sut.UpdateCustomer(1, familyName, "given", DateTime.Now, "", true));
            Assert.That(ex.Message, Is.EqualTo("Customer FamilyName is required."));
        }

        [TestCase("", TestName = "UpdateCustomerGivenNameIsEmpty")]
        [TestCase(null, TestName = "UpdateCustomerGivenNameIsNull")]
        public void UpdateCustomerBlankGivenNameTest(string givenName)
        {
            Mock<IRepositoryReader<Customer>> mockRepoReader = new Mock<IRepositoryReader<Customer>>();
            Mock<IRepositoryWriter<Customer>> mockRepoWriter = new Mock<IRepositoryWriter<Customer>>();

            ICustomerService sut = new CustomerService(mockRepoReader.Object, mockRepoWriter.Object);

            var ex = Assert.Throws<InvalidDataException>(() => sut.UpdateCustomer(2,"family", givenName, DateTime.Now, "", true));
            Assert.That(ex.Message, Is.EqualTo("Customer GivenName is required."));
        }

        [TestCase("", TestName = "UpdateCustomerSSNIsEmpty")]
        [TestCase(null, TestName = "UpdateCustomerSSNIsNull")]
        public void UpdateCustomerBlankSsnTest(string ssn)
        {
            Mock<IRepositoryReader<Customer>> mockRepoReader = new Mock<IRepositoryReader<Customer>>();
            Mock<IRepositoryWriter<Customer>> mockRepoWriter = new Mock<IRepositoryWriter<Customer>>();

            ICustomerService sut = new CustomerService(mockRepoReader.Object, mockRepoWriter.Object);

            var ex = Assert.Throws<InvalidDataException>(() => sut.UpdateCustomer(3, "family", "given", DateTime.Now, ssn, true));
            Assert.That(ex.Message, Is.EqualTo("Customer socialSecurityNumber is required."));
        }
    }
}
