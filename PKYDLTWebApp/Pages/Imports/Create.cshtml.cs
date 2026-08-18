using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ClinicManagement.Data;
using ClinicManagement.Models;
using System.ComponentModel.DataAnnotations;

namespace PKYDLTWebApp.Pages.Imports
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
        [Display(Name = "Nhà cung cấp")]
        public int SupplierId { get; set; }

        [BindProperty]
        [Display(Name = "Ngày nhập hàng")]
        public DateTime ImportDate { get; set; } = DateTime.Now;

        [BindProperty]
        [Display(Name = "Số hóa đơn nhà cung cấp")]
        public string? InvoiceNumber { get; set; }

        [BindProperty]
        [Display(Name = "Ngày hóa đơn")]
        public DateTime? InvoiceDate { get; set; }

        [BindProperty]
        [Display(Name = "Địa chỉ giao hàng")]
        public string? DeliveryAddress { get; set; }

        [BindProperty]
        [Display(Name = "Chiết khấu (₫)")]
        public decimal DiscountAmount { get; set; } = 0;

        [BindProperty]
        [Display(Name = "Thuế VAT (%)")]
        public decimal? VATPercent { get; set; }

        [BindProperty]
        [Display(Name = "Phí vận chuyển (₫)")]
        public decimal ShippingCost { get; set; } = 0;

        [BindProperty]
        [Display(Name = "Phương thức thanh toán")]
        public string? PaymentMethod { get; set; } = "Tiền mặt";

        [BindProperty]
        [Display(Name = "Số tiền đã trả (₫)")]
        public decimal PaidAmount { get; set; } = 0;

        [BindProperty]
        [Display(Name = "Ghi chú")]
        public string? Notes { get; set; }

        [BindProperty]
        public List<ImportOrderDetail> ImportDetails { get; set; } = new();

        // ============ DỮ LIỆU HIỂN THỊ ============

        public List<Supplier> Suppliers { get; set; } = new();
        public List<Product> Products { get; set; } = new();
        public string ImportCode { get; set; } = string.Empty;


        // ============ PAGE HANDLER ============

        public async Task OnGetAsync()
        {
            await LoadDataAsync();
            ImportCode = await GenerateImportCodeAsync();
        }


        public async Task<IActionResult> OnPostAsync()
        {
            // Bảo vệ: nếu ngày nhập trống (01/01/0001) thì dùng ngày hiện tại
            if (ImportDate < new DateTime(2000, 1, 1))
            {
                ImportDate = DateTime.Now;
            }

            if (SupplierId <= 0)
            {
                ModelState.AddModelError("SupplierId", "Vui lòng chọn nhà cung cấp");
            }

            var validDetails = ImportDetails?
                .Where(d => d.ProductId > 0 && d.Quantity > 0)
                .ToList() ?? new List<ImportOrderDetail>();

            if (!validDetails.Any())
            {
                ModelState.AddModelError("", "Vui lòng thêm ít nhất một sản phẩm");
            }

            if (!ModelState.IsValid)
            {
                await LoadDataAsync();
                ImportCode = await GenerateImportCodeAsync();
                return Page();
            }

            var importOrder = new ImportOrder
            {
                SupplierId = SupplierId,
                ImportCode = await GenerateImportCodeAsync(),
                ImportDate = ImportDate,
                InvoiceNumber = InvoiceNumber,
                InvoiceDate = InvoiceDate,
                DeliveryAddress = DeliveryAddress,
                DiscountAmount = DiscountAmount,
                VATPercent = VATPercent,
                ShippingCost = ShippingCost,
                PaymentMethod = PaymentMethod,
                PaidAmount = PaidAmount,
                Notes = Notes,
                Status = "Đã nhập",
                PaymentStatus = PaidAmount > 0 ? "Đã thanh toán" : "Chưa thanh toán",
                CreatedByUserId = await GetCurrentUserIdAsync(),
                CreatedAt = DateTime.Now,
                ImportDetails = new List<ImportOrderDetail>()
            };

            foreach (var d in validDetails)
            {
                var detail = new ImportOrderDetail
                {
                    ProductId = d.ProductId,
                    Quantity = d.Quantity,
                    UnitPrice = d.UnitPrice,
                    ExpiryDate = d.ExpiryDate,
                    BatchNumber = d.BatchNumber,
                    ReceivedQuantity = d.Quantity,
                    Notes = d.Notes
                };
                detail.CalculateTotal();
                importOrder.ImportDetails.Add(detail);

                await UpdateInventoryAndProductAsync(detail, importOrder.ImportDate);
            }

            importOrder.TotalItems = importOrder.ImportDetails.Sum(x => x.Quantity);
            importOrder.CalculateTotal();

            _context.ImportOrders.Add(importOrder);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Tạo đơn nhập hàng thành công. Mã đơn: " + importOrder.ImportCode;
            return RedirectToPage("Index");
        }


        // ============ HELPERS ============

        private async Task UpdateInventoryAndProductAsync(ImportOrderDetail detail, DateTime importDate)
        {
            var inventory = await _context.Inventories
                .FirstOrDefaultAsync(i => i.ProductId == detail.ProductId);

            if (inventory != null)
            {
                inventory.Quantity += detail.ReceivedQuantity;
                inventory.LastReceivedDate = importDate;
                inventory.UpdatedAt = DateTime.Now;
            }
            else
            {
                _context.Inventories.Add(new ClinicManagement.Models.Inventory
                {
                    ProductId = detail.ProductId,
                    Quantity = detail.ReceivedQuantity,
                    MinimumQuantity = 10,
                    LastReceivedDate = importDate,
                    Status = "Sẵn",
                    CreatedAt = DateTime.Now
                });
            }

            var product = await _context.Products.FindAsync(detail.ProductId);
            if (product != null)
            {
                if (detail.UnitPrice > 0)
                {
                    product.CostPrice = detail.UnitPrice;
                }
                if (detail.ExpiryDate.HasValue)
                {
                    product.ExpiryDate = detail.ExpiryDate;
                }
                if (!string.IsNullOrWhiteSpace(detail.BatchNumber))
                {
                    product.BatchNumber = detail.BatchNumber;
                }
                product.UpdatedAt = DateTime.Now;
            }
        }

        private async Task<int?> GetCurrentUserIdAsync()
        {
            if (User.Identity == null || string.IsNullOrWhiteSpace(User.Identity.Name))
                return null;

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == User.Identity.Name);

            return user?.Id;
        }

        private async Task<string> GenerateImportCodeAsync()
        {
            var last = await _context.ImportOrders
                .OrderByDescending(io => io.Id)
                .Select(io => io.ImportCode)
                .FirstOrDefaultAsync();

            int num = 1;
            if (!string.IsNullOrEmpty(last))
            {
                var parts = last.Split('-');
                if (parts.Length >= 2 && int.TryParse(parts[^1], out int parsed))
                {
                    num = parsed + 1;
                }
            }

            return $"IM-{DateTime.Now:yyyyMMdd}-{num:D4}";
        }

        private async Task LoadDataAsync()
        {
            Suppliers = await _context.Suppliers
                .Where(s => s.IsActive)
                .OrderBy(s => s.SupplierName)
                .ToListAsync();

            Products = await _context.Products
                .Where(p => p.IsActive)
                .OrderBy(p => p.ProductCode)
                .ToListAsync();
        }
    }
}