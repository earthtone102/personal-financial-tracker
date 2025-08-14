using PersonalFinancialTracker.Model;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalFinancialTracker.Data
{
    public class TransactionData:BaseModel
    {
        public Guid Id { get; set; }
        //public Guid UserId { get; set; }

        [Column(TypeName = "timestamp without time zone")]
        public DateTime? TransactionDate { get; set; }

        [ForeignKey(nameof(Account))]
        public Guid? AccountId { get; set; }
        public Account Account { get; set; }
        public long? Amount { get; set; }

        [ForeignKey(nameof(Category))]
        public Guid? CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
