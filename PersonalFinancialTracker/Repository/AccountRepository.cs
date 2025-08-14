using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PersonalFinancialTracker.Data;
using PersonalFinancialTracker.Helper;
using PersonalFinancialTracker.Model;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Transactions;

namespace PersonalFinancialTracker.Repository
{
    public class AccountRepository: IAccountRepository
    {
        private readonly IMapper _mapper;
        private readonly TransactionContext _context;
        private readonly OperationHelper _helper;
        private readonly ILogger<AccountRepository> _logger;

        public AccountRepository(IMapper mapper, TransactionContext context, OperationHelper helper, ILogger<AccountRepository> logger)
        {
            _mapper = mapper;
            _context = context;
            _helper = helper;
            _logger = logger;
        }
        public async Task<string> AddAccountAsync(AccountModel accountModel)
        {
            #region check if its exist
            accountModel = _helper.AccountModelToLower(accountModel);
            var checkAccount = _context.Accounts.Where(x => x.AccountName == accountModel.AccountName).FirstOrDefault();
            #endregion
            var id = "";
            if(checkAccount == null)
            {
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    try
                    {
                        var newAccount = _mapper.Map<Account>(accountModel);
                        newAccount.Id = new Guid();
                        newAccount.CreatedTime = DateTime.Now;

                        _context.Accounts.Add(newAccount);
                        await _context.SaveChangesAsync();

                        id = newAccount.Id.ToString();
                        _logger.LogInformation($"{_helper.RequestResultMessage("Add", "Account", OperationEnum.RequestResult.Success)}. Id: {id}");
                        scope.Complete();
                    }
                    catch (Exception ex)
                    {
                        id = "";
                        _logger.LogError($"{_helper.RequestResultMessage("Add", "Account", OperationEnum.RequestResult.Failed)}. StackTrace: {ex.InnerException}");
                    }
                }
            }
            else
            {
                id = checkAccount.Id.ToString();
                _logger.LogError($"Account with name {accountModel.AccountName} already existed with id: {id}");
            }
            return id;
        }
    }
}
