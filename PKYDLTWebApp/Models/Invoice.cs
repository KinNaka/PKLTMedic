using System.ComponentModel.DataAnnotations;

namespace ClinicManagement.Models
{
    /// <summary>
    /// Model Hóa Đơn
    /// Hóa đơn được in từ các đơn bán hàng, có thể có nhiều hóa đơn từ một đơn bán
    /// </summary>
    public class Invoice
    {
        // ============ PRIMARY KEY ============
        public int Id { get; set; }


        // ============ KHÓA NGOÀI ============

        /// <summary>Khóa ngoài - Đơn bán hàng</summary>
        [Required]
        public int SaleId { get; set; }


        /// <summary>Navigation property - Đơn bán hàng</summary>
        public Sale? Sale { get; set; }


        /// <summary>Khóa ngoài - Người tạo hóa đơn (staff)</summary>
        public int? CreatedByUserId { get; set; }


        /// <summary>Navigation property - Người tạo</summary>
        public User? CreatedByUser { get; set; }


        // ============ THÔNG TIN HÓA ĐƠN ============

        /// <summary>Số hiệu hóa đơn (VD: HĐ001/2026, distinct từ Sale Code)</summary>
        [Required]
        [StringLength(50)]
        public string InvoiceNumber { get; set; } = string.Empty;


        /// <summary>Ngày lập hóa đơn</summary>
        [Required]
        public DateTime InvoiceDate { get; set; } = DateTime.Now;


        /// <summary>Loại hóa đơn (Tờ rơi, Hóa đơn GTGT, Chứng từ tự in)</summary>
        [StringLength(100)]
        public string InvoiceType { get; set; } = "Chứng từ tự in";


        // ============ THÔNG TIN KHÁCH ============

        /// <summary>Tên khách hàng (có thể khác với trong DB nếu khách lẻ)</summary>
        [StringLength(200)]
        public string CustomerName { get; set; } = string.Empty;


        /// <summary>Địa chỉ khách hàng</summary>
        [StringLength(300)]
        public string? CustomerAddress { get; set; }


        /// <summary>Số điện thoại khách hàng</summary>
        [StringLength(20)]
        public string? CustomerPhone { get; set; }


        /// <summary>Email khách hàng</summary>
        [StringLength(100)]
        public string? CustomerEmail { get; set; }


        // ============ THÔNG TIN TÀI CHÍNH ============

        /// <summary>Tổng tiền hàng</summary>
        public decimal SubTotal { get; set; } = 0;


        /// <summary>Chiết khấu</summary>
        public decimal DiscountAmount { get; set; } = 0;


        /// <summary>Tiền thuế VAT</summary>
        public decimal VATAmount { get; set; } = 0;


        /// <summary>Tổng tiền phải thanh toán</summary>
        public decimal TotalAmount { get; set; } = 0;


        /// <summary>Số tiền khách đã trả</summary>
        public decimal PaidAmount { get; set; } = 0;


        /// <summary>Ghi chú thanh toán (VD: tiền mặt, chuyển khoản, v.v.)</summary>
        [StringLength(100)]
        public string? PaymentNote { get; set; }


        // ============ TRẠNG THÁI ============

        /// <summary>Trạng thái hóa đơn (Chưa in, Đã in, Huỷ)</summary>
        [StringLength(50)]
        public string Status { get; set; } = "Chưa in";


        /// <summary>Số lần in</summary>
        public int PrintCount { get; set; } = 0;


        /// <summary>Lần in cuối cùng</summary>
        public DateTime? LastPrintedDate { get; set; }


        // ============ THÔNG TIN QUẢN LÝ ============

        /// <summary>Mô tả/Ghi chú bổ sung</summary>
        [StringLength(500)]
        public string? Notes { get; set; }


        /// <summary>Ngày tạo record</summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;


        /// <summary>Lần cập nhật gần nhất</summary>
        public DateTime? UpdatedAt { get; set; }


        // ============ PHƯƠNG THỨC ============

        /// <summary>Cập nhật lần in cuối cùng</summary>
        public void MarkAsPrinted()
        {
            Status = "Đã in";
            PrintCount++;
            LastPrintedDate = DateTime.Now;
        }


        /// <summary>Tính lại tổng tiền từ Sale</summary>
        public void SyncFromSale()
        {
            if (Sale != null)
            {
                SubTotal = Sale.SubTotal;
                DiscountAmount = Sale.DiscountAmount;
                VATAmount = Sale.VATAmount;
                TotalAmount = Sale.TotalAmount;
                PaidAmount = Sale.PaidAmount;

                if (Sale.Customer != null)
                {
                    CustomerName = Sale.Customer.FullName;
                    CustomerPhone = Sale.Customer.Phone;
                    CustomerAddress = Sale.Customer.Address;
                }
                else if (!string.IsNullOrEmpty(Sale.Notes))
                {
                    CustomerName = "Khách lẻ";
                }
            }
        }
    }
}
