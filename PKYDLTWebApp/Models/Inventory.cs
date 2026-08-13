using System.ComponentModel.DataAnnotations;

namespace ClinicManagement.Models
{
    /// <summary>
    /// Model Kho - Quản lý tồn kho sản phẩm
    /// Lưu thông tin số lượng từng sản phẩm trong kho
    /// </summary>
    public class Inventory
    {
        // ============ PRIMARY KEY ============
        public int Id { get; set; }


        // ============ KHÓA NGOÀI ============

        /// <summary>Khóa ngoài - Sản phẩm</summary>
        [Required]
        public int ProductId { get; set; }


        /// <summary>Navigation property - Sản phẩm</summary>
        public Product? Product { get; set; }


        // ============ THÔNG TIN TỒN KHO ============

        /// <summary>Số lượng tồn kho hiện tại</summary>
        [Required]
        public int Quantity { get; set; } = 0;


        /// <summary>Số lượng tối thiểu để nhắc nhở cần nhập hàng</summary>
        public int MinimumQuantity { get; set; } = 10;


        /// <summary>Số lượng tối đa trong kho</summary>
        public int? MaximumQuantity { get; set; }


        /// <summary>Vị trí kho (VD: Kệ A1, Ngăn B2)</summary>
        [StringLength(100)]
        public string? WarehouseLocation { get; set; }


        // ============ THÔNG TIN GIÁM SÁT ============

        /// <summary>Ngày nhập kho gần nhất</summary>
        public DateTime? LastReceivedDate { get; set; }


        /// <summary>Ngày xuất kho gần nhất</summary>
        public DateTime? LastIssuedDate { get; set; }


        /// <summary>Ngày kiểm kho gần nhất</summary>
        public DateTime? LastCountDate { get; set; }


        /// <summary>Số lần bán trong tháng này</summary>
        public int MonthlySalesCount { get; set; } = 0;


        // ============ THÔNG TIN QUẢN LÝ ============

        /// <summary>Mô tả/Ghi chú</summary>
        [StringLength(300)]
        public string? Notes { get; set; }


        /// <summary>Trạng thái (Sẵn, Hư hỏng, Hết hạn, v.v.)</summary>
        [StringLength(50)]
        public string Status { get; set; } = "Sẵn";


        /// <summary>Ngày tạo record</summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;


        /// <summary>Lần cập nhật gần nhất</summary>
        public DateTime? UpdatedAt { get; set; }


        // ============ TÍNH TOÁN ============

        /// <summary>Tính giá trị kho (Quantity x CostPrice)</summary>
        public decimal GetInventoryValue()
        {
            return Product != null ? Quantity * Product.CostPrice : 0;
        }


        /// <summary>Kiểm tra xem có cần nhập hàng không</summary>
        public bool IsLowStock()
        {
            return Quantity <= MinimumQuantity;
        }


        /// <summary>Kiểm tra xem hàng có hết hạn sắp tới không (30 ngày)</summary>
        public bool IsNearExpiry()
        {
            if (Product?.ExpiryDate == null)
                return false;

            return Product.ExpiryDate <= DateTime.Now.AddDays(30) && 
                   Product.ExpiryDate > DateTime.Now;
        }


        /// <summary>Kiểm tra xem hàng đã quá hạn không</summary>
        public bool IsExpired()
        {
            if (Product?.ExpiryDate == null)
                return false;

            return Product.ExpiryDate < DateTime.Now;
        }
    }
}
