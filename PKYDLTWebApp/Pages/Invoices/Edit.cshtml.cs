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
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Invoice Invoice { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var entity = await _context.Invoices
                .Include(i => i.Sale)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (entity == null)
            {
                return NotFound();
            }

            Invoice = entity;
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

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var invoice = await _context.Invoices
                .FirstOrDefaultAsync(i => i.Id == Invoice.Id);

            if (invoice == null)
            {
                return NotFound();
            }

            // Kiểm tra trùng số hóa đơn (ngoại trừ chính nó)
            var duplicate = await _context.Invoices
                .AnyAsync(i => i.InvoiceNumber == Invoice.InvoiceNumber.Trim() && i.Id != Invoice.Id);
            if (duplicate)
            {
                ModelState.AddModelError("Invoice.InvoiceNumber", "Số hóa đơn này đã tồn tại");
                return Page();
            }

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

            // Tính lại tổng tiền phải thanh toán
            invoice.TotalAmount = invoice.SubTotal - invoice.DiscountAmount + invoice.VATAmount;
            invoice.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Đã cập nhật hóa đơn " + invoice.InvoiceNumber;
            return RedirectToPage("Index");
        }
    }
}