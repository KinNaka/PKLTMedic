using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ClinicManagement.Data;
using ClinicManagement.Models;

namespace PKYDLTWebApp.Pages.Adjustments
{
    [Authorize]
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public InventoryAdjustment? Adjustment { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Adjustment = await _context.InventoryAdjustments
                .Include(a => a.CreatedByUser)
                .Include(a => a.AdjustmentDetails)
                    .ThenInclude(d => d.Product)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (Adjustment == null)
            {
                return NotFound();
            }

            return Page();
        }
    }
}
