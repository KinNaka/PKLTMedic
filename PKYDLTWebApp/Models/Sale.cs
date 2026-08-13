using System.ComponentModel.DataAnnotations;

namespace ClinicManagement.Models
{
    /// <summary>
    /// Model Bán Hàng/Giao Dịch
    /// Ghi nhận mỗi lần bán hàng/thuốc cho khách hàng
    /// </summary>
    public class Sale
    {
        // ============ PRIMARY KEY ============
        public int Id { get; set; }


        // ============ KHÓA NGOÀI ============

        /// <summary>Khóa ngoài - Khách hàng</summary>
        public int? CustomerId { get; set; }


        /// <summary>Navigation property - Khách hàng</summary>
        public Customer? Customer { get; set; }


        /// <summary>Khóa ngoài - Nhân viên bán hàng</summary>
        public int? SalesPersonUserId { get; set; }


        /// <summary>Navigation property - Nhân viên bán hàng</summary>
        public User? SalesPerson { get; set; }


        // ============ THÔNG TIN GỢI MỸ ============

        /// <summary>Số hiệu hóa đơn/đơn bán (VD: SALE-2026-001)</summary>
        [Required]
        [StringLength(50)]
        public string SaleCode { get; set; } = string.Empty;


        /// <summary>Ngày bán</summary>
        [Required]
        public DateTime SaleDate { get; set; } = DateTime.Now;


        // ============ THÔNG TIN TÍNH TOÁN ============

        /// <summary>Tổng tiền hàng (chưa chiết khấu)</summary>
        public decimal SubTotal { get; set; } = 0;


        /// <summary>Chiết khấu tính bằng %</summary>
        public decimal? DiscountPercent { get; set; } = 0;


        /// <summary>Chiết khấu tính bằng tiền</summary>
        public decimal DiscountAmount { get; set; } = 0;


        /// <summary>Tiền thuế VAT</summary>
        public decimal VATAmount { get; set; } = 0;


        /// <summary>Tổng tiền phải thanh toán</summary>
        public decimal TotalAmount { get; set; } = 0;


        /// <summary>Số tiền khách đã trả</summary>
        public decimal PaidAmount { get; set; } = 0;


        /// <summary>Số tiền thối lại/nợ</summary>
        public decimal ChangeAmount { get; set; } = 0;


        // ============ THÔNG TIN THANH TOÁN ============

        /// <summary>Phương thức thanh toán (Tiền mặt, Chuyển khoản, QR, v.v.)</summary>
        [StringLength(100)]
        public string? PaymentMethod { get; set; } = "Tiền mặt";


        /// <summary>Trạng thái thanh toán (Chưa TT, Một phần, Đã TT)</summary>
        [StringLength(50)]
        public string PaymentStatus { get; set; } = "Chưa thanh toán";


        // ============ THÔNG TIN ĐƠNHÀNG ============

        /// <summary>Trạng thái đơn (Mới, Hoàn thành, Hủy)</summary>
        [StringLength(50)]
        public string Status { get; set; } = "Hoàn thành";


        /// <summary>Ghi chú/Mô tả</summary>
        [StringLength(300)]
        public string? Notes { get; set; }


        // ============ THÔNG TIN QUẢN LÝ ============

        /// <summary>Ngày tạo record</summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;


        /// <summary>Lần cập nhật gần nhất</summary>
        public DateTime? UpdatedAt { get; set; }


        /// <summary>Người lập đơn</summary>
        public int? CreatedByUserId { get; set; }


        // ============ RELATIONSHIPS ============

        /// <summary>Danh sách chi tiết bán hàng (từng sản phẩm)</summary>
        public ICollection<SaleDetail> SaleDetails { get; set; } = new List<SaleDetail>();


        /// <summary>Hóa đơn liên quan</summary>
        public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();


        // ============ TÍNH TOÁN ============

        /// <summary>Cập nhật tổng tiền</summary>
        public void CalculateTotal()
        {
            SubTotal = SaleDetails?.Sum(x => x.Quantity * x.UnitPrice) ?? 0;

            if (DiscountPercent.HasValue && DiscountPercent > 0)
            {
                DiscountAmount = SubTotal * (DiscountPercent.Value / 100);
            }

            TotalAmount = SubTotal - DiscountAmount + VATAmount;
            ChangeAmount = PaidAmount - TotalAmount;
        }


        /// <summary>Kiểm tra xem đã thanh toán hết chưa</summary>
        public bool IsFullyPaid()
        {
            return Math.Abs(PaidAmount - TotalAmount) >= 0;
        }


        /// <summary>Tính số tiền còn nợ (âm = khách nợ)</summary>
        public decimal GetRemainingAmount()
        {
            return TotalAmount - PaidAmount;
        }
    }
}
