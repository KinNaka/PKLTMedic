using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ClinicManagement.Data;
using ClinicManagement.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace PKYDLTWebApp.Pages.Invoices
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Invoice Invoice { get; set; } = new();

        /// <summary>
        /// Danh sách dòng sản phẩm của hóa đơn (chính là dòng của đơn bán hàng liên kết).
        /// Cho phép thêm / bớt / sửa số lượng, đổi giá và sửa ghi chú từng dòng.
        /// </summary>
        [BindProperty]
        public List<SaleDetail> SaleDetails { get; set; } = new();

        public List<Product> Products { get; set; } = new();

        /// <summary>JSON danh sách dòng hiện tại để JS khởi tạo bảng sản phẩm</summary>
        public string SaleDetailsJson { get; set; } = "[]";

        /// <summary>JSON danh sách sản phẩm để JS thêm dòng mới</summary>
        public string ProductsJson { get; set; } = "[]";

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var entity = await LoadInvoiceAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            Invoice = entity;
            await BuildFormDataAsync(entity.Sale?.SaleDetails?.ToList() ?? new List<SaleDetail>());
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (Invoice.InvoiceDate < new DateTime(2000, 1, 1))
            {
                Invoice.InvoiceDate = DateTime.Now;
            }

            if (string.IsNullOrWhiteSpace(Invoice.InvoiceNumber))
            {
                ModelState.AddModelError("Invoice.InvoiceNumber", "Số hóa đơn không được để trống");
            }

            var invoice = await LoadInvoiceAsync(Invoice.Id);
            if (invoice == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                Invoice.Sale = invoice.Sale;
                await BuildFormDataAsync(SaleDetails);
                return Page();
            }

            // Kiểm tra trùng số hóa đơn (ngoại trừ chính nó)
            var duplicate = await _context.Invoices
                .AnyAsync(i => i.InvoiceNumber == Invoice.InvoiceNumber.Trim() && i.Id != Invoice.Id);
            if (duplicate)
            {
                ModelState.AddModelError("Invoice.InvoiceNumber", "Số hóa đơn này đã tồn tại");
                Invoice.Sale = invoice.Sale;
                await BuildFormDataAsync(SaleDetails);
                return Page();
            }

            // ============ CHUẨN HÓA CÁC DÒNG SẢN PHẨM GỬI LÊN ============
            var posted = (SaleDetails ?? new List<SaleDetail>())
                .Where(d => d.ProductId > 0 && d.Quantity > 0)
                .ToList();

            if (!posted.Any())
            {
                ModelState.AddModelError("", "Vui lòng giữ ít nhất một sản phẩm trong hóa đơn");
                Invoice.Sale = invoice.Sale;
                await BuildFormDataAsync(SaleDetails);
                return Page();
            }

            // Tổng số lượng MỚI theo từng sản phẩm (có thể trùng sản phẩm ở nhiều dòng)
            var newByProduct = new Dictionary<int, int>();
            var newPriceByProduct = new Dictionary<int, decimal>();
            var newNoteByProduct = new Dictionary<int, string>();
            foreach (var d in posted)
            {
                if (newByProduct.ContainsKey(d.ProductId))
                    newByProduct[d.ProductId] += d.Quantity;
                else
                    newByProduct[d.ProductId] = d.Quantity;

                newPriceByProduct[d.ProductId] = d.UnitPrice;
                newNoteByProduct[d.ProductId] = d.Notes ?? "";
            }

            // Tổng số lượng CŨ theo từng sản phẩm đang có trong dữ liệu
            var oldByProduct = new Dictionary<int, int>();
            if (invoice.Sale?.SaleDetails != null)
            {
                foreach (var d in invoice.Sale.SaleDetails.Where(x => x.Quantity > 0))
                {
                    if (oldByProduct.ContainsKey(d.ProductId))
                        oldByProduct[d.ProductId] += d.Quantity;
                    else
                        oldByProduct[d.ProductId] = d.Quantity;
                }
            }

            // Tập hợp tất cả sản phẩm liên quan (tránh trùng)
            var productIds = new List<int>();
            foreach (var k in oldByProduct.Keys)
                if (!productIds.Contains(k)) productIds.Add(k);
            foreach (var k in newByProduct.Keys)
                if (!productIds.Contains(k)) productIds.Add(k);

            // Nạp tồn kho hiện tại (EF theo dõi để cập nhật)
            var inventories = await _context.Inventories.ToListAsync();
            var invByProduct = inventories.ToDictionary(i => i.ProductId, i => i);

            // ============ KIỂM TRA ĐỦ KHO CHO PHẦN THÊM MỚI ============
            foreach (var pid in productIds)
            {
                var oldQty = oldByProduct.ContainsKey(pid) ? oldByProduct[pid] : 0;
                var newQty = newByProduct.ContainsKey(pid) ? newByProduct[pid] : 0;
                var delta = newQty - oldQty; // > 0 = thêm hàng (cần trừ kho)

                if (delta > 0)
                {
                    var avail = invByProduct.ContainsKey(pid) ? invByProduct[pid].Quantity : 0;
                    if (avail < delta)
                    {
                        var prod = await _context.Products.FindAsync(pid);
                        ModelState.AddModelError("",
                            $"Kho không đủ cho '{prod?.ProductName ?? "?"}' (tồn: {avail}, cần thêm: {delta})");
                    }
                }
            }

            if (!ModelState.IsValid)
            {
                Invoice.Sale = invoice.Sale;
                await BuildFormDataAsync(SaleDetails);
                return Page();
            }

            // ============ CẬP NHẬT KHO THEO CHÊNH LỆCH ============
            // Thêm sản phẩm/số lượng  -> trừ kho
            // Bớt sản phẩm/số lượng   -> cộng lại kho
            foreach (var pid in productIds)
            {
                var oldQty = oldByProduct.ContainsKey(pid) ? oldByProduct[pid] : 0;
                var newQty = newByProduct.ContainsKey(pid) ? newByProduct[pid] : 0;
                // Số lượng trả về kho = oldQty - newQty
                //   -> bớt (newQty < oldQty): cộng lại kho (dương)
                //   -> thêm (newQty > oldQty): trừ kho (âm)
                var stockDelta = oldQty - newQty;

                if (stockDelta != 0 && invByProduct.ContainsKey(pid))
                {
                    var inv = invByProduct[pid];
                    inv.Quantity += stockDelta;
                    if (stockDelta > 0)
                        inv.LastReceivedDate = DateTime.Now;   // nhập về kho
                    else
                        inv.LastIssuedDate = DateTime.Now;      // xuất khỏi kho
                    inv.UpdatedAt = DateTime.Now;
                }
                else if (stockDelta > 0 && !invByProduct.ContainsKey(pid))
                {
                    // Bớt sản phẩm như lâu chưa có dòng kho -> tạo mới với số lượng trả về
                    _context.Inventories.Add(new ClinicManagement.Models.Inventory
                    {
                        ProductId = pid,
                        Quantity = stockDelta,
                        MinimumQuantity = 10,
                        LastReceivedDate = DateTime.Now,
                        Status = "Sẵn",
                        CreatedAt = DateTime.Now
                    });
                }
            }

            // ============ CẬP NHẬT DÒNG SẢN PHẨM CỦA ĐƠN BÁN ============
            var sale = invoice.Sale;
            if (sale != null)
            {
                // Xóa toàn bộ dòng cũ, tạo lại theo danh sách mới đã gộp theo sản phẩm
                if (sale.SaleDetails != null)
                {
                    _context.SaleDetails.RemoveRange(sale.SaleDetails);
                }

                var newLines = new List<SaleDetail>();
                foreach (var pid in newByProduct.Keys)
                {
                    var prod = await _context.Products.FindAsync(pid);
                    newLines.Add(new SaleDetail
                    {
                        SaleId = sale.Id,
                        ProductId = pid,
                        Quantity = newByProduct[pid],
                        UnitPrice = newPriceByProduct.ContainsKey(pid) ? newPriceByProduct[pid] : (prod?.RetailPrice ?? 0),
                        Notes = newNoteByProduct.ContainsKey(pid) ? newNoteByProduct[pid] : null,
                        DiscountPercent = 0,
                        DiscountAmount = 0
                    });
                }

                _context.SaleDetails.AddRange(newLines);
                sale.SaleDetails = newLines;
                sale.CalculateTotal();
                sale.PaymentStatus = (sale.PaidAmount >= sale.TotalAmount && sale.TotalAmount > 0)
                    ? "Đã thanh toán"
                    : (sale.PaidAmount > 0 ? "Một phần" : "Chưa thanh toán");
                sale.UpdatedAt = DateTime.Now;
            }

            // ============ CẬP NHẬT THÔNG TIN HÓA ĐƠN ============
            invoice.InvoiceNumber = Invoice.InvoiceNumber.Trim();
            invoice.InvoiceDate = Invoice.InvoiceDate;
            invoice.InvoiceType = string.IsNullOrWhiteSpace(Invoice.InvoiceType) ? "Chứng từ tự in" : Invoice.InvoiceType;
            invoice.Status = string.IsNullOrWhiteSpace(Invoice.Status) ? "Chưa in" : Invoice.Status;
            invoice.CustomerName = Invoice.CustomerName;
            invoice.CustomerAddress = Invoice.CustomerAddress;
            invoice.CustomerPhone = Invoice.CustomerPhone;
            invoice.CustomerEmail = Invoice.CustomerEmail;
            invoice.DiscountAmount = Invoice.DiscountAmount;
            invoice.VATAmount = Invoice.VATAmount;
            invoice.PaidAmount = Invoice.PaidAmount;
            invoice.PaymentNote = Invoice.PaymentNote;
            invoice.Notes = Invoice.Notes;

            // Tổng tiền hàng lấy lại từ dòng sản phẩm mới
            invoice.SubTotal = sale?.SubTotal ?? 0;
            invoice.TotalAmount = invoice.SubTotal - invoice.DiscountAmount + invoice.VATAmount;
            invoice.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Đã cập nhật hóa đơn " + invoice.InvoiceNumber;
            return RedirectToPage("Index");
        }

        // ============ HELPERS ============

        private async Task<Invoice?> LoadInvoiceAsync(int id)
        {
            return await _context.Invoices
                .Include(i => i.Sale)
                    .ThenInclude(s => s.SaleDetails)
                    .ThenInclude(d => d.Product)
                .Include(i => i.Sale)
                    .ThenInclude(s => s.Customer)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        private async Task BuildFormDataAsync(List<SaleDetail>? lines)
        {
            lines ??= new List<SaleDetail>();

            Products = await _context.Products
                .Where(p => p.IsActive)
                .OrderBy(p => p.ProductName)
                .ToListAsync();

            var stock = await _context.Inventories
                .ToDictionaryAsync(i => i.ProductId, i => i.Quantity);

            SaleDetailsJson = JsonSerializer.Serialize(lines.Select(d => new
            {
                id = d.Id,
                productId = d.ProductId,
                name = d.Product?.ProductName ?? "",
                code = d.Product?.ProductCode ?? "",
                unitPrice = d.UnitPrice,
                unit = d.Product?.Unit ?? "",
                quantity = d.Quantity,
                notes = d.Notes ?? "",
                stock = stock.ContainsKey(d.ProductId) ? stock[d.ProductId] : 0
            }));

            ProductsJson = JsonSerializer.Serialize(Products.Select(p => new
            {
                id = p.Id,
                name = p.ProductName,
                code = p.ProductCode,
                price = p.RetailPrice,
                unit = p.Unit
            }));
        }
    }
}
