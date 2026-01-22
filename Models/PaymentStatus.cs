using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechSystems.Models
{
    [Table("PaymentStatuses")]
    public class PaymentStatus
    {
        [Key]
        [Column("PaymentStatusID")]
        public int Id { get; set; }

        [Column("StatusName")]
        public string Name { get; set; }
    }
}