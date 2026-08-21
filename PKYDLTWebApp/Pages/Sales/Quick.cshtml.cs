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

        [BindProperty]
        public List<SaleDetail> SaleDetails { get; set; } = new();
        public List<Product> Products { get; set; } = new();
        public List<Customer> Customers { get; set; } = new();

        /// <summary>Trường ẩn - định in (invoice, invoice_noname, drug, none)</summary>
        [BindProperty]
        public string? PrintOption { get; set; }

        /// <summary>Đơn bán để hiển thị trường in sau đơn chuyển</summary>
        public Sale? PrintSale { get; set; }
        public string? PrintFormat { get; set; }

        /// <summary>Đơn bán gốc (Sao chép): điền sẵn khách hàng + sản phẩm + số lượng</summary>
        [BindProperty(SupportsGet = true)]
        public int? CopySaleId { get; set; }

        /// <summary>Đơn bán gốc hiển thị nhanh trường khi sao chép</summary>
        public string? CopySourceCode { get; set; }

        /// <summary>JSON data for cart items khi sao chép (khách hàng + sản phẩm + số lượng)</summary>
        public string CopyDetailsJson { get; set; } = "[]";

        /// <summary>JSON data for customer/product autocomplete (built server-side)</summary>
        public string CustomersJson { get; set; } = "[]";
        public string ProductsJson { get; set; } = "[]";

        public async Task OnGetAsync()
        {
            Products = await _context.Products
                .Where(p => p.IsActive)
                .OrderBy(p => p.ProductName)
                .ToListAsync();

            Customers = await _context.Customers
                .OrderBy(c => c.FullName)
                .ToListAsync();

            // Build autocomplete JSON payloads server-side
            BuildJson();

            // ============ IN DOCUMENT (redirect from OnPost) ============
            var printSaleIdText = TempData["PrintSaleId"] as string;
            if (!string.IsNullOrWhiteSpace(printSaleIdText))
            {
                var saleId = int.Parse(printSaleIdText);
                PrintFormat = TempData["PrintFormat"] as string;
                if (string.IsNullOrWhiteSpace(PrintFormat))
                {
                    PrintFormat = "invoice";
                }
                await LoadPrintDataAsync(saleId);
            }

            // Generate sale code
            var lastSale = await _context.Sales
                .OrderByDescending(s => s.Id)
                .FirstOrDefaultAsync();

            var saleNumber = (lastSale != null ? int.Parse(lastSale.SaleCode.Split('-').Last()) : 0) + 1;
            Sale.SaleCode = $"SALE-{DateTime.Now:yyyyMMdd}-{saleNumber.ToString("D4")}";
            Sale.SaleDate = DateTime.Now;
            Sale.PaymentMethod = "Tiền mặt";

            // ============ SAO CHÉP (kích hoạt) ============
            if (CopySaleId.HasValue)
            {
                await LoadCopyDataAsync();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadDataAsync();
                return Page();
            }

            if (Sale.SaleDate < new DateTime(2000, 1, 1))
            {
                Sale.SaleDate = DateTime.Now;
            }

            // Nhóm par produit pour éviter les doublons
            var validDetails = SaleDetails?
                .Where(d => d.ProductId > 0 && d.Quantity > 0)
                .GroupBy(d => d.ProductId)
                .Select(g => g.First())
                .ToList() ?? new List<SaleDetail>();

            if (!validDetails.Any())
            {
                ModelState.AddModelError("", "Vui lòng thêm sản phẩm để bán");
                await LoadDataAsync();
                return Page();
            }

            // Vérifie la disponibilité du stock pour chaque produit
            foreach (var detail in validDetails)
            {
                var inventory = await _context.Inventories
                    .FirstOrDefaultAsync(i => i.ProductId == detail.ProductId);

                int available = inventory?.Quantity ?? 0;

                if (available < detail.Quantity)
                {
                    var product = await _context.Products.FindAsync(detail.ProductId);
                    ModelState.AddModelError("",
                        $"Kho không đủ cho '{product?.ProductName ?? "?"}' (tồn: {available}, cần: {detail.Quantity})");
                }
            }

            if (!ModelState.IsValid)
            {
                await LoadDataAsync();
                return Page();
            }

            var userId = await GetCurrentUserIdAsync();

            Sale.CreatedAt = DateTime.Now;
            Sale.CreatedByUserId = userId;
            Sale.SalesPersonUserId = userId;
            Sale.SaleDate = DateTime.Now;
            Sale.Status = "Hoàn thành";
            Sale.SaleDetails = validDetails;
            Sale.CalculateTotal();
            Sale.PaymentStatus = (Sale.PaidAmount >= Sale.TotalAmount && Sale.TotalAmount > 0)
                ? "Đã thanh toán"
                : (Sale.PaidAmount > 0 ? "Một phần" : "Chưa thanh toán");

            _context.Sales.Add(Sale);

            // ============ TRẺ KHO (déduire le stock) ============
            foreach (var detail in validDetails)
            {
                var inventory = await _context.Inventories
                    .FirstOrDefaultAsync(i => i.ProductId == detail.ProductId);

                if (inventory != null)
                {
                    inventory.Quantity -= detail.Quantity;
                    inventory.LastIssuedDate = DateTime.Now;
                    inventory.UpdatedAt = DateTime.Now;
                }
            }

            await _context.SaveChangesAsync();

            // ============ LƯU HÓA ĐƠN (créer la facture) ============
            var invoice = new Invoice
            {
                SaleId = Sale.Id,
                InvoiceNumber = await GenerateInvoiceNumberAsync(),
                InvoiceDate = DateTime.Now,
                InvoiceType = "Chứng từ tự in",
                CreatedByUserId = userId,
                Status = "Đã in",
                PrintCount = 1,
                LastPrintedDate = DateTime.Now
            };

            var saleRef = await _context.Sales
                .Include(s => s.Customer)
                .FirstOrDefaultAsync(s => s.Id == Sale.Id);

            if (saleRef != null)
            {
                invoice.Sale = saleRef;
            }
            if (string.IsNullOrWhiteSpace(invoice.CustomerName))
            {
                invoice.CustomerName = Sale.Customer?.FullName ?? "Khách lẻ";
            }
            invoice.SyncFromSale();

            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Ghi nhận bán hàng + lưu hóa đơn + trừ kho thành công. Đơn bán: " + Sale.SaleCode;

            // ============ IN ĐƠCÚMENT (redirect to print receipt) ============
            var printOption = !string.IsNullOrWhiteSpace(PrintOption) ? PrintOption.Trim() : "none";
            if (printOption != "none")
            {
                TempData["PrintSaleId"] = Sale.Id.ToString();
                TempData["PrintFormat"] = printOption;
                return RedirectToPage("Quick");
            }

            return RedirectToPage("Index");
        }

        public async Task<JsonResult> OnGetProductDetailsAsync(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
                return new JsonResult(new { error = "Sản phẩm không tồn tại" });

            var inventory = await _context.Inventories
                .FirstOrDefaultAsync(i => i.ProductId == productId);

            return new JsonResult(new
            {
                productName = product.ProductName,
                unitPrice = product.RetailPrice,
                unit = product.Unit,
                stock = inventory?.Quantity ?? 0
            });
        }

        // ============ HELPERS ============

        private async Task LoadDataAsync()
        {
            Products = await _context.Products
                .Where(p => p.IsActive)
                .OrderBy(p => p.ProductName)
                .ToListAsync();

            Customers = await _context.Customers
                .OrderBy(c => c.FullName)
                .ToListAsync();

            BuildJson();
        }

        private void BuildJson()
        {
            CustomersJson = System.Text.Json.JsonSerializer.Serialize(
                Customers.Select(c => new { c.Id, Name = c.FullName, Phone = c.Phone ?? "" }).ToList());
            ProductsJson = System.Text.Json.JsonSerializer.Serialize(
                Products.Select(p => new { p.Id, Code = p.ProductCode, Name = p.ProductName, Price = p.RetailPrice, Unit = p.Unit }).ToList());
        }

        /// <summary>
        /// Sao chép: nạp đơn bán gốc và điền sẵn khách hàng + sản phẩm + số lượng
        /// cùng loại hàng cùng số lượng như đơn gốc
        /// </summary>
        private async Task LoadCopyDataAsync()
        {
            if (!CopySaleId.HasValue)
            {
                return;
            }

            var source = await _context.Sales
                .Include(s => s.Customer)
                .Include(s => s.SaleDetails)
                    .ThenInclude(d => d.Product)
                .FirstOrDefaultAsync(s => s.Id == CopySaleId.Value);

            if (source == null)
            {
                return;
            }

            CopySourceCode = source.SaleCode;
            Sale.CustomerId = source.CustomerId;

            var items = source.SaleDetails
                .Where(d => d.ProductId > 0 && d.Quantity > 0)
                .Select(d => new
                {
                    productId = d.ProductId,
                    productName = d.Product?.ProductName ?? "",
                    unitPrice = d.UnitPrice,
                    unit = d.Product?.Unit ?? "",
                    quantity = d.Quantity,
                    notes = d.Notes ?? ""
                })
                .ToList();

            CopyDetailsJson = System.Text.Json.JsonSerializer.Serialize(items);
        }

        private async Task LoadPrintDataAsync(int saleId)
        {
            PrintSale = await _context.Sales
                .Include(s => s.Customer)
                .Include(s => s.SaleDetails)
                    .ThenInclude(d => d.Product)
                .FirstOrDefaultAsync(s => s.Id == saleId);
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
