using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using MessagePack;

namespace ScavengeRUs.Models.Entities
{
    /// <summary>
    /// This is the model which correlates to the AccessCode table in the database.
    /// The properties in this class are the columns in the table, except a few.
    /// </summary>
    public class AccessCode
    {
        public int Id { get; set; }
        [DisplayName("Access Code")]
        public string? Code { get; set; }
        public int HuntId { get; set; } //Foreign key
        [NotMapped]
        public Hunt? Hunt{ get; set; } //Navigation property
        public ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
    }
}
