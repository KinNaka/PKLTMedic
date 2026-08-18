using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ClinicManagement.Data;
using ClinicManagement.Models;

namespace PKYDLTWebApp.Pages.Imports
{
    [Authorize]
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        public ImportOrder ImportOrder { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var importOrder = await _context.ImportOrders
                .Include(io => io.Supplier)
                .Include(io => io.CreatedByUser)
                .Include(io => io.ImportDetails)
                    .ThenInclude(d => d.Product)
                .FirstOrDefaultAsync(io => io.Id == Id);

            if (importOrder == null)
            {
                return NotFound();
            }

            ImportOrder = importOrder;
            return Page();
        }
    }
}