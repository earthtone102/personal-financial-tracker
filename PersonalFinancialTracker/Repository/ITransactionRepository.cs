using PersonalFinancialTracker.Helper;
using PersonalFinancialTracker.Model;

namespace PersonalFinancialTracker.Repository
{
    public interface ITransactionRepository
    {
        Task<string> AddTransactionAsync(TransactionModel transactionModel);
        Task<bool> DeleteTransactionAsync(Guid id);
        Task<bool> UpdateTransactionAsync(Guid id, TransactionModel newData);
        Task<List<TransactionModel>> GetAllTransactionAsync(FilterModel filter);
        Task<long> GetTotalIncomeAsync(FilterModel filter);
        Task<long> GetTotalExpenseAsync(FilterModel filter);
        Task<long> GetTotalBalanceAsync(FilterModel filter);
        Task<List<TotalSpentPerCategoryModel>> GetTotalSpentPerCategory(string transactionType, FilterModel filterModel);

    }
}
