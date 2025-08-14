using PersonalFinancialTracker.Model;
using static PersonalFinancialTracker.Helper.OperationEnum;

namespace PersonalFinancialTracker.Helper
{
    public class OperationHelper
    {
        public string RequestResultMessage(string operation, string entityName, RequestResult status)
        {
            var result = string.Empty;
            if (status == RequestResult.Success)
            {
                result = $"Succesfully {operation} new {entityName}";
            }
            else if (status == RequestResult.Failed)
            {
                result = $"Failed {operation} new {entityName}";
            }
            return result;
        }

        public AccountModel AccountModelToLower(AccountModel accountModel)
        {
            accountModel.AccountName = accountModel.AccountName?.ToLower();
            return accountModel;
        }
        public CategoryModel CategoryModelToLower(CategoryModel categoryModel)
        {
            categoryModel.TransactionCategory = categoryModel.TransactionCategory?.ToLower();
            categoryModel.TransactionType = categoryModel.TransactionType?.ToLower();
            return categoryModel;
        }
        public FilterModel FilterModelToLower(FilterModel filterModel)
        {
            filterModel.Account = AccountModelToLower(filterModel.Account);
            filterModel.Category = CategoryModelToLower(filterModel.Category);
            return filterModel;
        }
        public TransactionModel TransactionModelToLower(TransactionModel transactionModel)
        {
            transactionModel.Account = AccountModelToLower(transactionModel.Account);
            transactionModel.Category = CategoryModelToLower(transactionModel.Category);
            return transactionModel;
        }
    }
}
