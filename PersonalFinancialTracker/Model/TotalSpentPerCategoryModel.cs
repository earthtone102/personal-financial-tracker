using PersonalFinancialTracker.Helper;

namespace PersonalFinancialTracker.Model
{
    public class TotalSpentPerCategoryModel
    {
        public CategoryModel Category { get; set; }
        public long? TotalSpent { get; set; } = null;
    }
}
