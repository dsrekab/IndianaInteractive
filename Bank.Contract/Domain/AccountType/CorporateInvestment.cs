
namespace Bank.Contract.Domain.AccountType
{
    public class CorporateInvestment : InvestmentAccount, IAccountType
    {
        public string GetDisplayName()
        {
            return "Corporate Investment";
        }

        /// <summary>
        /// This is an odd way to implement.  Only doing this so that I can show OO design
        /// </summary>
        /// <returns></returns>
        public decimal? GetMaxWithdrawalAmount()
        {
            return null;
        }
    }
}
