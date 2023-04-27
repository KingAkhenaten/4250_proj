using Microsoft.AspNetCore.Mvc;
using ScavengeRUs.Models.Entities;

namespace ScavengeRUs.Services
{
    /// <summary>
    /// This interface is used to add the UserRepository as a service to the application in the program.cs file. 
    /// The methods below are implemented in the UserRepository.cs file.
    /// </summary>
    public interface IUserRepository
    {
        Task<ApplicationUser?> ReadAsync(string userName);
        Task<ICollection<ApplicationUser>> ReadAllAsync();
        Task UpdateAsync(string userName, ApplicationUser user);  
        Task DeleteAsync(string userName);  
        Task<ApplicationUser> CreateAsync(ApplicationUser user, string password);
        Task AssignUserToRoleAsync(string userName, string roleName);
        Task AddUserToHunt(string username, Hunt hunt);
        Task<ApplicationUser> FindByAccessCode(string accessCode);
    }
}
