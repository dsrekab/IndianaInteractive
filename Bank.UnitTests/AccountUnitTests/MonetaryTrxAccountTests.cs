using System;
using System.Collections.Generic;
using Bank.Contract.Data;
using Bank.Contract.Domain;
using Bank.Contract.Domain.AccountType;
using Bank.Contract.Services;
using Bank.Services;
using Moq;
using NUnit.Framework;

namespace Bank.UnitTests.AccountUnitTests
{
    [TestFixture]
    public class MonetaryTrxAccountTests
    {
        [TestCase(1, true, -13, TestName ="InvalidDepositAmount")]
        [TestCase(5, true, 16, TestName = "InvalidDepositAccountId")]
        [TestCase(1, false, 18, TestName = "InActiveDepositAccountId")]
        [TestCase(1, true, 20, TestName = "ValidDepositTest")]
        public void DepostTests(int acctId, bool acctActive, decimal amount)
        {
            Mock<IRepositoryReader<Account>> mockRepoReader = new Mock<IRepositoryReader<Account>>();
            Mock<IRepositoryWriter<Account>> mockRepoWriter = new Mock<IRepositoryWriter<Account>>();
            Mock<ITransactionService> mockTrxService = new Mock<ITransactionService>();

            mockRepoReader.Setup(m => m.GetAll()).Returns(new List<Account> { new Account { Id = acctId, IsActive=acctActive } });

            IAccountService sut = new AccountService(mockRepoReader.Object, mockRepoWriter.Object, mockTrxService.Object);

            if (amount < 0)
            {
                var ex = Assert.Throws<Exception>(() => sut.DepositToAccount(1, amount));
                Assert.That(ex.Message, Is.EqualTo("Unable to deposit a negative dollar amount"));
            }
            else
            if (acctId == 5 || acctActive == false)
            {
                var ex = Assert.Throws<Exception>(() => sut.DepositToAccount(1, amount));
                Assert.That(ex.Message, Is.EqualTo($"No active accounts found for AccountId 1"));
            }
            else
            {
                sut.DepositToAccount(1, amount);
                mockTrxService.Verify(m => m.AddTransaction(1, amount, null), Times.Once);

                DateTime trxDateTime = new DateTime(2018, 03, 04);

                sut.DepositToAccount(1, amount+5, trxDateTime);
                mockTrxService.Verify(m => m.AddTransaction(1, amount+5, trxDateTime, null), Times.Once);
            }
        }


        [TestCase(1, 2, true, true, -20, TestName = "NegativeTransferAmount")]
        [TestCase(7, 2, true, true, 22, TestName = "FromAccountDoesNotExist")]
        [TestCase(1, 2, false, true, 23, TestName = "FromAccountNotActive")]
        [TestCase(1, 8, true, true, 24, TestName = "ToAccountDoesNotExist")]
        [TestCase(1, 2, true, false, 25, TestName = "ToAccountNotActive")]
        [TestCase(1, 2, true, true, 26, TestName = "ValidTransfer")]
        public void TransferTests(int fromAcctId, int toAcctId, bool fromAcctActive, bool toAcctActive, decimal amount)
        {
            Mock<IRepositoryReader<Account>> mockRepoReader = new Mock<IRepositoryReader<Account>>();
            Mock<IRepositoryWriter<Account>> mockRepoWriter = new Mock<IRepositoryWriter<Account>>();
            Mock<ITransactionService> mockTrxService = new Mock<ITransactionService>();

            mockRepoReader.Setup(m => m.GetAll()).Returns(
                new List<Account> {
                    new Account { Id = 1, IsActive = fromAcctActive },
                    new Account { Id = 2, IsActive = toAcctActive }
                });

            IAccountService sut = new AccountService(mockRepoReader.Object, mockRepoWriter.Object, mockTrxService.Object);

            if (amount < 0)
            {
                var ex = Assert.Throws<Exception>(() => sut.TransferBetweenAccount(1, 2, amount));
                Assert.That(ex.Message, Is.EqualTo("Unable to transfer a negative dollar amount"));

                var exDate = Assert.Throws<Exception>(() => sut.TransferBetweenAccount(1, 2, amount, new DateTime(2018,03,08)));
                Assert.That(exDate.Message, Is.EqualTo("Unable to transfer a negative dollar amount"));
            }
            else if (fromAcctId==7 || fromAcctActive==false)
            {
                var ex = Assert.Throws<Exception>(() => sut.TransferBetweenAccount(fromAcctId, 2, amount));
                Assert.That(ex.Message, Is.EqualTo($"No active accounts found for FromAccount AccountId {fromAcctId}"));

                var exDate = Assert.Throws<Exception>(() => sut.TransferBetweenAccount(fromAcctId, 2, amount, new DateTime(2018, 03, 08)));
                Assert.That(exDate.Message, Is.EqualTo($"No active accounts found for FromAccount AccountId {fromAcctId}"));
            }
            else if (toAcctId == 8 || toAcctActive == false)
            {
                var ex = Assert.Throws<Exception>(() => sut.TransferBetweenAccount(1, toAcctId, amount));
                Assert.That(ex.Message, Is.EqualTo($"No active accounts found for ToAccount AccountId {toAcctId}"));

                var exDate = Assert.Throws<Exception>(() => sut.TransferBetweenAccount(1, toAcctId, amount, new DateTime(2018, 03, 08)));
                Assert.That(exDate.Message, Is.EqualTo($"No active accounts found for ToAccount AccountId {toAcctId}"));
            }
            else
            {
                sut.TransferBetweenAccount(1, 2, amount);
                mockTrxService.Verify(m => m.AddTransaction(1, amount*-1, null), Times.Once);
                mockTrxService.Verify(m => m.AddTransaction(2, amount, null), Times.Once);

                DateTime trxDateTime = new DateTime(2018, 03, 05);

                sut.TransferBetweenAccount(1, 2, amount + 5, trxDateTime);
                mockTrxService.Verify(m => m.AddTransaction(1, -1*(amount + 5), trxDateTime, null), Times.Once);
                mockTrxService.Verify(m => m.AddTransaction(2, amount + 5, trxDateTime, null), Times.Once);
            }
        }


        [TestCase(1, 2, true, 30, "Checking", TestName ="WithdrawalAccountDoesNotExist")]
        [TestCase(1, 1, false, 31, "Checking", TestName = "WithdrawalAccountInactive")]
        [TestCase(1, 1, true, 1001, "IndInv", TestName = "WithdrawalAmountExceedsMaxAllowed")]
        [TestCase(1, 1, true, 1002, "Checking", TestName = "CheckingWithdrawalAmountExceedsBalance")]
        [TestCase(1, 1, true, 1003, "CorpInv", TestName = "CorpInvWithdrawalAmountExceedsBalance")]
        [TestCase(1, 1, true, 104, "IndInv", TestName = "ValidIndividualWithdrawal")]
        [TestCase(1, 1, true, 105, "Checking", TestName = "ValidCheckingWithdrawal")]
        [TestCase(1, 1, true, 106, "CorpInv", TestName = "ValidCorporateWithdrawal")]
        public void WithdrawalTest(int acctId, int withdrawalAcctId, bool acctIsActive, decimal amount, string acctTypeString)
        {
            Mock<IRepositoryReader<Account>> mockRepoReader = new Mock<IRepositoryReader<Account>>();
            Mock<IRepositoryWriter<Account>> mockRepoWriter = new Mock<IRepositoryWriter<Account>>();
            Mock<ITransactionService> mockTrxService = new Mock<ITransactionService>();

            IAccountType accountType = null;
            switch(acctTypeString)
            {
                case "Checking":
                    accountType = new Checking();
                    break;
                case "CorpInv":
                    accountType = new CorporateInvestment();
                    break;
                case "IndInv":
                    accountType = new IndividualInvestment();
                    break;
            }

            if (amount > 500)
            {
                mockTrxService.Setup(m => m.GetAllTransactionsByAccountId(acctId)).Returns(
                    new List<Transaction>
                    {
                        new Transaction{AccountId=acctId, Amount=8 },
                        new Transaction{AccountId=acctId, Amount=18 }
                    });
            }
            else
            {
                mockTrxService.Setup(m => m.GetAllTransactionsByAccountId(acctId)).Returns(
                    new List<Transaction>
                    {
                        new Transaction{AccountId=acctId, Amount=amount }
                    });
            }

            mockRepoReader.Setup(m => m.GetAll()).Returns(
                new List<Account> {
                    new Account { Id = acctId, IsActive = acctIsActive, AccountType=accountType }
                });

            IAccountService sut = new AccountService(mockRepoReader.Object, mockRepoWriter.Object, mockTrxService.Object);

            if (acctId!=withdrawalAcctId || acctIsActive==false)
            {
                var ex = Assert.Throws<Exception>(() => sut.WithdrawalFromAccount(withdrawalAcctId, amount));
                Assert.That(ex.Message, Is.EqualTo($"No active accounts found for AccountId {withdrawalAcctId}"));
            }
            else if (acctTypeString=="IndInv" && amount>1000)
            {
                var ex = Assert.Throws<Exception>(() => sut.WithdrawalFromAccount(withdrawalAcctId, amount));
                Assert.That(ex.Message, Is.EqualTo($"Account Types of {accountType.GetDisplayName()} are limited to a dollar amount of {accountType.GetMaxWithdrawalAmount()} in a single transaction"));
            }
            else if (amount>500)
            {
                var ex = Assert.Throws<Exception>(() => sut.WithdrawalFromAccount(withdrawalAcctId, amount));
                Assert.That(ex.Message, Is.EqualTo("Withdrawal amount exceeds current account balance.  Overdrafting not allowed."));
            }
            else
            {
                sut.WithdrawalFromAccount(acctId, amount);
                mockTrxService.Verify(m => m.AddTransaction(acctId, -1*amount, null), Times.Once);

                DateTime trxDateTime = new DateTime(2018, 03, 06);

                sut.WithdrawalFromAccount(1, amount - 5, trxDateTime);
                mockTrxService.Verify(m => m.AddTransaction(acctId, -1*(amount - 5), trxDateTime, null), Times.Once);
            }
        }
    }
}
