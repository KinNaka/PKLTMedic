using System.ComponentModel.DataAnnotations;

namespace ClinicManagement.Models
{
    /// <summary>
    /// Model Nhà Cung Cấp
    /// Quản lý thông tin các nhà cung cấp thuốc, sản phẩm
    /// </summary>
    public class Supplier
    {
        // ============ PRIMARY KEY ============
        public int Id { get; set; }


        // ============ THÔNG TIN CƠ BẢN ============

        /// <summary>Tên nhà cung cấp</summary>
        [Required]
        [StringLength(200)]
        public string SupplierName { get; set; } = string.Empty;


        /// <summary>Mã nhà cung cấp (VD: NCC001)</summary>
        [StringLength(50)]
        public string? SupplierCode { get; set; }


        /// <summary>Tên người đại diện / Liên hệ</summary>
        [StringLength(150)]
        public string? ContactPerson { get; set; }


        // ============ THÔNG TIN LIÊN HỆ ============

        /// <summary>Số điện thoại</summary>
        [StringLength(20)]
        public string? Phone { get; set; }


        /// <summary>Email liên hệ</summary>
        [StringLength(100)]
        public string? Email { get; set; }


        /// <summary>Địa chỉ</summary>
        [StringLength(300)]
        public string? Address { get; set; }


        /// <summary>Thành phố</summary>
        [StringLength(100)]
        public string? City { get; set; }


        /// <summary>Mã vùng/Zip code</summary>
        [StringLength(20)]
        public string? ZipCode { get; set; }


        // ============ THÔNG TIN NGÂN HÀNG ============

        /// <summary>Tên tài khoản ngân hàng</summary>
        [StringLength(200)]
        public string? BankAccountName { get; set; }


        /// <summary>Số tài khoản ngân hàng</summary>
        [StringLength(50)]
        public string? BankAccountNumber { get; set; }


        /// <summary>Tên ngân hàng</summary>
        [StringLength(150)]
        public string? BankName { get; set; }


        /// <summary>Mã số thuế</summary>
        [StringLength(50)]
        public string? TaxCode { get; set; }


        // ============ THÔNG TIN QUẢN LÝ ============

        /// <summary>Điều khoản thanh toán (VD: T30 = 30 ngày)</summary>
        [StringLength(100)]
        public string? PaymentTerms { get; set; }


        /// <summary>Chiết khấu mặc định (%)</summary>
        public decimal DefaultDiscountPercent { get; set; } = 0;


        /// <summary>Mô tả/Ghi chú</summary>
        [StringLength(500)]
        public string? Description { get; set; }


        /// <summary>Trạng thái hoạt động</summary>
        public bool IsActive { get; set; } = true;


        /// <summary>Ngày tạo</summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;


        /// <summary>Lần cập nhật gần nhất</summary>
        public DateTime? UpdatedAt { get; set; }


        // ============ RELATIONSHIPS ============

        /// <summary>Danh sách sản phẩm từ nhà cung cấp này</summary>
        public ICollection<Product> Products { get; set; } = new List<Product>();


        /// <summary>Danh sách đơn nhập từ nhà cung cấp</summary>
        public ICollection<ImportOrder> ImportOrders { get; set; } = new List<ImportOrder>();
    }
}
