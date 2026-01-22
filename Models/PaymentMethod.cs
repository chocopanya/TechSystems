using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechSystems.Models
{
    [Table("PaymentMethods")]
    public class PaymentMethod
    {
        [Key]
        [Column("MethodID")]
        public int Id { get; set; }

        [Column("MethodName")]
        public string Name { get; set; }
    }
}