using ScavengeRUs.Models.Entities;

namespace ScavengeRUs.Services;

public interface ILocationRepository
{
    Task<Location?> ReadAsync(int id);
    Task<ICollection<Location>> ReadAllAsync();
    Task UpdateAsync(Location location);  
    Task DeleteAsync(int id);  
    Task<Location> CreateAsync(Location location);
    Task<ICollection<Location>> GetByUserIdAsync(string userId);
    
    
}