using ScavengeRUs.Models.Entities;

namespace ScavengeRUs.Services;

/// <summary>
/// This is supposed to be an interface that would be implemented to call the database methods for the location table.
/// However it is not implemented or used as a service for the application. This could be deleted safely then.
/// </summary>
public interface ILocationRepository
{
    Task<Location?> ReadAsync(int id);
    Task<ICollection<Location>> ReadAllAsync();
    Task UpdateAsync(Location location);  
    Task DeleteAsync(int id);  
    Task<Location> CreateAsync(Location location);
    Task<ICollection<Location>> GetByUserIdAsync(string userId);
    
    
}