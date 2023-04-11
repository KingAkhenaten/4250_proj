namespace ScavengeRUs.Models.Entities
{
    public class LocationUser
    {
        /// <summary>
        /// This is the weak entity for the many to many relationship between ApplicationUser and Location(tasks)
        /// </summary>
        public int Id { get; set; } //primary key
        public int locationId { get; set; } //foreign key
        public Location? location { get; set; } //object for the location
        public string userId { get; set; } = string.Empty; //foreign key
        public ApplicationUser? user { get; set; } //object for the user
    }
}
