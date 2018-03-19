
namespace Bank.Contract.Domain
{
    public class Bank:IEntity
    {
        public int Id { get; set; }
        public string Name { get; set;}
        public bool IsActive { get; set; }
    }
}
