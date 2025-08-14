using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace PersonalFinancialTracker.Data
{
    public class TransactionContext: DbContext
    {
        public TransactionContext(DbContextOptions<TransactionContext> options): base (options)
        {

        }
        //table name
        public DbSet<TransactionData> Transactions { get; set; } = null!;
        public DbSet<Account> Accounts { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
    }
}
