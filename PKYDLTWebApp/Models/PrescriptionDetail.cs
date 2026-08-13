using System.ComponentModel.DataAnnotations;

namespace ClinicManagement.Models
{
    /// <summary>
    /// Model Chi Tiết Toa Bệnh
    /// Mỗi dòng thuốc/hướng dẫn dùng trong một toa bệnh
    /// </summary>
    public class PrescriptionDetail
    {
        // ============ PRIMARY KEY ============
        public int Id { get; set; }


        // ============ KHÓA NGOÀI ============

        /// <summary>Khóa ngoài - Toa bệnh</summary>
        [Required]
        public int PrescriptionId { get; set; }


        /// <summary>Navigation property - Toa bệnh</summary>
        public Prescription? Prescription { get; set; }


        /// <summary>Khóa ngoài - Sản phẩm/Thuốc</summary>
        [Required]
        public int ProductId { get; set; }


        /// <summary>Navigation property - Sản phẩm/Thuốc</summary>
        public Product? Product { get; set; }


        // ============ THÔNG TIN LIỀU DÙNG ============

        /// <summary>Số lượng cần dùng</summary>
        [Required]
        public int Quantity { get; set; } = 1;


        /// <summary>Đơn vị liều (VD: viên, ống, ml, vỉ, etc.)</summary>
        [Required]
        [StringLength(20)]
        public string Unit { get; set; } = "Viên";


        /// <summary>Liều lượng mỗi lần (VD: 500mg, 1 viên)</summary>
        [StringLength(100)]
        public string? Dosage { get; set; }


        /// <summary>Tần suất dùng (VD: 3 lần/ngày, 2 lần/ngày sáng tối)</summary>
        [StringLength(150)]
        public string? Frequency { get; set; } = "3 lần/ngày";


        /// <summary>Đường dùng (Uống, Tiêm, Bôi, Nhỏ, Hít, etc.)</summary>
        [StringLength(100)]
        public string? Route { get; set; } = "Uống";


        /// <summary>Thời gian dùng (VD: 7 ngày, 2 tuần)</summary>
        [StringLength(100)]
        public string? Duration { get; set; }


        // ============ THÔNG TIN LƯỚI TÓM TẮT ============

        /// <summary>Tổng số lần dùng (tính từ tần suất x thời gian)</summary>
        public int TotalDoses { get; set; } = 0;


        /// <summary>Ghi chú/Hướng dẫn thêm (VD: sau khi ăn, uống với nước ấm)</summary>
        [StringLength(300)]
        public string? Instructions { get; set; }


        /// <summary>Cảnh báo/Chống chỉ định</summary>
        [StringLength(200)]
        public string? Contraindication { get; set; }


        /// <summary>Tương tác thuốc với thuốc khác (dấu chú ý)</summary>
        public bool HasDrugInteraction { get; set; } = false;


        /// <summary>Dị ứng/Tác dụng phụ cần lưu ý</summary>
        [StringLength(300)]
        public string? SideEffects { get; set; }


        // ============ THỰC HIỆN ============

        /// <summary>Trạng thái (chưa dùng, đang dùng, đã dùng xong)</summary>
        [StringLength(50)]
        public string Status { get; set; } = "Chưa dùng";


        /// <summary>Ngày bắt đầu dùng thực tế (nếu đã dùng)</summary>
        public DateTime? StartDate { get; set; }


        /// <summary>Ngày kết thúc dùng</summary>
        public DateTime? EndDate { get; set; }


        // ============ TÍNH TOÁN ============

        /// <summary>Cập nhật số liều dùng dựa trên frequency và duration</summary>
        public void CalculateTotalDoses()
        {
            // Ví dụ: Frequency = "3 lần/ngày", Duration = "7 ngày"
            // TotalDoses = 3 * 7 = 21
            // Bạn có thể implement logic chi tiết sau
        }
    }
}
