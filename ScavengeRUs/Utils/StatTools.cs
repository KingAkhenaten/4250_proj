using ScavengeRUs.Models.Entities;
using ScavengeRUs.Services;

namespace ScavengeRUs.Utils;

public static class StatTools
{
   private static readonly ILocationRepository? _locationRepository;

   public static async Task<ICollection<Location>> GetTasks(string usersId)
   {
      var tasks = await _locationRepository.GetByUserIdAsync(usersId);

      return tasks;
   }

   public static async Task<int> GetCompletedTasks(string userId)
   {
      var completedTasks = GetTasks(userId);

      return 5;
   }
}
   
