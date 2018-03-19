
namespace Bank.Contract.Domain.AccountType
{
    public interface IAccountType
    {
        decimal? GetMaxWithdrawalAmount();
        string GetDisplayName();
    }
}
