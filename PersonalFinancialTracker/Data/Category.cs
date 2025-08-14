using PersonalFinancialTracker.Model;

namespace PersonalFinancialTracker.Data
{
    public class Category:BaseModel
    {
        public Guid Id { get; set; }
        public string? TransactionCategory { get; set; } = null;
        public string? TransactionType { get; set; } = null;
    }
}
