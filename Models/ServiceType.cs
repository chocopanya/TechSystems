using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechSystems.Models
{
    [Table("ServiceTypes")]
    public class ServiceType
    {
        [Key]
        [Column("ServiceTypeID")]
        public int Id { get; set; }

        [Column("TypeName")]
        public string Name { get; set; }

        public virtual ICollection<Tariff> Tariffs { get; set; } = new List<Tariff>();
    }
}