using ScavengeRUs.Models.Entities;

namespace ScavengeRUs.Services
{
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
