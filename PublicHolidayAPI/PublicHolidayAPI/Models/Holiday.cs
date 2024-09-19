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
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public string Type { get; set; }
        public ICollection<string> Flags { get; set; } = new List<string>();
        public DateTime? ObservedOn { get; set; }

        public Country Country { get; set; }
    }
}
