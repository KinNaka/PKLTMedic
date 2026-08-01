using System.ComponentModel.DataAnnotations;

namespace ClinicManagement.Models
{
    public class Customer
    {
        public int Id { get; set; }

        [Required]
        [StringLength(11)]
        public string CustomerCode { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Họ tên")]
        public string FullName { get; set; }


        [Display(Name = "Ngày sinh")]
        public DateTime? DateOfBirth { get; set; }


        [Display(Name = "Giới tính")]
        public string? Gender { get; set; }


        [Display(Name = "Số điện thoại")]
        public string? Phone { get; set; }


        [Display(Name = "Địa chỉ")]
        public string? Address { get; set; }


        [Display(Name = "Ngày tạo")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}