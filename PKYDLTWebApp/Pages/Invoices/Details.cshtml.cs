using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ClinicManagement.Data;
using ClinicManagement.Models;

namespace PKYDLTWebApp.Pages.Invoices
{
    [Authorize]
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Invoice? Invoice { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Invoice = await _context.Invoices
                .Include(i => i.Sale)
                    .ThenInclude(s => s.Customer)
                .Include(i => i.Sale)
                    .ThenInclude(s => s.SaleDetails)
                    .ThenInclude(d => d.Product)
                .Include(i => i.CreatedByUser)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (Invoice == null)
            {
                return NotFound();
            }

            return Page();
        }
    }
}