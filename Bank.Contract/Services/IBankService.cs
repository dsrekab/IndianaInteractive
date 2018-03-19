
using System.Collections.Generic;

namespace Bank.Contract.Services
{
    public interface IBankService
    {
        void AddBank(string name);
        void DeleteBank(int bankId);
        void DeleteBank(Domain.Bank bank);
        void UpdateBank(int bankId, string name, bool isActive);
        IEnumerable<Domain.Bank> GetAllBanks();
        Domain.Bank GetBankByName(string name);
    }
}
