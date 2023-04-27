using ScavengeRUs.Models.Entities;

namespace ScavengeRUs.Services
{
    /// <summary>
    /// This interface is used to add the HuntRepository as a service to the application in the program.cs file. 
    /// The methods below are implemented in the HuntRepository.cs file.
    /// </summary>
    public interface IHuntRepository
    {
        Task<ICollection<Hunt>> ReadAllAsync();
        Task<Hunt>? ReadAsync(int huntId);
        Task<Hunt> ReadHuntWithRelatedData(int huntId);
        Task AddUserToHunt(int huntId, ApplicationUser user);
        Task<Hunt> CreateAsync(Hunt hunt);
        Task DeleteAsync(int huntId);
        Task RemoveUserFromHunt(string username, int huntId);
        Task<ICollection<Location>> GetLocations(ICollection<HuntLocation> huntLocations);
        Task<ICollection<Location>> GetAllLocations();
        Task AddLocation(int locationId, int huntId);
        Task<Location> ReadLocation(int id);
        Task RemoveTaskFromHunt(int id, int huntid);
    }
}
