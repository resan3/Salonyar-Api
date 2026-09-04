using ApiSalonyar.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiSalonyar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExpensesController : ControllerBase
    {
        private readonly ClinicDbContext _context;
        public ExpensesController(ClinicDbContext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Expense>>> GetExpenses(
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to)
        {
            var query = _context.Expenses
                .Include(x => x.Category)
                .Include(x => x.PaymentMethod)
                .Where(x => !x.IsDeleted);

            if (from.HasValue) query = query.Where(x => x.ExpenseDate >= from.Value);
            if (to.HasValue) query = query.Where(x => x.ExpenseDate <= to.Value.AddDays(1));

            return await query.OrderByDescending(x => x.ExpenseDate).ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Expense>> PostExpense(Expense item)
        {
            ModelState.Clear();
            item.IsDeleted = false;
            item.CreatedAt = DateTime.Now;
            item.BranchId = item.BranchId == 0 ? 1 : item.BranchId;
            item.ExpenseDate = item.ExpenseDate == default ? DateTime.Now : item.ExpenseDate;
            _context.Expenses.Add(item);
            await _context.SaveChangesAsync();
            return Ok(item);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutExpense(int id, Expense item)
        {
            ModelState.Clear();
            var existing = await _context.Expenses.FindAsync(id);
            if (existing == null) return NotFound();
            existing.Title = item.Title;
            existing.Amount = item.Amount;
            existing.CategoryId = item.CategoryId;
            existing.PaymentMethodId = item.PaymentMethodId;
            existing.Description = item.Description;
            existing.ExpenseDate = item.ExpenseDate;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExpense(int id)
        {
            var item = await _context.Expenses.FindAsync(id);
            if (item == null) return NotFound();
            item.IsDeleted = true;
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
