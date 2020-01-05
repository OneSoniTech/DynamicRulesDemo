using DynamicRulesDemo.Models;
using DynamicRulesDemo.Models.Db;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamicRulesDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExpenseLogsController : RuleBasedController
    {

        public ExpenseLogsController(ILogger<ExpenseLogsController> logger, OneTechDbContext context, IMemoryCache cache) : base(logger, context, cache)
        {
        }

        // GET: api/ExpenseLogs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExpenseLog>>> GetExpenses()
        {
            return await Db.Expenses.ToListAsync();
        }

        // GET: api/ExpenseLogs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ExpenseLog>> GetExpenseLog([FromRoute]string id)
        {
            var expenseLog = await Db.Expenses.FindAsync(id);

            if (expenseLog == null)
            {
                return NotFound();
            }

            return expenseLog;
        }

        // PUT: api/ExpenseLogs/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutExpenseLog([FromRoute]string id, [FromBody]ExpenseLog expenseLog)
        {
            if (id != expenseLog.Id)
            {
                return BadRequest();
            }
            ExecuteRules("CoreValidation_ExpenseLog", expenseLog);
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            Db.Entry(expenseLog).State = EntityState.Modified;

            try
            {
                await Db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExpenseLogExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/ExpenseLogs
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<ExpenseLog>> PostExpenseLog([FromBody]ExpenseLog expenseLog)
        {
            ExecuteRules("CoreValidation_ExpenseLog", expenseLog);
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            Db.Expenses.Add(expenseLog);
            try
            {
                await Db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ExpenseLogExists(expenseLog.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetExpenseLog", new { id = expenseLog.Id }, expenseLog);
        }

        // DELETE: api/ExpenseLogs/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ExpenseLog>> DeleteExpenseLog([FromRoute]string id)
        {
            var expenseLog = await Db.Expenses.FindAsync(id);
            if (expenseLog == null)
            {
                return NotFound();
            }
            ExecuteRules("CoreValidation_ExpenseLog", expenseLog);
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            Db.Expenses.Remove(expenseLog);
            await Db.SaveChangesAsync();

            return expenseLog;
        }

        private bool ExpenseLogExists(string id)
        {
            return Db.Expenses.Any(e => e.Id == id);
        }
    }
}
