using Bank.Contract.Services;
using Bank.Data;
using Bank.Services;
using System;
using System.Linq;

namespace Bank.IntegrationTests
{
    public class BankCrudIntegrationTests : IIntegrationTest
    {
        public void Run()
        {
            BankMemoryRepository bankRepository = new BankMemoryRepository();
            string bankName = "Int Bank Test 1";
            string updatedBankName= "Int Bank Test 1 Updated";

            //Create a new Bank
            IBankService testBank = new BankService(bankRepository, bankRepository);

            try
            {
                testBank.AddBank(bankName);
            }
            catch (Exception ex)
            {
                throw new Exception($"Exception throw while creating new Bank. Exception: {ex.ToString()}");
            }


            //Ensure Bank was added
            try
            {
                //get all banks to ensure only 1 was added
                var bankList = testBank.GetAllBanks();
                if (bankList.Count() != 1) { throw new Exception($"Expected to find 1 bank, instead found {bankList.Count()} banks"); }

                //get the specific bank to ensure get by name is working
                var addedBank = testBank.GetBankByName("Int Bank Test 1");
                if (addedBank is null) { throw new Exception($"Unable to find Bank by Name {bankName}"); }

                //get the specific bank with different casing
                var addedBankDifferentCase = testBank.GetBankByName("INT BAnk teST 1");
                if (addedBankDifferentCase is null) { throw new Exception($"Unable to find Bank by Name with different casing {bankName} - searched for: INT BAnk teST 1"); }
            }
            catch (Exception ex)
            {
                throw new Exception($"Exception throw while reading from Bank repository after CREATE.  Exception: {ex.ToString()}");
            }

            //Update Bank Name
            try
            {
                var addedBank = testBank.GetBankByName(bankName);

                addedBank.Name = updatedBankName;

                bankRepository.Update(addedBank);
            }
            catch (Exception ex)
            {
                throw new Exception($"Exception throw while Updating Bank Name.  Exception: {ex.ToString()}");
            }

            //Ensure Bank was updated
            try
            {
                //get all banks to ensure there is still only one bank
                var bankList = testBank.GetAllBanks();
                if (bankList.Count() != 1) { throw new Exception($"Expected to find 1 bank, instead found {bankList.Count()} banks"); }

                //get the specific bank to ensure get by name is working
                var addedBank = testBank.GetBankByName("Int Bank Test 1 Updated");
                if (addedBank is null) { throw new Exception($"Unable to find Bank by Name {updatedBankName}"); }

                //get the specific bank with different casing
                var addedBankDifferentCase = testBank.GetBankByName("iNT BAnk teST 1 updaTEd");
                if (addedBankDifferentCase is null) { throw new Exception($"Unable to find Bank by Name with different casing {updatedBankName} - searched for: iNT BAnk teST 1 updaTEd"); }
            }
            catch (Exception ex)
            {
                throw new Exception($"Exception throw while reading from Bank repository after UPDATE.  Exception: {ex.ToString()}");
            }

            //Delete Bank
            try
            {
                var addedBank = testBank.GetBankByName(updatedBankName);

                bankRepository.Delete(addedBank);
            }
            catch (Exception ex)
            {
                throw new Exception($"Exception throw while Deleting Bank.  Exception: {ex.ToString()}");
            }

            //Ensure Bank was Deleted
            try
            {
                //get all banks to ensure there is still only one bank
                var bankList = testBank.GetAllBanks();
                if (bankList.Count() != 1) { throw new Exception($"Expected to find 1 bank, instead found {bankList.Count()} banks"); }

                //get the specific bank to ensure get by name is working
                var addedBank = testBank.GetBankByName(updatedBankName);
                if (addedBank is null)
                {
                    throw new Exception($"Unable to find Bank by Name {updatedBankName}"); 
                }
                else
                {
                    if (addedBank.IsActive)
                    {
                        throw new Exception($"Found {updatedBankName} after delete, however it was still marked as active.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Exception throw while reading from Bank repository after DELETE.  Exception: {ex.ToString()}");
            }
        }
    }
}
