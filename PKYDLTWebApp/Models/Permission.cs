using System.ComponentModel.DataAnnotations;

namespace ClinicManagement.Models
{
    /// <summary>
    /// Model Quyền Hạn
    /// Quản lý quyền truy cập chi tiết từng tính năng cho mỗi Role
    /// </summary>
    public class Permission
    {
        // ============ PRIMARY KEY ============
        public int Id { get; set; }


        // ============ KHÓA NGOÀI ============

        /// <summary>Khóa ngoài - Role được gán quyền</summary>
        [Required]
        public int RoleId { get; set; }


        /// <summary>Navigation property - Role</summary>
        public Role? Role { get; set; }


        // ============ THÔNG TIN QUYỀN ============

        /// <summary>Tên module/tính năng (VD: "Product", "Inventory", "Sale")</summary>
        [Required]
        [StringLength(100)]
        public string ModuleName { get; set; } = string.Empty;


        /// <summary>Hành động cụ thể (View, Create, Edit, Delete, Print)</summary>
        [Required]
        [StringLength(50)]
        public string Action { get; set; } = string.Empty;


        /// <summary>Mô tả quyền hạn (VD: "Xem danh sách sản phẩm")</summary>
        [StringLength(200)]
        public string? Description { get; set; }


        /// <summary>Được phép hay không</summary>
        public bool IsGranted { get; set; } = true;


        // ============ THÔNG TIN QUẢN LÝ ============

        /// <summary>Ngày tạo</summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;


        /// <summary>Lần cập nhật gần nhất</summary>
        public DateTime? UpdatedAt { get; set; }


        // ============ UNIQUE CONSTRAINT ============
        // [RoleId + ModuleName + Action] phải duy nhất
    }


    /// <summary>
    /// Enum danh sách các modules/tính năng
    /// </summary>
    public enum ModuleEnum
    {
        // Qản lý sản phẩm
        Product,

        // Quản lý kho
        Inventory,

        // Quản lý nhà cung cấp
        Supplier,

        // Quản lý nhập hàng
        ImportOrder,

        // Quản lý bán hàng
        Sale,

        // Quản lý hóa đơn
        Invoice,

        // Quản lý toa bệnh
        Prescription,

        // Quản lý khách hàng
        Customer,

        // Quản lý người dùng
        User,

        // Quản lý vai trò
        Role,

        // Báo cáo
        Report,

        // Cài đặt
        Settings
    }


    /// <summary>
    /// Enum danh sách các hành động
    /// </summary>
    public enum ActionEnum
    {
        // Xem danh sách
        View,

        // Tạo mới
        Create,

        // Chỉnh sửa
        Edit,

        // Xóa
        Delete,

        // In
        Print,

        // Xuất Excel/PDF
        Export,

        // Duyệt/Ghi nhận
        Approve,

        // Huỷ/Hoàn
        Reject
    }
}
