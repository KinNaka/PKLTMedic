using System.ComponentModel.DataAnnotations;

namespace ClinicManagement.Models
{
    public class User
    {
        public int Id { get; set; }


        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;


        [Required]
        public string PasswordHash { get; set; } = string.Empty;


        [Required]
        [StringLength(150)]
        public string FullName { get; set; } = string.Empty;


        [StringLength(20)]
        public string? Phone { get; set; }


        [StringLength(100)]
        public string? Email { get; set; }


        public bool IsActive { get; set; } = true;


        public DateTime CreatedAt { get; set; } = DateTime.Now;



        // Khóa ngoại Role
        public int RoleId { get; set; }


        public Role? Role { get; set; }
    }
}