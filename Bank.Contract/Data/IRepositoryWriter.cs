using Bank.Contract.Domain;

namespace Bank.Contract.Data
{
    public interface IRepositoryWriter<T>:IRepositoryAdder<T> where T:IEntity
    {
        void Delete(T entity);
        void Update(T entity);
    }
}
