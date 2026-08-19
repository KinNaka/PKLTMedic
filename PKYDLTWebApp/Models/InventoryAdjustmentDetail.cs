using System.ComponentModel.DataAnnotations;

namespace ClinicManagement.Models
{
    /// <summary>
    /// Model Chi Tiết Phiếu Điều Chỉnh Kho
    /// Mỗi dòng ghi nhận một sản phẩm: số tồn hệ thống -> số tồn thực tế (đếm được)
    /// </summary>
    public class InventoryAdjustmentDetail
    {
        // ============ PRIMARY KEY ============
        public int Id { get; set; }


        // ============ KHÓA NGOÀI ============

        /// <summary>Khóa ngoài - Phiếu điều chỉnh</summary>
        [Required]
        public int InventoryAdjustmentId { get; set; }


        /// <summary>Navigation property - Phiếu điều chỉnh</summary>
        public InventoryAdjustment? InventoryAdjustment { get; set; }


        /// <summary>Khóa ngoài - Sản phẩm</summary>
        [Required]
        public int ProductId { get; set; }


        /// <summary>Navigation property - Sản phẩm</summary>
        public Product? Product { get; set; }


        // ============ THÔNG TIN ĐIỀU CHỈNH ============

        /// <summary>Số tồn trong hệ thống trước khi điều chỉnh</summary>
        [Required]
        public int SystemQuantity { get; set; } = 0;


        /// <summary>Số tồn thực tế đếm được sau điều chỉnh</summary>
        [Required]
        public int ActualQuantity { get; set; } = 0;


        /// <summary>Chênh lệch = ActualQuantity - SystemQuantity (âm = giảm, dương = tăng)</summary>
        public int QuantityChange { get; set; } = 0;


        // ============ THÔNG TIN BỔ SUNG ============

        /// <summary>Lý do chênh lệch (hao hụt, thừa, hư hỏng, ...)</summary>
        [StringLength(200)]
        public string? Reason { get; set; }


        /// <summary>Ghi chú thêm</summary>
        [StringLength(200)]
        public string? Notes { get; set; }
    }
}
