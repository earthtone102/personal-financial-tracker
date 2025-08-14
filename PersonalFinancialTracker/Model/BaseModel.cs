using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalFinancialTracker.Model
{
    public class BaseModel
    {
        [Column(TypeName = "timestamp without time zone")]
        public DateTime? CreatedTime { get; set; } = DateTime.MinValue;
        [Column(TypeName = "timestamp without time zone")]
        public DateTime? UpdatedTime { get; set; } = DateTime.MinValue;
    }
}
