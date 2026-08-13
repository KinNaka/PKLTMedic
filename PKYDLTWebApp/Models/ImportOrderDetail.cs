using System.ComponentModel.DataAnnotations;

namespace ClinicManagement.Models
{
    /// <summary>
    /// Model Chi Tiết Đơn Nhập Hàng
    /// Mỗi dòng trong đơn nhập là một ImportOrderDetail
    /// </summary>
    public class ImportOrderDetail
    {
        // ============ PRIMARY KEY ============
        public int Id { get; set; }


        // ============ KHÓA NGOÀI ============

        /// <summary>Khóa ngoài - Đơn nhập</summary>
        [Required]
        public int ImportOrderId { get; set; }


        /// <summary>Navigation property - Đơn nhập</summary>
        public ImportOrder? ImportOrder { get; set; }


        /// <summary>Khóa ngoài - Sản phẩm</summary>
        [Required]
        public int ProductId { get; set; }


        /// <summary>Navigation property - Sản phẩm</summary>
        public Product? Product { get; set; }


        // ============ THÔNG TIN CHI TIẾT ============

        /// <summary>Số lượng nhập</summary>
        [Required]
        public int Quantity { get; set; } = 1;


        /// <summary>Đơn giá nhập</summary>
        [Required]
        public decimal UnitPrice { get; set; } = 0;


        /// <summary>Thành tiền (Quantity x UnitPrice)</summary>
        public decimal Total { get; set; } = 0;


        /// <summary>Hạn sử dụng của lô hàng</summary>
        public DateTime? ExpiryDate { get; set; }


        /// <summary>Số lô/Batch number</summary>
        [StringLength(50)]
        public string? BatchNumber { get; set; }


        /// <summary>Hàng nhập thực tế</summary>
        public int ReceivedQuantity { get; set; } = 0;


        /// <summary>Hàng hư hỏng/mất</summary>
        public int DamagedQuantity { get; set; } = 0;


        /// <summary>Ghi chú (VD: không đủ lô, khác kích cỡ, v.v.)</summary>
        [StringLength(200)]
        public string? Notes { get; set; }


        // ============ TÍNH TOÁN ============

        /// <summary>Cập nhật thành tiền</summary>
        public void CalculateTotal()
        {
            Total = Quantity * UnitPrice;
        }
    }
}
