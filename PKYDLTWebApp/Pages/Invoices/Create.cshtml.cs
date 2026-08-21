using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ClinicManagement.Data;
using ClinicManagement.Models;
using System.ComponentModel.DataAnnotations;

namespace PKYDLTWebApp.Pages.Invoices
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        // ============ BOUND PROPERTIES ============

        [BindProperty]
        [Required(ErrorMessage = "Vui lòng chọn đơn bán hàng")]
        [Display(Name = "Đơn bán hàng")]
        public int SaleId { get; set; }

        [BindProperty]
        [Display(Name = "Số hóa đơn")]
        [StringLength(50)]
        public string InvoiceNumber { get; set; } = string.Empty;

        [BindProperty]
        [Display(Name = "Ngày lập hóa đơn")]
        public DateTime InvoiceDate { get; set; } = DateTime.Now;

        [BindProperty]
        [Display(Name = "Loại hóa đơn")]
        [StringLength(100)]
        public string InvoiceType { get; set; } = "Chứng từ tự in";

        [BindProperty]
        [Display(Name = "Trạng thái")]
        [StringLength(50)]
        public string Status { get; set; } = "Chưa in";

        [BindProperty]
        [Display(Name = "Ghi chú thanh toán")]
        [StringLength(100)]
        public string? PaymentNote { get; set; }

        [BindProperty]
        [Display(Name = "Ghi chú")]
        [StringLength(500)]
        public string? Notes { get; set; }

        // ============ DỮ LIỆU HIỂN THỊ ============

        public List<Sale> Sales { get; set; } = new();

        public async Task OnGetAsync()
        {
            await LoadDataAsync();
            InvoiceNumber = await GenerateInvoiceNumberAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (InvoiceDate < new DateTime(2000, 1, 1))
            {
                InvoiceDate = DateTime.Now;
            }

            if (string.IsNullOrWhiteSpace(InvoiceNumber))
            {
                InvoiceNumber = await GenerateInvoiceNumberAsync();
            }

            if (!ModelState.IsValid)
            {
                await LoadDataAsync();
                return Page();
            }

            // Kiểm tra trùng số hóa đơn
            var duplicate = await _context.Invoices
                .AnyAsync(i => i.InvoiceNumber == InvoiceNumber.Trim());
            if (duplicate)
            {
                ModelState.AddModelError("InvoiceNumber", "Số hóa đơn này đã tồn tại");
                await LoadDataAsync();
                return Page();
            }

            var sale = await _context.Sales
                .Include(s => s.Customer)
                .FirstOrDefaultAsync(s => s.Id == SaleId);

            if (sale == null)
            {
                ModelState.AddModelError("", "Đơn bán hàng không tồn tại");
                await LoadDataAsync();
                return Page();
            }

            var invoice = new Invoice
            {
                SaleId = sale.Id,
                Sale = sale,
                InvoiceNumber = InvoiceNumber.Trim(),
                InvoiceDate = InvoiceDate,
                InvoiceType = string.IsNullOrWhiteSpace(InvoiceType) ? "Chứng từ tự in" : InvoiceType,
                Status = string.IsNullOrWhiteSpace(Status) ? "Chưa in" : Status,
                PaymentNote = PaymentNote,
                Notes = Notes,
                CreatedByUserId = await GetCurrentUserIdAsync(),
                CreatedAt = DateTime.Now
            };

            invoice.SyncFromSale();

            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Đã tạo hóa đơn " + invoice.InvoiceNumber;
            return RedirectToPage("Index");
        }

        // ============ HELPERS ============

        private async Task LoadDataAsync()
        {
            Sales = await _context.Sales
                .Include(s => s.Customer)
                .OrderByDescending(s => s.SaleDate)
                .ThenByDescending(s => s.Id)
                .Take(200)
                .ToListAsync();
        }

        private async Task<int?> GetCurrentUserIdAsync()
        {
            if (User.Identity == null || string.IsNullOrWhiteSpace(User.Identity.Name))
                return null;

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == User.Identity.Name);

            return user?.Id;
        }

        private async Task<string> GenerateInvoiceNumberAsync()
        {
            var last = await _context.Invoices
                .OrderByDescending(i => i.Id)
                .Select(i => i.InvoiceNumber)
                .FirstOrDefaultAsync();

            int num = 1;
            if (!string.IsNullOrEmpty(last))
            {
                var parts = last.Split('/');
                if (parts.Length >= 1 && int.TryParse(parts[^1], out int parsed))
                {
                    num = parsed + 1;
                }
            }

            return $"HĐ/{DateTime.Now:yyyyMM}/{num:D5}";
        }
    }
}