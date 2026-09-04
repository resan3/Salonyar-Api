using ApiSalonyar.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiSalonyar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentMethodsController : ControllerBase
    {
        private readonly ClinicDbContext _context;
        public PaymentMethodsController(ClinicDbContext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PaymentMethod>>> GetPaymentMethods()
            => await _context.PaymentMethods
                .Where(x => !x.IsDeleted && x.IsActive)
                .OrderBy(x => x.Title)
                .ToListAsync();

        [HttpPost]
        public async Task<ActionResult<PaymentMethod>> PostPaymentMethod(PaymentMethod item)
        {
            ModelState.Clear();
            item.IsDeleted = false;
            item.IsActive = true;
            item.CreatedAt = DateTime.Now;
            _context.PaymentMethods.Add(item);
            await _context.SaveChangesAsync();
            return Ok(item);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutPaymentMethod(int id, PaymentMethod item)
        {
            ModelState.Clear();
            var existing = await _context.PaymentMethods.FindAsync(id);
            if (existing == null) return NotFound();
            existing.Title = item.Title;
            existing.Code = item.Code;
            existing.RequiresCode = item.RequiresCode;
            existing.CodeLabel = item.CodeLabel;
            existing.Description = item.Description;
            existing.IsActive = item.IsActive;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePaymentMethod(int id)
        {
            var item = await _context.PaymentMethods.FindAsync(id);
            if (item == null) return NotFound();
            item.IsDeleted = true;
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
