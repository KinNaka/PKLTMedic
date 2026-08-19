using System.ComponentModel.DataAnnotations;

namespace ClinicManagement.Models
{
    /// <summary>
    /// Model Điều Chỉnh Kho / Kiểm Kho
    /// Ghi nhận mỗi lần tinh chỉnh số tồn kho để khớp với số tồn thực tế
    /// (tránh lệch thực tồn: hao hụt, thừa, hư hỏng, hết hạn, nhập sai số liệu, ...)
    /// </summary>
    public class InventoryAdjustment
    {
        // ============ PRIMARY KEY ============
        public int Id { get; set; }


        // ============ KHÓA NGOÀI ============

        /// <summary>Khóa ngoài - Người lập phiếu (user)</summary>
        public int? CreatedByUserId { get; set; }


        /// <summary>Navigation property - Người lập phiếu</summary>
        public User? CreatedByUser { get; set; }


        // ============ THÔNG TIN PHIẾU ĐIỀU CHỈNH ============

        /// <summary>Số hiệu phiếu (VD: ADJ-2026-0001)</summary>
        [Required]
        [StringLength(50)]
        public string AdjustmentCode { get; set; } = string.Empty;


        /// <summary>Ngày kiểm kho / điều chỉnh</summary>
        [Required]
        public DateTime AdjustmentDate { get; set; } = DateTime.Now;


        /// <summary>Loại điều chỉnh (Kiểm kê, Hư hỏng, Hết hạn, Điều chỉnh thủ công, Khác)</summary>
        [Required]
        [StringLength(50)]
        public string AdjustmentType { get; set; } = "Kiểm kê";


        /// <summary>Tổng số dòng sản phẩm được điều chỉnh</summary>
        public int TotalItems { get; set; } = 0;


        /// <summary>Tổng chênh lệch đơn vị (âm = giảm, dương = tăng) so với hệ thống</summary>
        public int TotalQuantityChange { get; set; } = 0;


        /// <summary>Số dòng bị chênh lệch (cần tinh chỉnh)</summary>
        public int AdjustedItems { get; set; } = 0;


        // ============ THÔNG TIN TRẠNG THÁI ============

        /// <summary>Trạng thái phiếu (Hoàn thành, Hủy)</summary>
        [StringLength(50)]
        public string Status { get; set; } = "Hoàn thành";


        // ============ THÔNG TIN QUẢN LÝ ============

        /// <summary>Ghi chú/Mô tả lý do điều chỉnh</summary>
        [StringLength(500)]
        public string? Notes { get; set; }


        /// <summary>Ngày tạo phiếu</summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;


        /// <summary>Lần cập nhật gần nhất</summary>
        public DateTime? UpdatedAt { get; set; }


        // ============ RELATIONSHIPS ============

        /// <summary>Danh sách chi tiết phiếu (từng sản phẩm)</summary>
        public ICollection<InventoryAdjustmentDetail> AdjustmentDetails { get; set; } = new List<InventoryAdjustmentDetail>();
    }
}
