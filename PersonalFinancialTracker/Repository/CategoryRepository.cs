using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PersonalFinancialTracker.Data;
using PersonalFinancialTracker.Helper;
using PersonalFinancialTracker.Model;
using Serilog;
using System.Transactions;

namespace PersonalFinancialTracker.Repository
{
    public class CategoryRepository: ICategoryRepository
    {
        private readonly TransactionContext _context;
        private readonly IMapper _mapper;
        private readonly OperationHelper _helper;
        private readonly ILogger<CategoryRepository> _logger;
        public CategoryRepository(TransactionContext transactionContext, IMapper mapper, OperationHelper helper, ILogger<CategoryRepository> logger)
        {
            _context = transactionContext;
            _mapper = mapper;
            _helper = helper;
            _logger = logger;
        }

        public async Task<List<CategoryModel>> GetAllCategoryAsync()
        {
            var records = new List<Category>();
            try
            {
                records = await _context.Categories.ToListAsync();
                _logger.LogInformation($"{_helper.RequestResultMessage("Get All", "Category", OperationEnum.RequestResult.Success)}. Count: {records.Count()}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"{_helper.RequestResultMessage("Get All", "Category", OperationEnum.RequestResult.Failed)}. StackTrace: {ex.InnerException!.ToString()}");
            }

            return _mapper.Map<List<CategoryModel>>(records);
        }

        public async Task<string> AddCategoryAsync(CategoryModel categoryModel)
        {
            var id = "";
            #region check if exist
            categoryModel = _helper.CategoryModelToLower(categoryModel);
            var check = await _context.Categories.Where(x => x.TransactionType == categoryModel.TransactionType && x.TransactionCategory == categoryModel.TransactionCategory).FirstOrDefaultAsync();
            #endregion

            if (check == null)
            {
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    try
                    {
                        var newCategory = _mapper.Map<Category>(categoryModel);
                        newCategory.Id = new Guid();
                        newCategory.CreatedTime = DateTime.Now;

                        _context.Categories.Add(newCategory);
                        await _context.SaveChangesAsync();

                        id = newCategory.Id.ToString();
                        _logger.LogInformation($"{_helper.RequestResultMessage("Add", "Category", OperationEnum.RequestResult.Success)}. Id: {id}");
                        scope.Complete();
                    }
                    catch (Exception ex)
                    {
                        id = "";
                        _logger.LogError($"{_helper.RequestResultMessage("Add", "Category", OperationEnum.RequestResult.Failed)}. StackTrace: {ex.InnerException!.ToString()}");
                    }
                }
            }
            else
            {
                id = check.Id.ToString();
                _logger.LogError($"Category with name {categoryModel.TransactionCategory} for transaction type {categoryModel.TransactionType} has already existed with id: {id}");
            }
            return id;
        }


        //delete category
        //get single category
        //update category (like name or what transaction type it will be for)
    }
}
