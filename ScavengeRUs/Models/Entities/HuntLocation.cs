namespace ScavengeRUs.Models.Entities
{
    public class HuntLocation
    {
        /// <summary>
        /// This model is the weak entity for the many to many relationship between Hunt and Location(tasks).
        /// It correlates to the HuntLocation table in the database and the properties below match the columns in the database,
        /// except for the navigation properties.
        /// </summary>
        public int Id { get; set; }
        public int HuntId { get; set; } //Foreign key
        public Hunt? Hunt { get; set; } //Navigation property
        public int LocationId { get; set; } //Foreign key
        public Location? Location { get; set; } //Navigation property
    }
}
