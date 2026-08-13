using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ClinicManagement.Data;
using ClinicManagement.Models;
using System.Text.Json;

namespace PKYDLTWebApp.Pages.Sales
{
    [Authorize]
    public class QuickModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public QuickModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Sale Sale { get; set; } = new();

        public List<SaleDetail> SaleDetails { get; set; } = new();
        public List<Product> Products { get; set; } = new();
        public List<Customer> Customers { get; set; } = new();

        public async Task OnGetAsync()
        {
            Products = await _context.Products
                .Where(p => p.IsActive)
                .OrderBy(p => p.ProductName)
                .ToListAsync();

            Customers = await _context.Customers
                .OrderBy(c => c.FullName)
                .ToListAsync();

            // Generate sale code
            var lastSale = await _context.Sales
                .OrderByDescending(s => s.Id)
                .FirstOrDefaultAsync();

            var saleNumber = (lastSale != null ? int.Parse(lastSale.SaleCode.Split('-').Last()) : 0) + 1;
            Sale.SaleCode = $"SALE-{DateTime.Now:yyyyMMdd}-{saleNumber.ToString("D4")}";
            Sale.SaleDate = DateTime.Now;
            Sale.PaymentMethod = "Tiền mặt";
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (SaleDetails == null || !SaleDetails.Any())
            {
                ModelState.AddModelError("", "Vui lòng thêm sản phẩm");
                return Page();
            }

            Sale.CreatedAt = DateTime.Now;
            Sale.SaleDetails = SaleDetails;
            Sale.CalculateTotal();

            _context.Sales.Add(Sale);

            // Cập nhật tồn kho
            foreach (var detail in SaleDetails)
            {
                var inventory = await _context.Inventories
                    .FirstOrDefaultAsync(i => i.ProductId == detail.ProductId);

                if (inventory != null)
                {
                    inventory.Quantity -= detail.Quantity;
                    inventory.LastIssuedDate = DateTime.Now;
                }
            }

            await _context.SaveChangesAsync();

            // Tạo hóa đơn tự động
            var invoice = new Invoice
            {
                SaleId = Sale.Id,
                InvoiceNumber = $"HĐ{DateTime.Now:yyMMddHHmm}",
                InvoiceDate = DateTime.Now,
                InvoiceType = "Chứng từ tự in"
            };
            invoice.SyncFromSale();
            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Ghi nhận bán hàng thành công. Đơn bán: " + Sale.SaleCode;
            return RedirectToPage("Index");
        }

        public async Task<JsonResult> OnGetProductDetailsAsync(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
                return new JsonResult(new { error = "Sản phẩm không tồn tại" });

            return new JsonResult(new
            {
                productName = product.ProductName,
                unitPrice = product.RetailPrice,
                unit = product.Unit
            });
        }
    }
}
