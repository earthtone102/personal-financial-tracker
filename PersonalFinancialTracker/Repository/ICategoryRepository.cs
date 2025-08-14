using PersonalFinancialTracker.Model;

namespace PersonalFinancialTracker.Repository
{
    public interface ICategoryRepository
    {
        Task<List<CategoryModel>> GetAllCategoryAsync();
        Task<string> AddCategoryAsync(CategoryModel data);
    }
}
