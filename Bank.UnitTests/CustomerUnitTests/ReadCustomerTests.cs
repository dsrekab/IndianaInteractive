using System;
using System.Collections.Generic;
using System.Linq;
using Bank.Contract.Data;
using Bank.Contract.Domain;
using Bank.Contract.Services;
using Bank.Services;
using Moq;
using NUnit.Framework;

namespace Bank.UnitTests.CustomerUnitTests
{
    [TestFixture]
    public class ReadCustomerTests
    {
        [TestCase("family1", "given1", "family1", "given1", TestName = "Matching FamilyName")]
        [TestCase("family2", "given2", "fAmILy2", "given2", TestName = "Matching FamilyName - Different Case")]
        [TestCase("family3", "given3", "family3", "given3", TestName = "Matching GivenName")]
        [TestCase("family4", "given4", "family4", "gIVeN4", TestName = "Matching GivenName - Different Case")]
        public void GetCustomerByNameTests(string familyName, string givenName, string testFamilyName, string testGivenName)
        {
            Mock<IRepositoryReader<Customer>> mockRepoReader = new Mock<IRepositoryReader<Customer>>();
            Mock<IRepositoryWriter<Customer>> mockRepoWriter = new Mock<IRepositoryWriter<Customer>>();

            mockRepoReader.Setup(m => m.GetAll()).Returns(new List<Customer> { new Customer { FamilyName = familyName, GivenName = givenName } });

            ICustomerService sut = new CustomerService(mockRepoReader.Object, mockRepoWriter.Object);

            Assert.IsNotNull(sut.GetCustomersByName(testFamilyName, testGivenName));
        }

        [TestCase()]
        public void GetAllCustomersTests()
        {
            Mock<IRepositoryReader<Customer>> mockRepoReader = new Mock<IRepositoryReader<Customer>>();
            Mock<IRepositoryWriter<Customer>> mockRepoWriter = new Mock<IRepositoryWriter<Customer>>();

            mockRepoReader.Setup(m => m.GetAll()).Returns(
                new List<Customer> {
                    new Customer { FamilyName = "Family5", GivenName = "Given5" },
                    new Customer { FamilyName = "Family6", GivenName = "Given6" }
                });

            ICustomerService sut = new CustomerService(mockRepoReader.Object, mockRepoWriter.Object);

            Assert.AreEqual(2, sut.GetAllCustomers().Count());
        }

        [TestCase()]
        public void GetCustomerBySsnTests()
        {
            Mock<IRepositoryReader<Customer>> mockRepoReader = new Mock<IRepositoryReader<Customer>>();
            Mock<IRepositoryWriter<Customer>> mockRepoWriter = new Mock<IRepositoryWriter<Customer>>();

            mockRepoReader.Setup(m => m.GetAll()).Returns(new List<Customer> { new Customer { FamilyName = "Family7", GivenName = "Given7", SocialSecurityNumber="111227777" } });

            ICustomerService sut = new CustomerService(mockRepoReader.Object, mockRepoWriter.Object);

            Assert.IsNotNull(sut.GetCustomerBySsn("111227777"));
        }
    }
}
