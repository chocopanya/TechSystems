using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechSystems.Models
{
    [Table("Applications")]
    public class ServiceRequest
    {
        [Key]
        [Column("ApplicationID")]
        public int Id { get; set; }

        [Column("TariffID")]
        public int TariffId { get; set; }

        [ForeignKey("TariffId")]
        public virtual Tariff Tariff { get; set; }

        [Column("ClientID")]
        public int ClientId { get; set; }

        [ForeignKey("ClientId")]
        public virtual User Client { get; set; }

        [Column("ApplicationDate")]
        public DateTime ApplicationDate { get; set; }

        [Column("StatusID")]
        public int StatusId { get; set; }

        [Column("LicenseCount")]
        public int LicenseCount { get; set; }

        [Column("TotalCost")]
        public decimal TotalCost { get; set; }

        [Column("Comment")]
        public string Comment { get; set; }

        [NotMapped]
        public string Status
        {
            get
            {
                if (StatusId == 1) return "Новая";
                if (StatusId == 2) return "В обработке";
                if (StatusId == 3) return "Подтверждена";
                if (StatusId == 4) return "Отменена";
                if (StatusId == 5) return "Завершена";
                return "Неизвестно";
            }
        }

        [NotMapped]
        public bool CanBeConfirmed => StatusId == 1 || StatusId == 2;

        [NotMapped]
        public bool CanBeCancelled => StatusId == 1 || StatusId == 2 || StatusId == 3;
    }
}