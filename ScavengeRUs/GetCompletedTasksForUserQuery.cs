using Microsoft.EntityFrameworkCore;
using ScavengeRUs.Data;
using ScavengeRUs.Models.Entities;
using Location = Microsoft.CodeAnalysis.Location;

namespace ScavengeRUs;

public class GetCompletedTasksForUserQuery
{
    private readonly ApplicationDbContext _dbContext;

    public GetCompletedTasksForUserQuery(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Models.Entities.Location>> GetAsync(int huntId)
    {
        var hunt = await _dbContext.Hunts.FirstOrDefaultAsync(h => h.Id == huntId);
        var location = await _dbContext.Location.ToListAsync();
        return location;
    }
}