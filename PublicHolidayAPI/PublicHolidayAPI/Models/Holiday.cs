using PublicHolidayAPI.Contracts;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Metrics;

namespace PublicHolidayAPI.Models
{
    public class Holiday
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Country")]
        public int CountryId { get; set; }
        [Required]
        public string Name { get; set; }
        [NotMapped]
        public List<HolidayName> Names { get; set; } = new List<HolidayName>();
        [Required]
        public DateTime Date { get; set; }
        [NotMapped]
        public List<HolidayNote> Notes { get; set; } = new List<HolidayNote>();
        [NotMapped]
        public List<string> Flags { get; set; } = new List<string>();
        [Required]
        public HolidayTypes Type { get; set; }

        public DateTime? ObservedOn { get; set; }

        public Country Country { get; set; }
    }
}
