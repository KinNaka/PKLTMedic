using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ClinicManagement.Data;
using ClinicManagement.Models;
using System.ComponentModel.DataAnnotations;

namespace PKYDLTWebApp.Pages.Products
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Product Product { get; set; } = new();

        // ============ THÔNG TIN TỒN KHO BAN ĐẦU ============
        [BindProperty]
        [Display(Name = "Số lượng tồn kho ban đầu")]
        public int InitialQuantity { get; set; } = 0;

        [BindProperty]
        [Display(Name = "Mức tồn tối thiểu")]
        public int MinimumQuantity { get; set; } = 10;

        [BindProperty]
        [Display(Name = "Vị trí kho")]
        [StringLength(100)]
        public string? WarehouseLocation { get; set; }

        public List<Supplier> Suppliers { get; set; } = new();

        public async Task OnGetAsync()
        {
            Suppliers = await _context.Suppliers
                .Where(s => s.IsActive)
                .OrderBy(s => s.SupplierName)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                Suppliers = await _context.Suppliers
                    .Where(s => s.IsActive)
                    .OrderBy(s => s.SupplierName)
                    .ToListAsync();
                return Page();
            }

            // Kiểm tra mã sản phẩm không trùng
            var existing = await _context.Products
                .FirstOrDefaultAsync(p => p.ProductCode == Product.ProductCode);

            if (existing != null)
            {
                ModelState.AddModelError("Product.ProductCode", "Mã sản phẩm đã tồn tại");
                Suppliers = await _context.Suppliers
                    .Where(s => s.IsActive)
                    .OrderBy(s => s.SupplierName)
                    .ToListAsync();
                return Page();
            }

            Product.CreatedAt = DateTime.Now;
            _context.Products.Add(Product);
            await _context.SaveChangesAsync();

            // ============ TỰ ĐỘNG TẠO TỒN KHO CHO SẢN PHẨM MỚI ============
            var inventory = new ClinicManagement.Models.Inventory
            {
                ProductId = Product.Id,
                Quantity = InitialQuantity,
                MinimumQuantity = MinimumQuantity,
                WarehouseLocation = WarehouseLocation,
                LastReceivedDate = InitialQuantity > 0 ? DateTime.Now : null,
                Status = "Sẵn",
                CreatedAt = DateTime.Now
            };

            _context.Inventories.Add(inventory);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Thêm sản phẩm thành công";
            return RedirectToPage("Index");
        }
    }
}