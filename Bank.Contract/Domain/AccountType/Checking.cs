
namespace Bank.Contract.Domain.AccountType
{
    public class Checking : IAccountType
    {
        public string GetDisplayName()
        {
            return "Checking";
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
