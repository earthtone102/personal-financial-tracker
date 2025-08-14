using PersonalFinancialTracker.Model;

namespace PersonalFinancialTracker.Data
{
    public class Account:BaseModel
    {
        public Guid Id { get; set; }
        public string? AccountName { get; set; }

    }
}
