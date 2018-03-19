using Bank.Contract.Data;
using Bank.Contract.Domain;
using System.Collections.Generic;
using System.Linq;

namespace Bank.Data
{
    public class CustomerMemoryRepository : IRepositoryReader<Customer>, IRepositoryWriter<Customer>
    {
        private List<Customer> _customerList;

        public CustomerMemoryRepository()
        {
            _customerList = new List<Customer>();
        }

        public void Add(Customer entity)
        {
            //using a database this would be an auto increment
            int maxId = _customerList.Count() == 0 ? 1 : _customerList.Max(m => m.Id) + 1;
            entity.Id = maxId;

            _customerList.Add(entity);
        }

        /// <summary>
        /// don't want to lose history, so just update the IsActive indicator
        /// </summary>
        /// <param name="entity"></param>
        public void Delete(Customer entity)
        {
            Customer customerToDelete = GetById(entity.Id);

            customerToDelete.IsActive = false;

            Update(customerToDelete);
        }

        public IEnumerable<Customer> GetAll()
        {
            return _customerList;
        }

        public Customer GetById(int id)
        {
            return _customerList.FirstOrDefault(m => m.Id == id);
        }

        public void Update(Customer entity)
        {
            Customer customerToUpdate = GetById(entity.Id);

            customerToUpdate.FamilyName = entity.FamilyName;
            customerToUpdate.GivenName = entity.GivenName;
            customerToUpdate.SocialSecurityNumber = entity.SocialSecurityNumber;
            customerToUpdate.DateOfBirth = entity.DateOfBirth;
            customerToUpdate.IsActive = entity.IsActive;
        }
    }
}
