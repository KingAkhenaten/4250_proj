using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using ScavengeRUs.Data;
using ScavengeRUs.Models.Entities;
using System.Security.Claims;

namespace ScavengeRUs.Services
{
    /// <summary>
    /// This class includes methods for reading, creating, editing, adding and removing roles to a user. 
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        /// <summary>
        /// Dependency injection that pulls in the database
        /// </summary>q
        /// <param name="db"></param>
        /// <param name="userManager"></param>
        /// <param name="roleManager"></param>
        public UserRepository(ApplicationDbContext db, 
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        /// <summary>
        /// Returns a user object given the username
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<ApplicationUser?> ReadAsync(string userName)
        {
            var user =  _db.Users.FirstOrDefault(u => u.UserName.ToLower() == userName.ToLower());
            if (user != null)
            {
                _db.Entry(user).Reference(h => h.Hunt).Load();
                user.Roles = await _userManager.GetRolesAsync(user);
                await _db.SaveChangesAsync();   
            }
            return user;
        }
        /// <summary>
        /// Passing a user object and a password this creates a new user and adds it to the database
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<ApplicationUser> CreateAsync(
            ApplicationUser user, string password)
        {
            await _userManager.CreateAsync(user, password);
            await AssignUserToRoleAsync(user.UserName, user.Roles.First());
            user.Roles.Add(user.Roles.First());
            return user;
        }
        /// <summary>
        /// This assigns a user to a role passing the username and rolename
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public async Task AssignUserToRoleAsync(string userName, string roleName)
        {
            //var roleCheck = await _roleManager.RoleExistsAsync(roleName);
            //if (!roleCheck)
            //{
            //    await _roleManager.CreateAsync(new IdentityRole(roleName));
            //}
            var user = await ReadAsync(userName);            
            var role = await _roleManager.FindByNameAsync(roleName);           
            if (user != null)
            {
                if (user.Roles.Count != 0)
                {
                    foreach (var item in user.Roles)
                    {
                        await RemoveUserFromRoleAsync(userName, item);
                    }
                    
                }
                user.Roles.Add(role.Name);
                var result = await _userManager.AddToRoleAsync(user, role.Name);
                await _db.SaveChangesAsync();
            }
            
        }
        /// <summary>
        /// This remove a user from a role passing the username and rolename
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public async Task RemoveUserFromRoleAsync(string userName, string roleName)
        {
            var user = await ReadAsync(userName);
            if (user != null)
            {
                if (user.Roles.Contains(roleName))
                {
                    await _userManager.RemoveFromRoleAsync(user, roleName);
                }
            }
        }
        /// <summary>
        /// This returns a list of all the users
        /// </summary>
        /// <returns></returns>
        public async Task<ICollection<ApplicationUser>> ReadAllAsync()
        {
            var users = await _db.Users
                .Include(p => p.Hunt)
                .ToListAsync();
            foreach (var user in users)
            {
                if (user != null)
                {
                    user.Roles = await _userManager.GetRolesAsync(user);
                }
            }
            return users;
        }
        /// <summary>
        /// This updates the a users fields. Passing the username (old user) and user object (new user)
        /// </summary>
        /// <param name="username"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task UpdateAsync(string username, ApplicationUser user)
        {
            var userToUpdate = await ReadAsync(username);
            if(userToUpdate != null)
            {
                userToUpdate.FirstName = user.FirstName;
                userToUpdate.LastName = user.LastName;
                userToUpdate.PhoneNumber = user.PhoneNumber;
                // await RemoveUserFromRoleAsync(username, userToUpdate.Role);
                userToUpdate.Roles.Add(user.Roles.First());
                await AssignUserToRoleAsync(username, user.Roles.First());
                await _db.SaveChangesAsync();
            }
        }
        /// <summary>
        /// This delete a user from the db passing the username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public async Task DeleteAsync(string username)
        {
            var user = _db.Users.FirstOrDefault(u => u.UserName == username);
            if( user != null )
            {
                _db.Remove(user);
                await _db.SaveChangesAsync();
            }
        }
        public async Task AddUserToHunt(string username, Hunt hunt)
        {
            var user = await ReadAsync(username);
            if (user != null)
            {
                user.Hunt = hunt;
                var accessCode = new AccessCode()
                {
                    Code = $"{username}{hunt.Id}"
                };
                user.AccessCode = accessCode;
                await UpdateAsync(username, user);
            }
        }
        public async Task<ApplicationUser> FindByAccessCode(string accessCode)
        {
            if (accessCode == null)
            {
                return null!;
            }
            var user = await _db.Users
                .Include(a => a.Hunt)
                .FirstOrDefaultAsync(u => u.AccessCode!.Code == accessCode);
            return user!;
        }

    }
}
