using Bank.Contract.Data;
using Bank.Contract.Domain;
using Bank.Contract.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Bank.Services
{
    public class CustomerService : ICustomerService
    {

        private readonly IRepositoryReader<Customer> _customerRepoReader;
        private readonly IRepositoryWriter<Customer> _customerRepoWriter;

        public CustomerService(IRepositoryReader<Customer> customerRepoReader, IRepositoryWriter<Customer> customerRepoWriter)
        {
            _customerRepoReader = customerRepoReader;
            _customerRepoWriter = customerRepoWriter;
        }

        public void AddCustomer(string familyName, string givenName, DateTime dateOfBirth, string socialSecurityNumber)
        {
            if (string.IsNullOrWhiteSpace(familyName))
            {
                throw new InvalidDataException("Customer FamilyName is required.");
            }

            if (string.IsNullOrWhiteSpace(givenName))
            {
                throw new InvalidDataException("Customer GivenName is required.");
            }

            if (string.IsNullOrWhiteSpace(socialSecurityNumber))
            {
                throw new InvalidDataException("Customer socialSecurityNumber is required.");
            }
            

            List<Customer> customerList = _customerRepoReader.GetAll().ToList();
            Customer existingCustomer = customerList.FirstOrDefault(m => m.FamilyName.ToUpper() == familyName.ToUpper()
                                              && m.GivenName.ToUpper() == givenName.ToUpper()
                                              && m.SocialSecurityNumber == socialSecurityNumber
                                              && m.DateOfBirth == dateOfBirth
                                              );

            if (existingCustomer != null)
            {
                //re-activate an inactive customer, otherwise nothing to do.  If there were a UI, I would return some kind of indicator, but since this is unitTest driven, just no-op
                if (existingCustomer.IsActive==false)
                {
                    UpdateCustomer(existingCustomer.Id, existingCustomer.FamilyName, existingCustomer.GivenName, existingCustomer.DateOfBirth, existingCustomer.SocialSecurityNumber, true);
                }
            }
            else
            {
                Customer newCustomer = new Customer { FamilyName = familyName, GivenName = givenName, SocialSecurityNumber = socialSecurityNumber, DateOfBirth = dateOfBirth, IsActive=true };
                _customerRepoWriter.Add(newCustomer);
            }            
        }

        public void DeleteCustomer(Customer customer)
        {
            _customerRepoWriter.Delete(customer);
        }

        public void DeleteCustomer(int customerId)
        {
            Customer customer = _customerRepoReader.GetById(customerId);

            if (customer != null)
            {
                DeleteCustomer(customer);
            }
        }

        public IEnumerable<Customer> GetAllCustomers()
        {
            return _customerRepoReader.GetAll();
        }

        public IEnumerable<Customer> GetCustomersByName(string familyName, string givenName)
        {
            var allCustomers = GetAllCustomers();
            return allCustomers.Where(m => m.FamilyName.ToUpper() == familyName.ToUpper() && m.GivenName.ToUpper() == givenName.ToUpper());
        }

        public Customer GetCustomerBySsn(string socialSecurityNumber)
        {
            var allCustomers = GetAllCustomers();
            return allCustomers.FirstOrDefault(m => m.SocialSecurityNumber==socialSecurityNumber);
        }

        public void UpdateCustomer(int customerId, string familyName, string givenName, DateTime dateOfBirth, string socialSecurityNumber, bool isActive)
        {
            if (string.IsNullOrWhiteSpace(familyName))
            {
                throw new InvalidDataException("Customer FamilyName is required.");
            }

            if (string.IsNullOrWhiteSpace(givenName))
            {
                throw new InvalidDataException("Customer GivenName is required.");
            }

            if (string.IsNullOrWhiteSpace(socialSecurityNumber))
            {
                throw new InvalidDataException("Customer socialSecurityNumber is required.");
            }


            List<Customer> customerList = _customerRepoReader.GetAll().ToList();

            //Can't update a customer to match another customer
            if (customerList.FirstOrDefault(m => m.FamilyName.ToUpper() == familyName.ToUpper()
                                              && m.GivenName.ToUpper() == givenName.ToUpper()
                                              && m.SocialSecurityNumber == socialSecurityNumber
                                              && m.DateOfBirth == dateOfBirth
                                              && m.Id!=customerId)!=null)
            {
                throw new InvalidDataException("You cannot update a customer to match the demographics of another customer.");
            }

            var allCustomers = GetAllCustomers();
            Customer customerToUpdate = allCustomers.FirstOrDefault(m => m.Id==customerId);

            customerToUpdate.FamilyName = familyName;
            customerToUpdate.GivenName = givenName;
            customerToUpdate.SocialSecurityNumber = socialSecurityNumber;
            customerToUpdate.DateOfBirth = dateOfBirth;
            customerToUpdate.IsActive = isActive;

            _customerRepoWriter.Update(customerToUpdate);
        }
    }
}
