using Bank.Contract.Data;
using Bank.Contract.Services;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Bank.Services
{
    public class BankService : IBankService
    {
        /// <summary>
        /// Note: some of the checks made in these method, e.g. name cannot be "", I would normally do in the UI layer.
        /// Because this assessment is using unit tests instead of a UI layer, I'm doing them here
        /// </summary>
        /// 

        private readonly IRepositoryReader<Contract.Domain.Bank> _bankRepoReader;
        private readonly IRepositoryWriter<Contract.Domain.Bank> _bankRepoWriter;

        public BankService(IRepositoryReader<Contract.Domain.Bank> bankRepoReader, IRepositoryWriter<Contract.Domain.Bank> bankRepoWriter)
        {
            _bankRepoReader = bankRepoReader;
            _bankRepoWriter = bankRepoWriter;
        }

        public void AddBank(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new InvalidDataException("All banks must have a unique Name.");
            }

            List<Contract.Domain.Bank> bankList = _bankRepoReader.GetAll().ToList();

            if (bankList.FirstOrDefault(m=>m.Name.ToUpper()==name.ToUpper()) != null)
            {
                throw new InvalidDataException($"There is already banked name [{name}].  All Bank names must be unique.");
            }

            Contract.Domain.Bank newBank = new Contract.Domain.Bank { Name = name, IsActive = true };

            _bankRepoWriter.Add(newBank);
        }

        public void DeleteBank(int bankId)
        {
            Contract.Domain.Bank bank = _bankRepoReader.GetById(bankId);

            if (bank != null)
            {
                _bankRepoWriter.Delete(bank);
            }
        }

        public void DeleteBank(Contract.Domain.Bank bank)
        {
            _bankRepoWriter.Delete(bank);
        }

        public Contract.Domain.Bank GetBankByName(string name)
        {
            var bankList = GetAllBanks();

            return bankList.FirstOrDefault(m => m.Name.ToUpper() == name.ToUpper());
        }

        public IEnumerable<Contract.Domain.Bank> GetAllBanks()
        {
            return _bankRepoReader.GetAll();
        }

        public void UpdateBank(int bankId, string name, bool isActive)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new InvalidDataException("All banks must have a unique Name.");
            }

            List<Contract.Domain.Bank> bankList = _bankRepoReader.GetAll().ToList();

            if (bankList.FirstOrDefault(m => m.Name.ToUpper() == name.ToUpper() && m.Id!=bankId) != null)
            {
                throw new InvalidDataException($"There is already banked name [{name}].  All Bank names must be unique.");
            }

            Contract.Domain.Bank bank = _bankRepoReader.GetById(bankId);

            bank.Name = name;
            bank.IsActive = isActive;

            _bankRepoWriter.Update(bank);
        }
    }
}
