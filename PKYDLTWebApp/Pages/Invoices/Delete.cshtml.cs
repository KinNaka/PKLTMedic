using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ClinicManagement.Data;
using ClinicManagement.Models;

namespace PKYDLTWebApp.Pages.Invoices
{
    [Authorize]
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Invoice? Invoice { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Invoice = await _context.Invoices
                .Include(i => i.Sale)
                    .ThenInclude(s => s.SaleDetails)
                    .ThenInclude(d => d.Product)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (Invoice == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var invoice = await _context.Invoices
                .Include(i => i.Sale)
                    .ThenInclude(s => s.SaleDetails)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (invoice == null)
            {
                return NotFound();
            }

            var number = invoice.InvoiceNumber;

            // ============ CỘNG LẠI TOÀN BỘ SẢN PHẨM VÀO KHO ============
            // Khi xóa hóa đơn, hoàn lại toàn bộ số lượng sản phẩm đã xuất cho đơn bán.
            if (invoice.Sale?.SaleDetails != null)
            {
                // Gộp số lượng theo từng sản phẩm (phòng trường hợp trùng sản phẩm ở nhiều dòng)
                var productQty = new Dictionary<int, int>();
                foreach (var d in invoice.Sale.SaleDetails.Where(x => x.Quantity > 0))
                {
                    if (productQty.ContainsKey(d.ProductId))
                        productQty[d.ProductId] += d.Quantity;
                    else
                        productQty[d.ProductId] = d.Quantity;
                }

                foreach (var pid in productQty.Keys)
                {
                    var invItem = await _context.Inventories
                        .FirstOrDefaultAsync(i => i.ProductId == pid);

                    if (invItem != null)
                    {
                        invItem.Quantity += productQty[pid];
                        invItem.LastReceivedDate = DateTime.Now;
                        invItem.UpdatedAt = DateTime.Now;
                    }
                    else
                    {
                        _context.Inventories.Add(new ClinicManagement.Models.Inventory
                        {
                            ProductId = pid,
                            Quantity = productQty[pid],
                            MinimumQuantity = 10,
                            LastReceivedDate = DateTime.Now,
                            Status = "Sẵn",
                            CreatedAt = DateTime.Now
                        });
                    }
                }
            }

            _context.Invoices.Remove(invoice);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Đã xóa hóa đơn " + number + " và cộng lại toàn bộ sản phẩm vào kho";
            return RedirectToPage("Index");
        }
    }
}