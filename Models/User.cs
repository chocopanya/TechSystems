using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechSystems.Models
{
    [Table("Users")]
    public class User
    {
        [Key]
        [Column("UserID")]
        public int Id { get; set; }

        [Column("RoleID")]
        public int RoleId { get; set; }

        [Column("FullName")]
        public string FullName { get; set; }

        [Column("Login")]
        public string Login { get; set; }

        [Column("PasswordHash")]
        public string Password { get; set; }

        [Column("Email")]
        public string Email { get; set; }

        [Column("Phone")]
        public string Phone { get; set; }

        [Column("CompanyName")]
        public string CompanyName { get; set; }

        [NotMapped]
        public bool IsAdmin => RoleId == 1;

        [NotMapped]
        public bool IsManager => RoleId == 2;

        [NotMapped]
        public bool IsClient => RoleId == 3;

        public virtual ICollection<ServiceRequest> Applications { get; set; } = new List<ServiceRequest>();
    }
}