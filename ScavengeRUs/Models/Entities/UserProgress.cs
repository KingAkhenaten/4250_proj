using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScavengeRUs.Models.Entities
{
    public class UserProgress 
    {
        [Key]
        public string PhoneNumber { get; set; }
        [Required]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        public string Task { get; set; } = string.Empty;
        [Required]
        public DateTime StartTime { get; set; }
        [Required]
        public DateTime EndTime { get; set; }
        [Required]
        public int Rank { get; set; }

        [Display(Name = "Access Code")]
        public string? AccessCode { get; set; }
        public string? Completed { get; set; }
        
    }
}
