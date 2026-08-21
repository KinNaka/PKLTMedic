using Microsoft.AspNetCore.Mvc;
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

        public List<string> Statuses { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? StatusFilter { get; set; }

        public async Task OnGetAsync()
        {
            Statuses = await _context.Invoices
                .Select(i => i.Status)
                .Distinct()
                .OrderBy(s => s)
                .ToListAsync();

            var query = _context.Invoices
                .Include(i => i.Sale)
                    .ThenInclude(s => s.Customer)
                .Include(i => i.CreatedByUser)
                .AsQueryable();

            // Tìm kiếm theo số HĐ / khách hàng / mã đơn bán / số điện thoại
            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                var keyword = SearchTerm.Trim();
                query = query.Where(i =>
                    i.InvoiceNumber.Contains(keyword) ||
                    i.CustomerName.Contains(keyword) ||
                    (i.CustomerPhone != null && i.CustomerPhone.Contains(keyword)) ||
                    (i.Sale != null && i.Sale.SaleCode.Contains(keyword)) ||
                    (i.Sale != null && i.Sale.Customer != null && i.Sale.Customer.FullName.Contains(keyword)));
            }

            // Lọc theo trạng thái
            if (!string.IsNullOrWhiteSpace(StatusFilter) && StatusFilter != "all")
            {
                query = query.Where(i => i.Status == StatusFilter);
            }

            Invoices = await query
                .OrderByDescending(i => i.InvoiceDate)
                .ThenByDescending(i => i.Id)
                .Take(200)
                .ToListAsync();
        }
    }
}