using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ClinicManagement.Data;
using ClinicManagement.Models;
using System.ComponentModel.DataAnnotations;

namespace PKYDLTWebApp.Pages.Adjustments
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
        [Display(Name = "Ngày điều chỉnh")]
        public DateTime AdjustmentDate { get; set; } = DateTime.Now;

        [BindProperty]
        [Display(Name = "Loại điều chỉnh")]
        public string AdjustmentType { get; set; } = "Kiểm kê";

        [BindProperty]
        [Display(Name = "Ghi chú")]
        public string? Notes { get; set; }

        [BindProperty]
        public List<InventoryAdjustmentDetail> Details { get; set; } = new();

        // ============ DỮ LIỆU HIỂN THỊ ============

        public List<Product> Products { get; set; } = new();
        public Dictionary<int, int> SystemQuantities { get; set; } = new();
        public string AdjustmentCode { get; set; } = string.Empty;

        public async Task OnGetAsync()
        {
            await LoadDataAsync();
            AdjustmentCode = await GenerateCodeAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (AdjustmentDate < new DateTime(2000, 1, 1))
            {
                AdjustmentDate = DateTime.Now;
            }

            // Nhóm theo sản phẩm để tránh trùng lặp
            var validDetails = Details?
                .Where(d => d.ProductId > 0)
                .GroupBy(d => d.ProductId)
                .Select(g => g.First())
                .ToList() ?? new List<InventoryAdjustmentDetail>();

            if (!validDetails.Any())
            {
                ModelState.AddModelError("", "Vui lòng thêm ít nhất một sản phẩm để tinh chỉnh kho");
                await LoadDataAsync();
                AdjustmentCode = await GenerateCodeAsync();
                return Page();
            }

            var userId = await GetCurrentUserIdAsync();

            var adjustment = new InventoryAdjustment
            {
                AdjustmentCode = await GenerateCodeAsync(),
                AdjustmentDate = AdjustmentDate,
                AdjustmentType = string.IsNullOrWhiteSpace(AdjustmentType) ? "Kiểm kê" : AdjustmentType,
                Notes = Notes,
                Status = "Hoàn thành",
                CreatedByUserId = userId,
                CreatedAt = DateTime.Now
            };

            int totalItems = 0;
            int totalChange = 0;
            int adjustedItems = 0;

            foreach (var d in validDetails)
            {
                var inventory = await _context.Inventories
                    .FirstOrDefaultAsync(i => i.ProductId == d.ProductId);

                int systemQty = inventory?.Quantity ?? 0;
                int actualQty = Math.Max(0, d.ActualQuantity);

                var detail = new InventoryAdjustmentDetail
                {
                    ProductId = d.ProductId,
                    SystemQuantity = systemQty,
                    ActualQuantity = actualQty,
                    QuantityChange = actualQty - systemQty,
                    Reason = d.Reason,
                    Notes = d.Notes
                };

                adjustment.AdjustmentDetails.Add(detail);

                // Cập nhật tồn kho khớp với số thực tế đếm được
                if (inventory != null)
                {
                    inventory.Quantity = actualQty;
                    inventory.LastCountDate = AdjustmentDate;
                    inventory.UpdatedAt = DateTime.Now;
                }
                else
                {
                    _context.Inventories.Add(new ClinicManagement.Models.Inventory
                    {
                        ProductId = d.ProductId,
                        Quantity = actualQty,
                        MinimumQuantity = 10,
                        LastCountDate = AdjustmentDate,
                        Status = "Sẵn",
                        CreatedAt = DateTime.Now
                    });
                }

                totalItems++;
                if (detail.QuantityChange != 0)
                {
                    adjustedItems++;
                }
                totalChange += detail.QuantityChange;
            }

            adjustment.TotalItems = totalItems;
            adjustment.TotalQuantityChange = totalChange;
            adjustment.AdjustedItems = adjustedItems;

            _context.InventoryAdjustments.Add(adjustment);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Tinh chỉnh kho thành công. Mã phiếu: " + adjustment.AdjustmentCode;
            return RedirectToPage("Index");
        }

        // Lấy thông tin nhanh của sản phẩm khi chọn trong form
        public async Task<JsonResult> OnGetProductDetailsAsync(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return new JsonResult(new { error = "Sản phẩm không tồn tại" });
            }

            var inventory = await _context.Inventories
                .FirstOrDefaultAsync(i => i.ProductId == productId);

            return new JsonResult(new
            {
                productId = product.Id,
                productName = product.ProductName,
                unit = product.Unit,
                systemQuantity = inventory?.Quantity ?? 0
            });
        }

        // ============ HELPERS ============

        private async Task LoadDataAsync()
        {
            Products = await _context.Products
                .Where(p => p.IsActive)
                .OrderBy(p => p.ProductCode)
                .ToListAsync();

            SystemQuantities = await _context.Inventories
                .ToDictionaryAsync(i => i.ProductId, i => i.Quantity);
        }

        private async Task<int?> GetCurrentUserIdAsync()
        {
            if (User.Identity == null || string.IsNullOrWhiteSpace(User.Identity.Name))
                return null;

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == User.Identity.Name);

            return user?.Id;
        }

        private async Task<string> GenerateCodeAsync()
        {
            var last = await _context.InventoryAdjustments
                .OrderByDescending(a => a.Id)
                .Select(a => a.AdjustmentCode)
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

            return $"ADJ-{DateTime.Now:yyyyMMdd}-{num:D4}";
        }
    }
}
