using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DynamicRulesDemo.Models;
using DynamicRulesDemo.Models.Db;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;

namespace DynamicRulesDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RulesController : RuleBasedController
    {

        public RulesController(ILogger<RulesController> logger, OneTechDbContext context, IMemoryCache cache) : base(logger, context, cache)
        {
        }

        // GET: api/Rules
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Rule>>> GetExpenses()
        {
            return await Db.Rules.ToListAsync();
        }

        // GET: api/Rules/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Rule>> GetRule([FromRoute]string id)
        {
            var rule = await Db.Rules.FindAsync(id);

            if (rule == null)
            {
                return NotFound();
            }

            return rule;
        }

        // PUT: api/Rules/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRule([FromRoute]string id, [FromBody]Rule Rule)
        {
            if (id != Rule.Id)
            {
                return BadRequest();
            }
            Db.Entry(Rule).State = EntityState.Modified;

            try
            {
                await Db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RuleExists(id))
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

        // POST: api/Rules
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<Rule>> PostRule([FromBody]Rule rule)
        {
            Db.Rules.Add(rule);
            try
            {
                await Db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (RuleExists(rule.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetRule", new { id = rule.Id }, rule);
        }

        // DELETE: api/Rules/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Rule>> DeleteRule([FromRoute]string id)
        {
            var rule = await Db.Rules.FindAsync(id);
            if (rule == null)
            {
                return NotFound();
            }
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            Db.Rules.Remove(rule);
            await Db.SaveChangesAsync();

            return rule;
        }

        private bool RuleExists(string id)
        {
            return Db.Expenses.Any(e => e.Id == id);
        }
    }
}
