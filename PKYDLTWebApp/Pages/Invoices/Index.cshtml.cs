using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ClinicManagement.Data;
using ClinicManagement.Models;

namespace PKYDLTWebApp.Pages.Invoices
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<ClinicManagement.Models.Invoice> Invoices { get; set; } = new List<ClinicManagement.Models.Invoice>();

        public async Task OnGetAsync()
        {
            Invoices = await _context.Invoices
                .Include(i => i.Sale)
                    .ThenInclude(s => s.Customer)
                .OrderByDescending(i => i.InvoiceDate)
                .Take(100)
                .ToListAsync();
        }
    }
}