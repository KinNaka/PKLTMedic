using System.ComponentModel.DataAnnotations;

namespace ClinicManagement.Models
{
    /// <summary>
    /// Model Sản Phẩm/Thuốc
    /// Lưu thông tin chi tiết về các loại thuốc, sản phẩm bán tại phòng khám
    /// </summary>
    public class Product
    {
        // ============ PRIMARY KEY ============
        public int Id { get; set; }


        // ============ THÔNG TIN CƠ BẢN ============

        /// <summary>Mã sản phẩm (VD: THUOC001)</summary>
        [Required]
        [StringLength(50)]
        public string ProductCode { get; set; } = string.Empty;


        /// <summary>Tên sản phẩm/thuốc</summary>
        [Required]
        [StringLength(200)]
        public string ProductName { get; set; } = string.Empty;


        /// <summary>Tên khác của sản phẩm (VD: tên chứng chỉ TC bệnh)</summary>
        [StringLength(200)]
        public string? OtherName { get; set; }


        /// <summary>Loại sản phẩm (Thuốc, Nước, Thiết bị y tế, v.v.)</summary>
        [Required]
        [StringLength(100)]
        public string Category { get; set; } = string.Empty;


        // ============ THÔNG TIN QUẢN LÝ KHOÁ ============

        /// <summary>Số lô sản phẩm (batch code)</summary>
        [StringLength(50)]
        public string? BatchNumber { get; set; }


        /// <summary>Ngày hết hạn</summary>
        public DateTime? ExpiryDate { get; set; }


        /// <summary>Đơn vị tính (Vỉ, Lọ, Chai, Tuýp, etc.)</summary>
        [Required]
        [StringLength(20)]
        public string Unit { get; set; } = "Lọ";


        /// <summary>Hàm lượng/liều lượng (VD: 500mg, 10mg/5ml)</summary>
        [StringLength(100)]
        public string? Strength { get; set; }


        // ============ THÔNG TIN GIÁ ============

        /// <summary>Giá nhập từ nhà cung cấp</summary>
        public decimal CostPrice { get; set; } = 0;


        /// <summary>Giá bán lẻ</summary>
        [Required]
        public decimal RetailPrice { get; set; } = 0;


        /// <summary>Giá bán buôn (nếu có)</summary>
        public decimal? WholesalePrice { get; set; }


        /// <summary>Mức chiết khấu (%)</summary>
        public decimal? DiscountPercent { get; set; } = 0;


        // ============ THÔNG TIN NHÀ CUNG CẤP ============

        /// <summary>Khóa ngoài - Nhà cung cấp</summary>
        public int? SupplierId { get; set; }


        /// <summary>Navigation property - Nhà cung cấp</summary>
        public Supplier? Supplier { get; set; }


        // ============ THÔNG TIN QUẢN LÝ ============

        /// <summary>Mô tả chi tiết sản phẩm</summary>
        [StringLength(500)]
        public string? Description { get; set; }


        /// <summary>Ghi chú thêm</summary>
        [StringLength(200)]
        public string? Notes { get; set; }


        /// <summary>Sản phẩm chủ yếu được sử dụng (để hiển thị nổi bật)</summary>
        public bool IsFeatured { get; set; } = false;


        /// <summary>Trạng thái hoạt động</summary>
        public bool IsActive { get; set; } = true;


        /// <summary>Ngày tạo</summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;


        /// <summary>Lần cập nhật gần nhất</summary>
        public DateTime? UpdatedAt { get; set; }


        // ============ RELATIONSHIPS ============

        /// <summary>Danh sách tồn kho</summary>
        public ICollection<Inventory> InventoryItems { get; set; } = new List<Inventory>();


        /// <summary>Danh sách toa bệnh sử dụng</summary>
        public ICollection<PrescriptionDetail> PrescriptionDetails { get; set; } = new List<PrescriptionDetail>();


        /// <summary>Danh sách bán hàng</summary>
        public ICollection<SaleDetail> SaleDetails { get; set; } = new List<SaleDetail>();
    }
}
