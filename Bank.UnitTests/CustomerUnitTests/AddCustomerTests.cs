using NUnit.Framework;
using Moq;
using Bank.Contract.Data;
using Bank.Contract.Domain;
using Bank.Contract.Services;
using Bank.Services;
using System;
using System.Collections.Generic;
using System.IO;

namespace Bank.UnitTests.CustomerUnitTests
{
    [TestFixture]
    public class AddCustomerTests
    {
        [TestCase()]
        public void AddCustomerTest()
        {
            Mock<IRepositoryReader<Customer>> mockRepoReader = new Mock<IRepositoryReader<Customer>>();
            Mock<IRepositoryWriter<Customer>> mockRepoWriter = new Mock<IRepositoryWriter<Customer>>();

            ICustomerService sut = new CustomerService(mockRepoReader.Object, mockRepoWriter.Object);

            sut.AddCustomer("CustomerOne", "Adam",new DateTime(1979, 05, 14),"963852741");

            mockRepoWriter.Verify(m => m.Add(It.IsAny<Customer>()), Times.Once);
        }

        [TestCase()]
        public void AddDuplicateDeletedCustomerTest()
        {
            Mock<IRepositoryReader<Customer>> mockRepoReader = new Mock<IRepositoryReader<Customer>>();
            Mock<IRepositoryWriter<Customer>> mockRepoWriter = new Mock<IRepositoryWriter<Customer>>();

            mockRepoReader.Setup(m => m.GetAll()).Returns(
                    new List<Customer> {
                        new Customer { FamilyName = "Customer2", GivenName = "Beth", DateOfBirth=new DateTime(1979, 05, 18), SocialSecurityNumber = "111223333", IsActive = false }
                    });

            ICustomerService sut = new CustomerService(mockRepoReader.Object, mockRepoWriter.Object);

            sut.AddCustomer("Customer2", "Beth", new DateTime(1979, 05, 18), "111223333");

            mockRepoWriter.Verify(m => m.Update(It.IsAny<Customer>()), Times.Once);
        }

        [TestCase("", TestName = "AddCustomerFamilyNameIsEmpty")]
        [TestCase(null, TestName = "AddCustomerFamilyNameIsNull")]
        public void AddCustomerBlankFamilyNameTest(string familyName)
        {
            Mock<IRepositoryReader<Customer>> mockRepoReader = new Mock<IRepositoryReader<Customer>>();
            Mock<IRepositoryWriter<Customer>> mockRepoWriter = new Mock<IRepositoryWriter<Customer>>();

            ICustomerService sut = new CustomerService(mockRepoReader.Object, mockRepoWriter.Object);

            var ex = Assert.Throws<InvalidDataException>(() => sut.AddCustomer(familyName, "given", DateTime.Now,""));
            Assert.That(ex.Message, Is.EqualTo("Customer FamilyName is required."));
        }

        [TestCase("", TestName = "AddCustomerGivenNameIsEmpty")]
        [TestCase(null, TestName = "AddCustomerGivenNameIsNull")]
        public void AddCustomerBlankGivenNameTest(string givenName)
        {
            Mock<IRepositoryReader<Customer>> mockRepoReader = new Mock<IRepositoryReader<Customer>>();
            Mock<IRepositoryWriter<Customer>> mockRepoWriter = new Mock<IRepositoryWriter<Customer>>();

            ICustomerService sut = new CustomerService(mockRepoReader.Object, mockRepoWriter.Object);

            var ex = Assert.Throws<InvalidDataException>(() => sut.AddCustomer("family", givenName, DateTime.Now, ""));
            Assert.That(ex.Message, Is.EqualTo("Customer GivenName is required."));
        }

        [TestCase("", TestName = "AddCustomerSSNIsEmpty")]
        [TestCase(null, TestName = "AddCustomerSSNIsNull")]
        public void AddCustomerBlankSSNTest(string ssn)
        {
            Mock<IRepositoryReader<Customer>> mockRepoReader = new Mock<IRepositoryReader<Customer>>();
            Mock<IRepositoryWriter<Customer>> mockRepoWriter = new Mock<IRepositoryWriter<Customer>>();

            ICustomerService sut = new CustomerService(mockRepoReader.Object, mockRepoWriter.Object);

            var ex = Assert.Throws<InvalidDataException>(() => sut.AddCustomer("family", "given", DateTime.Now, ssn));
            Assert.That(ex.Message, Is.EqualTo("Customer socialSecurityNumber is required."));
        }
    }
}
