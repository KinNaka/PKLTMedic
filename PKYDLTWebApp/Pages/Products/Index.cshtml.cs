using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ClinicManagement.Data;
using ClinicManagement.Models;

namespace PKYDLTWebApp.Pages.Products
{
    /// <summary>
    /// Trang quản lý danh sách sản phẩm/thuốc
    /// </summary>
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        // ============ PROPERTIES ============

        /// <summary>Danh sách sản phẩm</summary>
        public IList<Product> Products { get; set; } = new List<Product>();

        /// <summary>Tham số tìm kiếm</summary>
        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        /// <summary>Lọc theo loại sản phẩm</summary>
        [BindProperty(SupportsGet = true)]
        public string? Category { get; set; }

        /// <summary>Trang hiện tại</summary>
        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; }

        /// <summary>Danh sách các loại sản phẩm</summary>
        public List<string?> Categories { get; set; } = new();


        // ============ PAGE HANDLER ============

        /// <summary>Load danh sách sản phẩm</summary>
        public async Task OnGetAsync()
        {
            // Lấy danh sách category
            Categories = await _context.Products
                .Select(p => p.Category)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();

            // Query sản phẩm
            var query = _context.Products
                .Include(p => p.Supplier)
                .AsQueryable();

            // Tìm kiếm
            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                query = query.Where(p =>
                    p.ProductCode.Contains(SearchTerm) ||
                    p.ProductName.Contains(SearchTerm)
                );
            }

            // Lọc theo category
            if (!string.IsNullOrWhiteSpace(Category))
            {
                query = query.Where(p => p.Category == Category);
            }

            // Tính tổng số trang
            var totalCount = await query.CountAsync();
            TotalPages = (int)Math.Ceiling(totalCount / (double)PageSize);

            // Lấy dữ liệu cho trang hiện tại
            Products = await query
                .OrderBy(p => p.ProductCode)
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();
        }

        /// <summary>Xóa sản phẩm</summary>
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                TempData["Error"] = "Sản phẩm không tồn tại";
                return RedirectToPage();
            }

            // Kiểm tra xem có trong kho không
            var inventory = await _context.Inventories
                .FirstOrDefaultAsync(i => i.ProductId == id);

            if (inventory != null)
            {
                TempData["Error"] = "Không thể xóa sản phẩm đang có trong kho";
                return RedirectToPage();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Xóa sản phẩm thành công";
            return RedirectToPage();
        }
    }
}
