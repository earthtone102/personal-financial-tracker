using Microsoft.AspNetCore.Mvc;
using PersonalFinancialTracker.Helper;
using PersonalFinancialTracker.Model;
using PersonalFinancialTracker.Repository;

namespace PersonalFinancialTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountRepository _accountRepository;
        private readonly OperationHelper _helper;
        public AccountController(IAccountRepository accountRepository, OperationHelper helper)
        {
            _accountRepository = accountRepository;
            _helper = helper;
        }

        [HttpPost("Add")]
        public async Task<ActionResult<string>> AddAccount([FromQuery]AccountModel accountModel)
        {
            var id = await _accountRepository.AddAccountAsync(accountModel);
            if(id == "")
            {
                return BadRequest(new { error = _helper.RequestResultMessage("Add", "Account", OperationEnum.RequestResult.Failed) });
            }
            return Ok(id);
        }

    }
}
