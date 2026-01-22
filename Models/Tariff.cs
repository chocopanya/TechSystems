using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Windows.Media;

namespace TechSystems.Models
{
    [Table("Tariffs")]
    public class Tariff
    {
        [Key]
        [Column("TariffID")]
        public int Id { get; set; }

        [Column("TariffName")]
        public string Name { get; set; }

        [Column("ServiceTypeID")]
        public int ServiceTypeId { get; set; }

        [ForeignKey("ServiceTypeId")]
        public virtual ServiceType ServiceType { get; set; }

        [Column("SubscriptionMonths")]
        public int SubscriptionMonths { get; set; }

        [Column("StartDate")]
        public DateTime StartDate { get; set; }

        [Column("Price")]
        public decimal Price { get; set; }

        [Column("UserLimit")]
        public int UserLimit { get; set; }

        [Column("AvailableLicenses")]
        public int AvailableLicenses { get; set; }

        [Column("Discount")]
        public decimal Discount { get; set; }

        [Column("PhotoFileName")]
        public string PhotoFileName { get; set; }

        [Column("Description")]
        public string Description { get; set; }

        [NotMapped]
        public decimal FinalPrice => Price * (1 - Discount / 100);

        [NotMapped]
        public bool IsSpecialOffer => Discount > 15;

        [NotMapped]
        public bool IsFewLicenses
        {
            get
            {
                if (UserLimit > 0)
                {
                    double availableRatio = (double)AvailableLicenses / UserLimit;
                    return availableRatio < 0.1;
                }
                return false;
            }
        }

        [NotMapped]
        public bool IsStartingSoon
        {
            get
            {
                TimeSpan timeUntilStart = StartDate - DateTime.Now;
                return timeUntilStart.TotalDays < 7 && timeUntilStart.TotalDays >= 0;
            }
        }

        [NotMapped]
        public double OccupancyPercent
        {
            get
            {
                if (UserLimit > 0)
                {
                    return ((double)(UserLimit - AvailableLicenses) / UserLimit) * 100;
                }
                return 0;
            }
        }

        [NotMapped]
        public bool HasAvailableLicenses => AvailableLicenses > 0;

        [NotMapped]
        public string PhotoPath
        {
            get
            {
                if (!string.IsNullOrEmpty(PhotoFileName))
                {
                    string[] possiblePaths = {
                        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", PhotoFileName),
                        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin", "Debug", "Images", PhotoFileName),
                        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin", "Release", "Images", PhotoFileName),
                        Path.Combine(Environment.CurrentDirectory, "Images", PhotoFileName)
                    };

                    foreach (string path in possiblePaths)
                    {
                        if (File.Exists(path))
                        {
                            return path;
                        }
                    }
                }

                string placeholderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "placeholder.png");
                if (File.Exists(placeholderPath))
                {
                    return placeholderPath;
                }

                return null;
            }
        }

        [NotMapped]
        public Brush OccupancyColor
        {
            get
            {
                double occupancy = OccupancyPercent;
                if (occupancy < 50)
                    return new SolidColorBrush(Color.FromRgb(76, 175, 80));
                if (occupancy < 80)
                    return new SolidColorBrush(Color.FromRgb(255, 193, 7));
                return new SolidColorBrush(Color.FromRgb(244, 67, 54));
            }
        }

        public virtual ICollection<ServiceRequest> Applications { get; set; } = new List<ServiceRequest>();
    }
}