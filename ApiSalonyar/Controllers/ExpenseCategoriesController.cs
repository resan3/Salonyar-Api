using ApiSalonyar.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiSalonyar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExpenseCategoriesController : ControllerBase
    {
        private readonly ClinicDbContext _context;
        public ExpenseCategoriesController(ClinicDbContext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExpenseCategory>>> GetCategories()
            => await _context.ExpenseCategories
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.Title)
                .ToListAsync();

        [HttpPost]
        public async Task<ActionResult<ExpenseCategory>> PostCategory(ExpenseCategory item)
        {
            ModelState.Clear();
            item.IsDeleted = false;
            _context.ExpenseCategories.Add(item);
            await _context.SaveChangesAsync();
            return Ok(item);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategory(int id, ExpenseCategory item)
        {
            ModelState.Clear();
            var existing = await _context.ExpenseCategories.FindAsync(id);
            if (existing == null) return NotFound();
            existing.Title = item.Title;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var item = await _context.ExpenseCategories.FindAsync(id);
            if (item == null) return NotFound();
            item.IsDeleted = true;
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
