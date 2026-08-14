using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ClinicManagement.Data;
using ClinicManagement.Models;

namespace PKYDLTWebApp.Pages.Inventory
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<ClinicManagement.Models.Inventory> Inventories { get; set; } = new List<ClinicManagement.Models.Inventory>();

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? FilterType { get; set; } // all, low, expiring, expired

        public async Task OnGetAsync()
        {
            var query = _context.Inventories
                .Include(i => i.Product)
                    .ThenInclude(p => p.Supplier)
                .AsQueryable();

            // Tìm kiếm
            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                query = query.Where(i =>
                    i.Product.ProductCode.Contains(SearchTerm) ||
                    i.Product.ProductName.Contains(SearchTerm)
                );
            }

            // Lọc theo trạng thái
            switch (FilterType)
            {
                case "low":
                    query = query.Where(i => i.Quantity < i.MinimumQuantity);
                    break;
                case "expiring":
                    var thirtyDaysLater = DateTime.Now.AddDays(30);
                    query = query.Where(i => 
                        i.Product.ExpiryDate != null &&
                        i.Product.ExpiryDate <= thirtyDaysLater &&
                        i.Product.ExpiryDate > DateTime.Now
                    );
                    break;
                case "expired":
                    query = query.Where(i => 
                        i.Product.ExpiryDate != null &&
                        i.Product.ExpiryDate < DateTime.Now
                    );
                    break;
            }

            Inventories = await query
                .OrderBy(i => i.Quantity)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostUpdateAsync(int id, int quantity)
        {
            var inventory = await _context.Inventories.FindAsync(id);

            if (inventory == null)
            {
                return NotFound();
            }

            inventory.Quantity = quantity;
            inventory.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();

            return new JsonResult(new { success = true });
        }
    }
}
