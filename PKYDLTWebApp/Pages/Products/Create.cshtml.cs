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

            TempData["Success"] = "Thêm sản phẩm thành công";
            return RedirectToPage("Index");
        }
    }
}
