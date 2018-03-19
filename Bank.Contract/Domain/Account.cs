using Bank.Contract.Domain.AccountType;

namespace Bank.Contract.Domain
{
    public class Account:IEntity
    {
        public int Id { get; set; }
        public string AccountNumber { get; set; }
        public IAccountType AccountType { get; set; }
        public int BankId { get; set; }
        public int OwningCustomerId { get; set; }
        public bool IsActive { get; set; }
    }
}
