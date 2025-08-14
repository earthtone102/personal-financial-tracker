using PersonalFinancialTracker.Helper;

namespace PersonalFinancialTracker.Model
{
    public class TransactionModel
    {
        public Guid? Id { get; set; }
        public DateTime? TransactionDate { get; set; } = DateTime.MinValue;
        public long? Amount { get; set; } = null;
        public CategoryModel Category { get; set; }
        public AccountModel Account { get; set; }
    }
}
