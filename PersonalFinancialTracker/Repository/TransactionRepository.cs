using PersonalFinancialTracker.Model;
using PersonalFinancialTracker.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using PersonalFinancialTracker.Helper;
using Serilog;
using System.Transactions;

namespace PersonalFinancialTracker.Repository
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly TransactionContext _context;
        private readonly IMapper _mapper;
        private readonly OperationHelper _helper;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ILogger<TransactionRepository> _logger;
        public TransactionRepository(TransactionContext context, IMapper mapper, OperationHelper helper, ICategoryRepository categoryRepository, IAccountRepository accountRepository, ILogger<TransactionRepository> logger)
        {
            _mapper = mapper;
            _context = context;
            _helper = helper;
            _categoryRepository = categoryRepository;
            _accountRepository = accountRepository;
            _logger = logger;
        }

        public async Task<string> AddTransactionAsync(TransactionModel transactionModel)
        {
            #region check if category exist
            var addCategory = await _categoryRepository.AddCategoryAsync(transactionModel.Category);
            #endregion

            #region check if account exist
            var addAccount = await _accountRepository.AddAccountAsync(transactionModel.Account);
            #endregion

            var id = "";
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                #region map and add to db
                var transaction = _mapper.Map<TransactionData>(transactionModel);
                transaction.Id = new Guid();
                transaction.Category = await _context.Categories.FindAsync(Guid.Parse(addCategory));
                transaction.Account = await _context.Accounts.FindAsync(Guid.Parse(addAccount));
                transaction.CreatedTime = DateTime.Now;

                _context.Transactions.Add(transaction);
                await _context.SaveChangesAsync();
                #endregion

                scope.Complete();
                id = transaction.Id.ToString();
                _logger.LogInformation($"{_helper.RequestResultMessage("Add", "Transaction", OperationEnum.RequestResult.Success)}. Id: {id}");
            }
            catch (Exception ex)
            {
                id = "";
                _logger.LogError($"{_helper.RequestResultMessage("Add", "Transaction", OperationEnum.RequestResult.Failed)}. StackTrace: {ex.InnerException!.ToString()}");
            }
            return id;
        }
        public async Task<bool> DeleteTransactionAsync(Guid id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            var successBool = false;
            if (transaction != null)
            {
                await using var scope = await _context.Database.BeginTransactionAsync();
                try
                {
                    _context.Transactions.Remove(transaction);
                    await _context.SaveChangesAsync();
                    await scope.CommitAsync();
                    successBool = true;
                    _logger.LogInformation($"{_helper.RequestResultMessage("Delete", "Transaction", OperationEnum.RequestResult.Success)}. Deleted Id: {id}");
                }
                catch (Exception ex)
                {
                    await scope.RollbackAsync();
                    _logger.LogError($"{_helper.RequestResultMessage("Delete", "Transaction", OperationEnum.RequestResult.Failed)}. StackTrace: {ex.InnerException!.ToString()}");
                }
            }
            else
            {
                _logger.LogError($"{_helper.RequestResultMessage("Delete", "Transaction", OperationEnum.RequestResult.Failed)}. StackTrace: Id Not Found");
            }

            return successBool;
        }
        public async Task<bool> UpdateTransactionAsync(Guid id, TransactionModel newData)
        {
            //immidiately set failed in case the entity isnt present
            var successBool = false;
            var existingTransaction = await _context.Transactions.FindAsync(id);

            if (existingTransaction != null)
            {
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    try
                    {
                        //var updatedTransactionModel = _mapper.Map<TransactionModel>(existingTransaction);

                        #region update the existing with data from the new one
                        //transaction date
                        existingTransaction.TransactionDate = newData.TransactionDate ?? existingTransaction.TransactionDate;
                        //amount
                        existingTransaction.Amount = newData.Amount?? existingTransaction.Amount;
                        //category (if none then add, if existed then retain the id)
                        var categoryAdd = await _categoryRepository.AddCategoryAsync(newData.Category);
                        if (categoryAdd != null && categoryAdd != newData.Category.Id.ToString())
                            existingTransaction.Category = await _context.Categories.FindAsync(Guid.Parse(categoryAdd));
                        //account (if none then add, if existed then retain the id)
                        var accountAdd = await _accountRepository.AddAccountAsync(newData.Account);
                        if (accountAdd != null && accountAdd != newData.Account.Id.ToString())
                            existingTransaction.Account = await _context.Accounts.FindAsync(Guid.Parse(accountAdd));
                        //update time
                        existingTransaction.UpdatedTime = DateTime.Now;                        
                        #endregion

                        _context.Transactions.Update(existingTransaction);
                        await _context.SaveChangesAsync();
                        scope.Complete();

                        //shape the response
                        successBool = true;
                        _logger.LogInformation($"{_helper.RequestResultMessage("Update", "Transaction", OperationEnum.RequestResult.Success)}. Id: {existingTransaction.Id}");

                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"{_helper.RequestResultMessage("Update", "Transaction", OperationEnum.RequestResult.Failed)}. StackTrace: {ex.InnerException!.ToString()}");
                    }
                }
            }
            else
            {
                _logger.LogError($"{_helper.RequestResultMessage("Update", "Transaction", OperationEnum.RequestResult.Failed)}. StackTrace: Id cant be found");
            }
            return successBool;
        }
        public async Task<List<TransactionModel>> GetAllTransactionAsync(FilterModel filterModel)
        {
            var result = new List<TransactionModel>();
            try
            {
                var records = new List<TransactionData>();
                filterModel = _helper.FilterModelToLower(filterModel);

                #region filter - to be change to apply all the filter in query
                //date
                records = await _context.Transactions.Where(x => x.TransactionDate >= filterModel.StartDate).Where(x => x.TransactionDate <= filterModel.EndDate).ToListAsync();
                //transaction type and transaction category
                if (filterModel.Category.TransactionType != null)
                    records = records.Where(x => x.Category.Id == filterModel.Category.Id).ToList();
                //account
                if (filterModel.Account != null)
                    records = records.Where(x => x.Account.Id == filterModel.Account.Id).ToList();
                #endregion

                result = _mapper.Map<List<TransactionModel>>(records);
                _logger.LogInformation($"{_helper.RequestResultMessage("Get All", "Transaction", OperationEnum.RequestResult.Success)}. Total: {result.Count}");
            }
            catch (Exception ex)
            {
                result = new List<TransactionModel>();
                _logger.LogError($"{_helper.RequestResultMessage("Get All", "Transaction", OperationEnum.RequestResult.Failed)}. StackTrace: {ex.InnerException!.ToString()}");
            }
            return result;
        }
        public async Task<long> GetTotalIncomeAsync(FilterModel filterModel)
        {
            var result = -1L;
            try
            {
                filterModel.Category.TransactionType = "income";
                filterModel = _helper.FilterModelToLower(filterModel);
                var transactions = new List<TransactionData>();

                if(filterModel.Account.Id != null)
                {
                    transactions = await _context.Transactions.Where(x => x.TransactionDate >= filterModel.StartDate && x.TransactionDate <= filterModel.EndDate && x.Category.Id == filterModel.Category.Id && x.Account.Id == filterModel.Account.Id).ToListAsync();
                }
                else
                {
                    transactions = await _context.Transactions.Where(x => x.TransactionDate >= filterModel.StartDate && x.TransactionDate <= filterModel.EndDate && x.Category.Id == filterModel.Category.Id).ToListAsync();
                }

                result = Convert.ToInt64(transactions.Select(x => x.Amount).Sum());

                _logger.LogInformation($"{_helper.RequestResultMessage("Get", "Total Income", OperationEnum.RequestResult.Success)}. Sum: {result}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"{_helper.RequestResultMessage("Get", "Total Income", OperationEnum.RequestResult.Failed)}. StackTrace: {ex.InnerException!.ToString()}");
            }
            return result;
        }
        public async Task<long> GetTotalExpenseAsync(FilterModel filterModel)
        {
            var result = -1L;
            try
            {
                filterModel.Category.TransactionType = "expense";
                filterModel = _helper.FilterModelToLower(filterModel);

                var transactions = new List<TransactionData>();
                if (filterModel.Account.Id != null)
                {
                    transactions = await _context.Transactions.Where(x=> x.TransactionDate >= filterModel.StartDate && x.TransactionDate <= filterModel.EndDate && x.Category.Id == filterModel.Category.Id && x.Account.Id == filterModel.Account.Id).ToListAsync();
                }
                else
                {
                    transactions = await _context.Transactions.Where(x => x.TransactionDate >= filterModel.StartDate && x.TransactionDate <= filterModel.EndDate && x.Category.Id == filterModel.Category.Id).ToListAsync();
                }
                
                result = Convert.ToInt64(transactions.Select(x => x.Amount).Sum());
                _logger.LogInformation($"{_helper.RequestResultMessage("Get", "Total Expense", OperationEnum.RequestResult.Success)}. Sum: {result}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"{_helper.RequestResultMessage("Get", "Total Expense", OperationEnum.RequestResult.Failed)}. StackTrace: {ex.InnerException!.ToString()}");
            }
            return result;
        }
        public async Task<long> GetTotalBalanceAsync(FilterModel filterModel)
        {
            var result = long.MinValue;
            try
            {
                var totalIncome = await GetTotalIncomeAsync(filterModel);
                var totalExpense = await GetTotalExpenseAsync(filterModel);
                result = totalIncome - totalExpense;
                _logger.LogInformation($"{_helper.RequestResultMessage("Get", "Total Balance", OperationEnum.RequestResult.Success)}. Total Balance: {result}.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"{_helper.RequestResultMessage("Get", "Total Balance", OperationEnum.RequestResult.Failed)}. StackTrace: {ex.InnerException!.ToString()}");
            }
            return result;
        }

        //currently, this api is for the diagram and the diagram only care about category and its total spent amount
        public async Task<List<TotalSpentPerCategoryModel>> GetTotalSpentPerCategory(string transactionType, FilterModel filterModel)
        {
            var result = new List<TotalSpentPerCategoryModel>();
            try
            {
                filterModel = _helper.FilterModelToLower(filterModel);
                result = await _context.Transactions.Where(w => w.Category.TransactionType == filterModel.Category.TransactionType && w.TransactionDate >= filterModel.StartDate && w.TransactionDate <= filterModel.EndDate).GroupBy(g => g.Category.TransactionCategory).Select(s => new TotalSpentPerCategoryModel
                {
                    Category = new CategoryModel {
                        TransactionCategory = s.Key
                    },
                    TotalSpent = s.Sum(x => x.Amount)
                }).ToListAsync();

                _logger.LogInformation($"{_helper.RequestResultMessage("Get", "Total Spent Per Category", OperationEnum.RequestResult.Success)}. Sum: {result.Count()}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"{_helper.RequestResultMessage("Get", "Total Spent Per Category", OperationEnum.RequestResult.Failed)}. StackTrace: {ex.InnerException!.ToString()}");
            }
            return result;
        }
    }
}
