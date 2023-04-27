using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScavengeRUs.Data;
using ScavengeRUs.Models.Entities;
using System.Runtime.InteropServices;

namespace ScavengeRUs.Services
{
    /// <summary>
    /// This class is the middleware controlling all db queries for hunts. Such as Adding users, Creating hunts, Reading, Deleting, etc
    /// </summary>
    public class HuntRepository : IHuntRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly IUserRepository _userRepo;

        public HuntRepository(ApplicationDbContext db, IUserRepository userRepo)
        {
            _db = db;           //Database injection
            _userRepo = userRepo;   //User repo injection
        }

        /// <summary>
        /// This method adds a hunt to the db passing a Hunt object
        /// </summary>
        /// <returns>The hunt object it adds to the database</returns>
        public async Task<Hunt> CreateAsync(Hunt hunt)
        {
            await _db.Hunts.AddAsync(hunt);
            await _db.SaveChangesAsync();
            return hunt;
        }

        /// <summary>
        /// This method reads all hunts from the db and returns a list of hunts
        /// </summary>
        /// <returns>Returns all hunts in the database as a list</returns>
        public async Task<ICollection<Hunt>> ReadAllAsync()
        {
            var hunts = await _db.Hunts
                .Include(a => a.HuntLocations) //the include statement will also read other table's data that is a part of the hunt, this one makes sure the tasks for a hunt is read with it
                .Include(h => h.Players) //this includes the players that are part of a hunt when it is read, if it didn't have this then the list of players for a hunt would be null after it is read from the database
                .ToListAsync();
            return hunts;
        }

        /// <summary>
        /// This methods returns a hunt passing a huntId
        /// </summary>
        /// <param name="huntId"></param>
        /// <returns>The hunt object that has the id given from the parameter, it will return a new hunt object if the id does not match up to an existing hunt</returns>
        public async Task<Hunt> ReadAsync(int huntId)
        {
            var hunt = await _db.Hunts
                .Include(l => l.HuntLocations) //includes locations of a hunt when it is read
                .Include (l => l.Players) //includes the players of a hunt when it is read
                .FirstOrDefaultAsync(a => a.Id == huntId);

            if (hunt != null)
            {
                _db.Entry(hunt)
                    .Collection(p => p.Players) //loads the collection of players that are a part of this hunt, this should already have been done above though from the include statement...
                    .Load();
                foreach (var player in hunt.Players)
                {
                    _db.Users.Include(u => u.TasksCompleted).Load(); //includes the tasks a player has completed
                }
                return hunt;
            }
            return new Hunt();


        }

        /// <summary>
        /// This method delete a hunt from the db passing the huntId
        /// </summary>
        /// <param name="huntId"></param>
        /// <returns>Nothing</returns>
        public async Task DeleteAsync(int huntId)
        {
            Hunt? hunt = await ReadAsync(huntId);
            if (hunt != null)            
            {
                _db.Hunts.Remove(hunt);
                var list = _db.AccessCodes.Where(a => a.HuntId == huntId);
                foreach (var item in list)
                {
                _db.AccessCodes.Remove(item); //removes access codes for a hunt when it is deleted

                }
                await _db.SaveChangesAsync();
            }
        }

        /// <summary>
        /// This method is similar to the ReadAsync, but it includes the players and access codes associated with a hunt. This is nessesary becasue of the way the database is set up with the foreign keys
        /// </summary>
        /// <param name="huntId"></param>
        /// <returns>The hunt object that matches up with the id given</returns>
        public async Task<Hunt> ReadHuntWithRelatedData(int huntId)
        {
                var hunts = await _db.Hunts
                
                .Include(p => p.Players)
                .ThenInclude(p => p.AccessCode)
                .Include(p => p.HuntLocations)
                .ToListAsync();
                return hunts.FirstOrDefault(a => a.Id == huntId);
        }

        /// <summary>
        /// This method reads a location from the db passing the id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>A location object that matches up with the id given</returns>
        public async Task<Location> ReadLocation(int id)
        {
            return await _db.Location.FirstOrDefaultAsync(a => a.Id == id);
        }

        /// <summary>
        /// This method returns a list of locations passing a list of HuntLocation objects (aka the weak entity since its a many to many relationship)
        /// </summary>
        /// <param name="huntLocations"></param>
        /// <returns>A collection of location objects that correspond to the collection of HuntLocation objects</returns>
        public async Task<ICollection<Location>> GetLocations(ICollection<HuntLocation> huntLocations)
        {
            var location = new Location();
            ICollection<Location> Locations = new List<Location>();
            foreach (var item in huntLocations)
            {
                location = await _db.Location.FirstOrDefaultAsync(a => a.Id == item.LocationId);
                Locations.Add(location);
            }
            return Locations;
        }

        /// <summary>
        /// This returns all locations in the db
        /// </summary>
        /// <returns>A collection of all locations in the database</returns>
        public async Task<ICollection<Location>> GetAllLocations()
        {
            return await _db.Location.ToListAsync();

        }

        /// <summary>
        /// Adds a task(location) to a hunt and creates the relationship in the db
        /// </summary>
        /// <param name="locationId"></param>
        /// <param name="huntId"></param>
        /// <returns>Nothing</returns>
        public async Task AddLocation(int locationId, int huntId)
        {
            var location = await _db.Location.FirstOrDefaultAsync(a => a.Id == locationId);
            var hunt = await _db.Hunts.FirstOrDefaultAsync(a => a.Id == huntId);
            var huntLocation = new HuntLocation() //creates the weak entity used between a hunt and location
            {
                Location = location,
                Hunt = hunt,
            };
            hunt.HuntLocations.Add(huntLocation);
            location.LocationHunts.Add(huntLocation);
            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// This methods adds a user to a hunt passing the huntId and a user
        /// </summary>
        /// <param name="huntId"></param>
        /// <param name="user"></param>
        /// <returns>Nothing</returns>
        public async Task AddUserToHunt(int huntId, ApplicationUser user)
        {
            var hunt = await ReadAsync(huntId);
            if (hunt != null)
            {
                user.Hunt = hunt;
                var existingUser = await _userRepo.ReadAsync(user.UserName);
                if (existingUser == null)
                {
                    user.Roles.Add("Player");
                    await _userRepo.CreateAsync(user, "Etsupass12!"); //this creates a new player using the default password 'Etsupass12!'
                }

                var player = await _userRepo.ReadAsync(user.Id);
                if (player != null)
                {
                    hunt.Players.Add(player);
                    await _db.SaveChangesAsync();
                    await _userRepo.AddUserToHunt(user.Id, hunt);

                }
            }
        }

        /// <summary>
        /// This method removes the relationship between a hunt, but doesnt delete the user passing a username and huntId.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="huntId"></param>
        /// <returns>Nothing</returns>
        public async Task RemoveUserFromHunt(string username, int huntId)
        {
            var user = await _userRepo.ReadAsync(username);
            var hunt = await ReadAsync(huntId);
            if (user != null && hunt != null)
            {
                user.Hunt = null;
                
                hunt.Players.Remove(user);
                await _db.SaveChangesAsync();
            }
        }

        /// <summary>
        /// This method removes the task from a hunt. Deletes the relationship from the HuntLocation table
        /// </summary>
        /// <param name="id"></param>
        /// <param name="huntid"></param>
        /// <returns>Nothing</returns>
        public async Task RemoveTaskFromHunt(int id, int huntid)
        {
            var hunt = await ReadAsync(huntid);
            var huntLocation = hunt.HuntLocations.FirstOrDefault(a => a.LocationId == id);
            hunt.HuntLocations.Remove(huntLocation);
            await _db.SaveChangesAsync();
        }
    }
}
