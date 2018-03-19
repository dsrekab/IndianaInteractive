using Bank.Contract.Domain;
using System.Collections.Generic;

namespace Bank.Contract.Data
{
    public interface IRepositoryReader<T> where T:IEntity
    {
        IEnumerable<T> GetAll();
        T GetById(int id);
    }
}
