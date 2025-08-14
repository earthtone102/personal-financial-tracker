using Microsoft.AspNetCore.Mvc;
using PersonalFinancialTracker.Data;
using PersonalFinancialTracker.Helper;
using PersonalFinancialTracker.Model;
using PersonalFinancialTracker.Repository;
using System.Threading.Tasks;


namespace PersonalFinancialTracker.Controllers
{   
    [ApiController]
    [Route("api/[Controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly OperationHelper _helper;

        public TransactionController(ITransactionRepository transactionRepository, OperationHelper helper)
        {
            _transactionRepository = transactionRepository;
            _helper = helper;
        }

        [HttpPost("Add")]
        public async Task<ActionResult<string>> Add([FromBody] TransactionModel transactionModel)
        {
            var result = await _transactionRepository.AddTransactionAsync(transactionModel);
            if (result == "")
                return BadRequest(new { error = _helper.RequestResultMessage("Add", "Transaction", OperationEnum.RequestResult.Failed) });
            return Ok(result);
        }

        [HttpDelete("Delete/{id}")]
        public async Task<ActionResult<bool>> DeleteTransaction([FromRoute] Guid id)
        {
            var result = await _transactionRepository.DeleteTransactionAsync(id);
            if (result == false)
                return BadRequest(new { error = _helper.RequestResultMessage("Delete", "Transaction", OperationEnum.RequestResult.Failed)});
            return Ok(result);
        }

        [HttpPut("Update/{id}")]
        public async Task<ActionResult<bool>> UpdateTransaction([FromRoute] Guid id, [FromBody] TransactionModel transactionUpdate)
        {
            var result = await _transactionRepository.UpdateTransactionAsync(id, transactionUpdate);
            if (result == false)
                return BadRequest(new { error = _helper.RequestResultMessage("Update", "Transaction", OperationEnum.RequestResult.Failed) });
            return Ok(result);
        }
        public async Task<ActionResult<List<TransactionModel>>> GetAllTransactions([FromQuery] FilterModel filter)
        {
            var records = await _transactionRepository.GetAllTransactionAsync(filter);

            if(records.Count<1)
            {
                return BadRequest(new { error = _helper.RequestResultMessage("Get All", "Transaction", OperationEnum.RequestResult.Failed) });
            }
            return Ok(records);
        }

        [HttpGet("GetTotalIncome")]
        public async Task<ActionResult<long>> GetTotalIncome([FromQuery] FilterModel filter)
        {
            var totalIncome = await _transactionRepository.GetTotalIncomeAsync(filter);

            if (totalIncome <0L)
            {
                return BadRequest(new { error = _helper.RequestResultMessage("Get", "Total Income", OperationEnum.RequestResult.Failed) });
            }
            return Ok(totalIncome);
        }

        [HttpGet("GetTotalExpense")]
        public async Task<ActionResult<long>> GetTotalExpense([FromQuery] FilterModel filter)
        {
            var totalExpense = await _transactionRepository.GetTotalExpenseAsync(filter);

            if (totalExpense < 0L)
            {
                return BadRequest(new { error = _helper.RequestResultMessage("Get", "Total Expense", OperationEnum.RequestResult.Failed) });
            }
            return Ok(totalExpense);
        }

        [HttpGet("GetTotalBalance")]
        public async Task<IActionResult> GetTotalBalance([FromQuery] FilterModel filter)
        {
            var totalBalance = await _transactionRepository.GetTotalBalanceAsync(filter);
            if (totalBalance == long.MinValue)
            {
                return BadRequest(new { error = _helper.RequestResultMessage("Get", "Total Balance", OperationEnum.RequestResult.Failed) });
            }
            return Ok(totalBalance);
        }

        [HttpGet("{transactionType}/GetTotalPerCategory")]
        public async Task<ActionResult<List<TotalSpentPerCategoryModel>>> GetTotalSpentPerCategory([FromRoute] string transactionType, [FromQuery] FilterModel filter)
        {
            var result = await _transactionRepository.GetTotalSpentPerCategory(transactionType, filter);
            if(result.Count<1)
            {
                return BadRequest(new { error = _helper.RequestResultMessage("Get", "Total Spent Per Category", OperationEnum.RequestResult.Failed) });
            }
            return Ok(result);
        }
    }
}
