using Bank.Contract.Data;
using System.Collections.Generic;
using System.Linq;

namespace Bank.Data
{
    public class BankMemoryRepository : IRepositoryReader<Contract.Domain.Bank>, IRepositoryWriter<Contract.Domain.Bank>
    {
        private List<Contract.Domain.Bank> _bankList;

        public BankMemoryRepository()
        {
            _bankList = new List<Contract.Domain.Bank>();
        }

        public void Add(Contract.Domain.Bank entity)
        {
            //using a database this would be an auto increment
            int maxId = _bankList.Count()==0?1:_bankList.Max(m => m.Id) + 1;

            entity.Id = maxId + 1; 

            _bankList.Add(entity);
        }

        /// <summary>
        /// don't want to lose history, so just update the isActive indicator instead of actually deleting
        /// </summary>
        /// <param name="entity"></param>
        public void Delete(Contract.Domain.Bank entity)
        {
            Contract.Domain.Bank bankToDelete = GetById(entity.Id);

            if (bankToDelete != null)
            {
                bankToDelete.IsActive = false;

                Update(bankToDelete);
            }
        }

        public IEnumerable<Contract.Domain.Bank> GetAll()
        {
            return _bankList;
        }

        public Contract.Domain.Bank GetById(int id)
        {
            return _bankList.FirstOrDefault(m => m.Id == id);
        }

        public void Update(Contract.Domain.Bank entity)
        {
            Contract.Domain.Bank bankToUpdate = GetById(entity.Id);

            if (bankToUpdate != null)
            {
                bankToUpdate.Name = entity.Name;
                bankToUpdate.IsActive = entity.IsActive;
            }
        }
    }
}
