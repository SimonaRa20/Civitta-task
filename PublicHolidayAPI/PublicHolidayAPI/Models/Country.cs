using System.ComponentModel.DataAnnotations;

namespace PublicHolidayAPI.Models
{
    public class Country
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string CountryCode { get; set; }
        [Required]
        public string FullName { get; set; }

        public ICollection<Holiday> Holidays { get; set; } = new List<Holiday>();
    }
}
