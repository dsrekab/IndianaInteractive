using Bank.Contract.Services;
using Bank.Data;
using Bank.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.IntegrationTests
{
    public class CustomerCrudIntegrationTests : IIntegrationTest
    {
        public void Run()
        {
            CustomerMemoryRepository customerRepository = new CustomerMemoryRepository();
            
            //Create a new Customer
            ICustomerService testCustomer = new CustomerService(customerRepository, customerRepository);

            try
            {
                testCustomer.AddCustomer("Integration","TestAdd1",new DateTime(2000,06,18),"123456789");
            }
            catch (Exception ex)
            {
                throw new Exception($"Exception throw while creating new Customer. Exception: {ex.ToString()}");
            }


            //Ensure Customer was added
            try
            {
                //get all Customers to ensure only 1 was added
                var customerList = testCustomer.GetAllCustomers();
                if (customerList.Count() != 1) { throw new Exception($"Expected to find 1 customer, instead found {customerList.Count()} customers"); }

                //get the specific customer to ensure get by name is working
                var addedCustomer = testCustomer.GetCustomersByName("Integration", "TestAdd1");
                if (addedCustomer.Count()!=1) { throw new Exception($"Find Customer By Name returned {addedCustomer.Count()} but expected 1 customer."); }

                //get the specific customer to ensure get by name with different casing is working
                var addedCustomerDifferentCase = testCustomer.GetCustomersByName("InTEgrAtIon", "TeStaDD1");
                if (addedCustomerDifferentCase.Count() != 1) { throw new Exception($"Find Customer By Name with different case (InTEgrAtIon, TeStaDD1) returned {addedCustomerDifferentCase.Count()} but expected 1 customer."); }


                //get the specific customer by SSN
                var addedCustomerBySsn = testCustomer.GetCustomerBySsn("123456789");
                if (addedCustomerBySsn is null) { throw new Exception($"Unable to find Customer by SSN (123456789)"); }
            }
            catch (Exception ex)
            {
                throw new Exception($"Exception throw while reading from Customer repository after CREATE.  Exception: {ex.ToString()}");
            }

            //Update Customer Name
            try
            {
                var addedCustomer = testCustomer.GetCustomerBySsn("123456789");

                addedCustomer.FamilyName = "IntegrationUpdated";
                addedCustomer.GivenName = "TestUpdate1";

                customerRepository.Update(addedCustomer);
            }
            catch (Exception ex)
            {
                throw new Exception($"Exception throw while Updating Customer Name.  Exception: {ex.ToString()}");
            }

            //Ensure Customer was updated
            try
            {
                //get all Customers to ensure there is still only one
                var customerList = testCustomer.GetAllCustomers();
                if (customerList.Count() != 1) { throw new Exception($"Expected to find 1 customer, instead found {customerList.Count()} customers"); }

                //get the specific customer to ensure get by name is working
                var addedCustomer = testCustomer.GetCustomersByName("IntegrationUpdated", "TestUpdate1");
                if (addedCustomer.Count() != 1) { throw new Exception($"Find Customer By Name returned {addedCustomer.Count()} but expected 1 customer."); }

                //get the specific customer to ensure get by name with different casing is working
                var addedCustomerDifferentCase = testCustomer.GetCustomersByName("InTEgrAtIonUpdATed", "TeStupDAte1");
                if (addedCustomerDifferentCase.Count() != 1) { throw new Exception($"Find Customer By Name with different case (InTEgrAtIonUpdATed, TeStupDAte1) returned {addedCustomer.Count()} but expected 1 customer."); }

            }
            catch (Exception ex)
            {
                throw new Exception($"Exception throw while reading from Customer repository after UPDATE.  Exception: {ex.ToString()}");
            }

            //Delete Customer
            try
            {
                var addedCustomer = testCustomer.GetCustomerBySsn("123456789");

                customerRepository.Delete(addedCustomer);
            }
            catch (Exception ex)
            {
                throw new Exception($"Exception throw while Deleting Customer.  Exception: {ex.ToString()}");
            }

            //Ensure Customer was Deleted
            try
            {
                //get all Customer to ensure there is still only one
                var customerList = testCustomer.GetAllCustomers();
                if (customerList.Count() != 1) { throw new Exception($"Expected to find 1 customer, instead found {customerList.Count()} customers"); }

                //get the specific Customer to ensure get by name is working
                var addedCustomer = testCustomer.GetCustomerBySsn("123456789");
                if (addedCustomer is null)
                {
                    throw new Exception("Unable to find Customer by Ssn (123456789)");
                }
                else
                {
                    if (addedCustomer.IsActive)
                    {
                        throw new Exception("Found Customer by Ssn (123456789) after DELETE, however customer was still marked as active.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Exception throw while reading from Customer repository after DELETE.  Exception: {ex.ToString()}");
            }
        }
    }
}
