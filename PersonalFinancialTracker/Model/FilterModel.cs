namespace PersonalFinancialTracker.Model
{
  public class FilterModel
  {
    public DateTime? StartDate { get; set; } = DateTime.MinValue;
    public DateTime? EndDate { get; set; } = DateTime.MaxValue;
    public CategoryModel Category { get; set; }
    public AccountModel Account { get; set; }
  }
}
