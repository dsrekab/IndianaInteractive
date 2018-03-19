using Bank.Contract.Data;
using Bank.Contract.Domain;
using System.Collections.Generic;
using System.Linq;

namespace Bank.Data
{
    /// <summary>
    /// An In-Memory repository, not tied to a database
    /// </summary>
    public class TransactionMemoryRepository : IRepositoryReader<Transaction>, IRepositoryAdder<Transaction>
    {
        private List<Transaction> _transactionList;

        public TransactionMemoryRepository()
        {
            _transactionList = new List<Transaction>();
        }

        /// <summary>
        /// Transactions are Add-Only, so implement the adder instead of the writer
        /// </summary>
        /// <param name="entity"></param>
        public void Add(Transaction entity)
        {
            //using a database this would be an auto increment
            int maxId = _transactionList.Count() == 0 ? 1 : _transactionList.Max(m => m.Id) + 1;
            entity.Id = maxId;

            _transactionList.Add(entity);
        }

        public IEnumerable<Transaction> GetAll()
        {
            return _transactionList;
        }

        public Transaction GetById(int id)
        {
            return _transactionList.FirstOrDefault(m => m.Id == id);
        }
    }
}
