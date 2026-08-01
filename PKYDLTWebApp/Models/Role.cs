using System.ComponentModel.DataAnnotations;

namespace ClinicManagement.Models
{
    public class Role
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;


        [StringLength(200)]
        public string? Description { get; set; }


        public DateTime CreatedAt { get; set; } = DateTime.Now;


        // Một Role có nhiều User
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}