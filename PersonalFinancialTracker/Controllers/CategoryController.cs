using Microsoft.AspNetCore.Mvc;
using PersonalFinancialTracker.Model;
using PersonalFinancialTracker.Repository;
using PersonalFinancialTracker.Helper;

namespace PersonalFinancialTracker.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly OperationHelper _helper;
        public CategoryController(ICategoryRepository categoryRepository, OperationHelper helper) {
            _categoryRepository = categoryRepository;
            _helper = helper;
        }
        public async Task<ActionResult<List<CategoryModel>>> GetAllCategory()
        {
            var response = await _categoryRepository.GetAllCategoryAsync();
            if(response.Count<1)
            {
                return BadRequest(new { error = _helper.RequestResultMessage("Get", "All Category", OperationEnum.RequestResult.Failed) });
            }
            return Ok(response);
        }
        [HttpPost("Add")]
        public async Task<ActionResult<string>> AddCategory([FromQuery]CategoryModel category)
        {
            var response = await _categoryRepository.AddCategoryAsync(category);
            if (response == "")
            {
                return BadRequest(new { error = _helper.RequestResultMessage("Add", "Category", OperationEnum.RequestResult.Failed) });
            }
            return Ok(response);
        }
    }
}
