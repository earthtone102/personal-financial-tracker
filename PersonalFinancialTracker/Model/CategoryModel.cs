using PersonalFinancialTracker.Helper;

namespace PersonalFinancialTracker.Model
{
    public class CategoryModel
    {
        public Guid? Id { get; set; } = null;
        public string? TransactionCategory { get; set; } = null; //food, clothes
        public string? TransactionType { get; set; } = null; //income, expense, transfer,
    }
}
