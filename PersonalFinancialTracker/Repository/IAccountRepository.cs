using PersonalFinancialTracker.Model;

namespace PersonalFinancialTracker.Repository
{
    public interface IAccountRepository
    {
        Task<string> AddAccountAsync(AccountModel accountModel);
    }
}
