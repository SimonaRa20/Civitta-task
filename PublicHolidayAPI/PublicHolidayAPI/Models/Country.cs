using Microsoft.AspNetCore.Mvc.RazorPages;
using PublicHolidayAPI.Contracts;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PublicHolidayAPI.Models
{
    public class Country
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string CountryCode { get; set; }
        [NotMapped]
        public List<string> Regions { get; set; } = new List<string>();
        [NotMapped]
        public List<HolidayTypes> HolidayTypes { get; set; }
        [Required]
        public string FullName { get; set; }
        [Required]
        public DateDetails FromDate { get; set; }
        [Required]
        public DateDetails ToDate { get; set; }
    }
}
