using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ClinicManagement.Data;
using ClinicManagement.Models;

namespace PKYDLTWebApp.Pages.Adjustments
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<InventoryAdjustment> Adjustments { get; set; } = new List<InventoryAdjustment>();

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? TypeFilter { get; set; }

        public List<string> Types { get; set; } = new()
        {
            "Kiểm kê",
            "Hư hỏng",
            "Hết hạn",
            "Điều chỉnh thủ công",
            "Khác"
        };

        public async Task OnGetAsync()
        {
            var query = _context.InventoryAdjustments
                .Include(a => a.CreatedByUser)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                query = query.Where(a =>
                    a.AdjustmentCode.Contains(SearchTerm) ||
                    a.AdjustmentType.Contains(SearchTerm) ||
                    (a.Notes != null && a.Notes.Contains(SearchTerm)));
            }

            if (!string.IsNullOrWhiteSpace(TypeFilter) && TypeFilter != "all")
            {
                query = query.Where(a => a.AdjustmentType == TypeFilter);
            }

            Adjustments = await query
                .OrderByDescending(a => a.AdjustmentDate)
                .ThenByDescending(a => a.Id)
                .ToListAsync();
        }
    }
}
