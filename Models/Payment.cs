using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechSystems.Models
{
    [Table("Payments")]
    public class Payment
    {
        [Key]
        [Column("PaymentID")]
        public int Id { get; set; }

        [Column("ClientID")]
        public int ClientId { get; set; }

        [ForeignKey("ClientId")]
        public virtual User Client { get; set; }

        [Column("PaymentDate")]
        public DateTime PaymentDate { get; set; }

        [Column("Amount")]
        public decimal Amount { get; set; }

        [Column("PaymentMethodID")]
        public int PaymentMethodId { get; set; }

        [ForeignKey("PaymentMethodId")]
        public virtual PaymentMethod PaymentMethod { get; set; }

        [Column("StatusID")]
        public int StatusId { get; set; }

        [ForeignKey("StatusId")]
        public virtual PaymentStatus Status { get; set; }
    }
}