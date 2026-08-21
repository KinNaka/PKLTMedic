using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ClinicManagement.Data;
using ClinicManagement.Models;

namespace PKYDLTWebApp.Pages.Invoices
{
    [Authorize]
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Invoice? Invoice { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Invoice = await _context.Invoices
                .Include(i => i.Sale)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (Invoice == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var invoice = await _context.Invoices
                .FirstOrDefaultAsync(i => i.Id == id);

            if (invoice == null)
            {
                return NotFound();
            }

            var number = invoice.InvoiceNumber;

            _context.Invoices.Remove(invoice);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Đã xóa hóa đơn " + number;
            return RedirectToPage("Index");
        }
    }
}