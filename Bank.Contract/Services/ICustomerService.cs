using Bank.Contract.Domain;
using System;
using System.Collections.Generic;

namespace Bank.Contract.Services
{
    public interface ICustomerService
    {
        void AddCustomer(string familyName, string givenName, DateTime dateOfBirth, string socialSecurityNumber);
        void DeleteCustomer(Customer customer);
        void DeleteCustomer(int customerId);
        void UpdateCustomer(int customerId, string familyName, string givenName, DateTime dateOfBirth, string socialSecurityNumber, bool IsActive);
        IEnumerable<Customer> GetAllCustomers();
        IEnumerable<Customer> GetCustomersByName(string familyName, string givenName);
        Customer GetCustomerBySsn(string socialSecurityNumber);
    }
}
