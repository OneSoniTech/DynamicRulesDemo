using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamicRulesDemo.Models.Db
{
    public class OneTechDbContext : DbContext
    {
        public OneTechDbContext(DbContextOptions<OneTechDbContext> options)
        : base(options)
        {
        }
        public DbSet<Rule> Rules { get; set; }
        public DbSet<ExpenseLog> Expenses { get; set; }
    }
}
