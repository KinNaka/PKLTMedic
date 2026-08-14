using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ClinicManagement.Data;
using ClinicManagement.Models;

namespace PKYDLTWebApp.Pages.Imports
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<ClinicManagement.Models.ImportOrder> ImportOrders { get; set; } = new List<ClinicManagement.Models.ImportOrder>();

        public async Task OnGetAsync()
        {
            ImportOrders = await _context.ImportOrders
                .Include(io => io.Supplier)
                .OrderByDescending(io => io.ImportDate)
                .Take(100)
                .ToListAsync();
        }
    }
}