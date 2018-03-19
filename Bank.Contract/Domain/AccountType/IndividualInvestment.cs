
namespace Bank.Contract.Domain.AccountType
{
    public class IndividualInvestment : InvestmentAccount, IAccountType
    {
        public string GetDisplayName()
        {
            return "Individual Investment";
        }

        /// <summary>
        /// This is an odd way to implement.  Only doing this so that I can show OO design
        /// </summary>
        /// <returns></returns>
        public decimal? GetMaxWithdrawalAmount()
        {
            return 1000;
        }
    }
}
