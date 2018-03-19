using Bank.Contract.Data;
using Bank.Contract.Domain;
using System.Collections.Generic;
using System.Linq;

namespace Bank.Data
{
    public class AccountMemoryRepository : IRepositoryReader<Account>, IRepositoryWriter<Account>
    {
        private List<Account> _accountList;

        public AccountMemoryRepository()
        {
            _accountList = new List<Account>();
        }

        public void Add(Account entity)
        {
            //using a database this would be an auto increment
            int maxId = _accountList.Count() == 0 ? 1 : _accountList.Max(m => m.Id)+1;
            entity.Id = maxId;

            _accountList.Add(entity);
        }

        /// <summary>
        /// Don't want to lose history, so instead of actually deleting, just update the isActive indicator
        /// </summary>
        /// <param name="entity"></param>
        public void Delete(Account entity)
        {
            Account acctToDelete = GetById(entity.Id); 

            acctToDelete.IsActive = false;

            Update(acctToDelete);
        }

        public IEnumerable<Account> GetAll()
        {
            return _accountList;
        }

        public Account GetById(int id)
        {
            return _accountList.FirstOrDefault(m => m.Id == id);
        }

        public void Update(Account entity)
        {
            Account acctToUpdate = GetById(entity.Id);

            acctToUpdate.AccountNumber = entity.AccountNumber;
            acctToUpdate.AccountType = entity.AccountType;
            acctToUpdate.IsActive = entity.IsActive;
            acctToUpdate.OwningCustomerId = entity.OwningCustomerId;
            acctToUpdate.BankId = entity.BankId;
        }
    }
}
