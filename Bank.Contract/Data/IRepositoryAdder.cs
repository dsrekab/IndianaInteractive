using Bank.Contract.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Contract.Data
{
    public interface IRepositoryAdder<T> where T:IEntity
    {
        void Add(T entity);
    }
}
