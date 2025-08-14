using AutoMapper;
using PersonalFinancialTracker.Model;
using PersonalFinancialTracker.Data;

namespace PersonalFinancialTracker.Helper
{
    public class ApplicationMapper: Profile
    {
        public ApplicationMapper()
        {
            // (dest, source)
            CreateMap<TransactionData, TransactionModel>().ReverseMap();
            CreateMap<Category,CategoryModel>().ReverseMap();
            CreateMap<Account, AccountModel>().ReverseMap();
        }
    }
}
