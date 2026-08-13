using System.ComponentModel.DataAnnotations;

namespace ClinicManagement.Models
{
    /// <summary>
    /// Model Toa Bệnh / Đơn Thuốc
    /// Lưu lịch sử toa bệnh cho mỗi bệnh nhân, dùng cho kế sinh em/khám
    /// </summary>
    public class Prescription
    {
        // ============ PRIMARY KEY ============
        public int Id { get; set; }


        // ============ KHÓA NGOÀI ============

        /// <summary>Khóa ngoài - Bệnh nhân/Khách hàng</summary>
        [Required]
        public int CustomerId { get; set; }


        /// <summary>Navigation property - Bệnh nhân</summary>
        public Customer? Customer { get; set; }


        /// <summary>Khóa ngoài - Bác sĩ/Y tá lập toa (user)</summary>
        public int? CreatedByUserId { get; set; }


        /// <summary>Navigation property - Người lập toa</summary>
        public User? CreatedByUser { get; set; }


        // ============ THÔNG TIN TỌA ============

        /// <summary>Số hiệu toa bệnh (VD: TOA-2026-001)</summary>
        [Required]
        [StringLength(50)]
        public string PrescriptionCode { get; set; } = string.Empty;


        /// <summary>Ngày lập toa</summary>
        [Required]
        public DateTime PrescriptionDate { get; set; } = DateTime.Now;


        /// <summary>Ngày có hiệu lực (khi nào toa được dùng)</summary>
        public DateTime? EffectiveDate { get; set; }


        /// <summary>Ngày hết hiệu lực</summary>
        public DateTime? ExpiryDate { get; set; }


        // ============ THÔNG TIN BỆNH SRY ============

        /// <summary>Chẩn đoán bệnh</summary>
        [StringLength(300)]
        public string? Diagnosis { get; set; }


        /// <summary>Lý do khám/Triệu chứng</summary>
        [StringLength(300)]
        public string? Symptoms { get; set; }


        /// <summary>Chỉ định/Hướng dẫn điều trị</summary>
        [StringLength(500)]
        public string? Instructions { get; set; }


        // ============ THÔNG TIN VỀ BỆNH NHÂN ============

        /// <summary>Cân nặng (kg) - dùng để tính liều</summary>
        public decimal? Weight { get; set; }


        /// <summary>Chiều cao (cm)</summary>
        public decimal? Height { get; set; }


        /// <summary>Huyết áp</summary>
        [StringLength(50)]
        public string? BloodPressure { get; set; }


        /// <summary>Nhiệt độ cơ thể (°C)</summary>
        public decimal? Temperature { get; set; }


        // ============ TRẠNG THÁI ============

        /// <summary>Trạng thái toa (Hoạt động, Hết hiệu lực, Huỷ)</summary>
        [StringLength(50)]
        public string Status { get; set; } = "Hoạt động";


        /// <summary>Toa có được in chưa</summary>
        public bool IsPrinted { get; set; } = false;


        /// <summary>Lần in cuối</summary>
        public DateTime? LastPrintedDate { get; set; }


        // ============ THÔNG TIN QUẢN LÝ ============

        /// <summary>Ghi chú bác sĩ</summary>
        [StringLength(500)]
        public string? DoctorNotes { get; set; }


        /// <summary>Ngày tạo record</summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;


        /// <summary>Lần cập nhật gần nhất</summary>
        public DateTime? UpdatedAt { get; set; }


        // ============ RELATIONSHIPS ============

        /// <summary>Danh sách chi tiết toa bệnh (từng loại thuốc)</summary>
        public ICollection<PrescriptionDetail> PrescriptionDetails { get; set; } = new List<PrescriptionDetail>();


        // ============ PHƯƠNG THỨC ============

        /// <summary>Kiểm tra xem toa còn hiệu lực không</summary>
        public bool IsValid()
        {
            if (Status != "Hoạt động")
                return false;

            if (ExpiryDate.HasValue && ExpiryDate < DateTime.Now)
                return false;

            return true;
        }


        /// <summary>Đánh dấu toa đã in</summary>
        public void MarkAsPrinted()
        {
            IsPrinted = true;
            LastPrintedDate = DateTime.Now;
        }
    }
}
