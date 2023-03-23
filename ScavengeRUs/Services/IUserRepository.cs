using Microsoft.AspNetCore.Mvc;
using ScavengeRUs.Models.Entities;

namespace ScavengeRUs.Services
{
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
