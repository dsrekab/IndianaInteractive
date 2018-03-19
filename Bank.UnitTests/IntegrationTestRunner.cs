using Bank.IntegrationTests;
using NUnit.Framework;

namespace Bank.UnitTests
{
    [TestFixture]
    public class IntegrationTestRunner
    {
        [TestCase]
        public void IntegrationTestsBankCrud()
        {
            IIntegrationTest intTest = new BankCrudIntegrationTests();
            intTest.Run();
        }

        [TestCase]
        public void IntegrationTestsCustomerCrud()
        {
            IIntegrationTest intTest = new CustomerCrudIntegrationTests();
            intTest.Run();
        }

        [TestCase]
        public void IntegrationTestsAccountCrud()
        {
            IIntegrationTest intTest = new AccountCrudIntegrationTests();
            intTest.Run();
        }

        [TestCase]
        public void IntegrationTestsTransactionCrud()
        {
            IIntegrationTest intTest = new TransactionCrudIntegrationTests();
            intTest.Run();
        }

        [TestCase]
        public void IntegrationTestsMonthSimulation()
        {
            IIntegrationTest intTest = new MonthSimulationIntegrationTest();
            intTest.Run();
        }
    }
}
