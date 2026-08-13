using System.ComponentModel.DataAnnotations;

namespace ClinicManagement.Models
{
    /// <summary>
    /// Model Đơn Nhập Hàng
    /// Ghi nhận các lần nhập hàng từ nhà cung cấp
    /// </summary>
    public class ImportOrder
    {
        // ============ PRIMARY KEY ============
        public int Id { get; set; }


        // ============ KHÓA NGOÀI ============

        /// <summary>Khóa ngoài - Nhà cung cấp</summary>
        [Required]
        public int SupplierId { get; set; }


        /// <summary>Navigation property - Nhà cung cấp</summary>
        public Supplier? Supplier { get; set; }


        /// <summary>Khóa ngoài - Người tạo đơn (user)</summary>
        public int? CreatedByUserId { get; set; }


        /// <summary>Navigation property - Người tạo</summary>
        public User? CreatedByUser { get; set; }


        // ============ THÔNG TIN ĐƠN NHẬP ============

        /// <summary>Số hiệu đơn nhập (VD: IM-2026-001)</summary>
        [Required]
        [StringLength(50)]
        public string ImportCode { get; set; } = string.Empty;


        /// <summary>Ngày nhập hàng</summary>
        [Required]
        public DateTime ImportDate { get; set; } = DateTime.Now;


        /// <summary>Ngày hết hạn của đơn (ngày phải thanh toán)</summary>
        public DateTime? DueDate { get; set; }


        // ============ THÔNG TIN HÓA ĐƠN NHÀ CUNG CẤP ============

        /// <summary>Số hóa đơn từ nhà cung cấp</summary>
        [StringLength(50)]
        public string? InvoiceNumber { get; set; }


        /// <summary>Ngày hóa đơn</summary>
        public DateTime? InvoiceDate { get; set; }


        // ============ THÔNG TIN THANH TOÁN ============

        /// <summary>Tổng tiền hàng (chưa chiết khấu)</summary>
        public decimal SubTotal { get; set; } = 0;


        /// <summary>Chiết khấu</summary>
        public decimal DiscountAmount { get; set; } = 0;


        /// <summary>Thuế VAT (%)</summary>
        public decimal? VATPercent { get; set; }


        /// <summary>Tiền thuế VAT</summary>
        public decimal VATAmount { get; set; } = 0;


        /// <summary>Chi phí vận chuyển</summary>
        public decimal ShippingCost { get; set; } = 0;


        /// <summary>Tổng tiền phải thanh toán</summary>
        public decimal TotalAmount { get; set; } = 0;


        /// <summary>Số tiền đã thanh toán</summary>
        public decimal PaidAmount { get; set; } = 0;


        // ============ THÔNG TIN TRẠNG THÁI ============

        /// <summary>Trạng thái đơn (Chờ xác nhận, Đã nhập, Hủy)</summary>
        [StringLength(50)]
        public string Status { get; set; } = "Chờ xác nhận";


        /// <summary>Trạng thái thanh toán (Chưa TT, Một phần, Đã TT)</summary>
        [StringLength(50)]
        public string PaymentStatus { get; set; } = "Chưa thanh toán";


        /// <summary>Phương thức thanh toán (Tiền mặt, Chuyển khoản, v.v.)</summary>
        [StringLength(100)]
        public string? PaymentMethod { get; set; }


        // ============ THÔNG TIN GIAO HÀNG ============

        /// <summary>Địa chỉ giao hàng</summary>
        [StringLength(300)]
        public string? DeliveryAddress { get; set; }


        /// <summary>Ngày giao hàng thực tế</summary>
        public DateTime? ActualDeliveryDate { get; set; }


        /// <summary>Tổng số sản phẩm được giao</summary>
        public int TotalItems { get; set; } = 0;


        // ============ THÔNG TIN QUẢN LÝ ============

        /// <summary>Ghi chú/Mô tả</summary>
        [StringLength(500)]
        public string? Notes { get; set; }


        /// <summary>Người xác nhận (staff)</summary>
        public int? ConfirmedByUserId { get; set; }


        /// <summary>Ngày xác nhận</summary>
        public DateTime? ConfirmedDate { get; set; }


        /// <summary>Ngày tạo record</summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;


        /// <summary>Lần cập nhật gần nhất</summary>
        public DateTime? UpdatedAt { get; set; }


        // ============ RELATIONSHIPS ============

        /// <summary>Danh sách chi tiết đơn nhập (từng sản phẩm)</summary>
        public ICollection<ImportOrderDetail> ImportDetails { get; set; } = new List<ImportOrderDetail>();


        // ============ TÍNH TOÁN ============

        /// <summary>Tính tổng tiền cần thanh toán</summary>
        public void CalculateTotal()
        {
            SubTotal = ImportDetails?.Sum(x => x.Quantity * x.UnitPrice) ?? 0;
            VATAmount = VATPercent.HasValue ? (SubTotal - DiscountAmount) * (VATPercent.Value / 100) : 0;
            TotalAmount = SubTotal - DiscountAmount + VATAmount + ShippingCost;
        }


        /// <summary>Kiểm tra xem đã thanh toán hết chưa</summary>
        public bool IsFullyPaid()
        {
            return Math.Abs(PaidAmount - TotalAmount) < 0.01m;
        }


        /// <summary>Tính số tiền còn nợ</summary>
        public decimal GetRemainingAmount()
        {
            return TotalAmount - PaidAmount;
        }
    }
}
