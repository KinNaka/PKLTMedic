using System.ComponentModel.DataAnnotations;

namespace ClinicManagement.Models
{
    /// <summary>
    /// Model Chi Tiết Bán Hàng
    /// Mỗi dòng sản phẩm trong một đơn bán hàng
    /// </summary>
    public class SaleDetail
    {
        // ============ PRIMARY KEY ============
        public int Id { get; set; }


        // ============ KHÓA NGOÀI ============

        /// <summary>Khóa ngoài - Đơn bán hàng</summary>
        [Required]
        public int SaleId { get; set; }


        /// <summary>Navigation property - Đơn bán hàng</summary>
        public Sale? Sale { get; set; }


        /// <summary>Khóa ngoài - Sản phẩm</summary>
        [Required]
        public int ProductId { get; set; }


        /// <summary>Navigation property - Sản phẩm</summary>
        public Product? Product { get; set; }


        // ============ THÔNG TIN CHI TIẾT ============

        /// <summary>Số lượng bán</summary>
        [Required]
        public int Quantity { get; set; } = 1;


        /// <summary>Đơn giá bán</summary>
        [Required]
        public decimal UnitPrice { get; set; } = 0;


        /// <summary>Thành tiền (Quantity x UnitPrice)</summary>
        public decimal Total { get; set; } = 0;


        /// <summary>Chiết khấu trên dòng này (%)</summary>
        public decimal? DiscountPercent { get; set; } = 0;


        /// <summary>Chiết khấu trên dòng này (tiền)</summary>
        public decimal DiscountAmount { get; set; } = 0;


        /// <summary>Ghi chú (VD: hàng tặng kèm, hết hạn sắp, v.v.)</summary>
        [StringLength(200)]
        public string? Notes { get; set; }


        // ============ TÍNH TOÁN ============

        /// <summary>Cập nhật thành tiền</summary>
        public void CalculateTotal()
        {
            Total = Quantity * UnitPrice;

            if (DiscountPercent.HasValue && DiscountPercent > 0)
            {
                DiscountAmount = Total * (DiscountPercent.Value / 100);
            }
        }


        /// <summary>Lấy tổng thực tế sau chiết khấu</summary>
        public decimal GetFinalTotal()
        {
            return Total - DiscountAmount;
        }
    }
}
